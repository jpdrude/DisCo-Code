/*
Project DisCo (Discrete Choreography) (GPL) initiated by Jan Philipp Drude

This file is part of Project DisCo.

Copyright (c) 2021, Jan Philipp Drude <jpdrude@gmail.com>

A full build of Project DisCo is available at <http://www.project-disco.com>

Project DisCo's underlaying Source Code is free to use; you can 
redistribute it and/or modify it under the terms of the GNU 
General Public License as published by the Free Software Foundation; 
either version 3 of the License, or (at your option) any later version. 

The Project DisCo source code is distributed in the hope that it will 
be useful, but WITHOUT ANY WARRANTY; without even the implied warranty 
of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
with the DisCo-Classes repository; 
If not, see <http://www.gnu.org/licenses/>.

@license GPL-3.0 <https://www.gnu.org/licenses/gpl.html>

The Project DisCo base classes build on Wasp developed by Andrea Rossi.
You can find Wasp at: <https://github.com/ar0551/Wasp>

Significant parts of Project DisCo have been developed by Jan Philipp Drude
as part of research on virtual reality, digital materials and 
discrete design at: 
dMA - digital Methods in Architecture - Prof. Mirco Becker
Leibniz University Hannover
*/

using Bolt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.Newtonsoft.Json;
using System.Text;
using System.IO;
using Valve.VR;
using UdpKit;
using System.Linq;
using UnityEngineInternal;

/*
 * Spawns Parts in Multiplayer
 * 
 * Parts to spawn are put into Container
 * are spawned a certain number per frame to build up
 * 
 * Streams Loads to other clients in Multiplayer
 * Loads aggregatiosn after stream received
 * 
 * Serialization Classes for streaming aggregations
 */

public class NetworkPartSpawner : MonoBehaviour
{
    private const int spawnPerFrame = 100;

    static List<PartSpawnData> data = new List<PartSpawnData>();
    public static List<PartSpawnData> Data { get { return data; } }


    static LoadAggregationContainer loadData = new LoadAggregationContainer();
    public static LoadAggregationContainer LoadData { get { return loadData; } set { loadData = value; } }
    

    // Update is called once per frame
    void Update()
    {
        if (data.Count > spawnPerFrame)
        {
            for (int i = spawnPerFrame - 1; i >= 0; --i)
            {
                PartSpawnData d = data[i];

                if (d.Token.GetType() == typeof(PartToken))
                {
                    SpawnBlock((PartToken)d.Token, d.Position, d.Rotation, d.owner);
                }
                else if (d.Token.GetType() == typeof(PartTokenParent))
                {
                    SpawnBlockParent((PartTokenParent)d.Token, d.Position, d.Rotation, d.owner);
                }

                data.RemoveAt(i);
            }
        }
        else
        {
            foreach (PartSpawnData d in data)
            {
                if (d.Token.GetType() == typeof(PartTokenParent))
                {
                    SpawnBlockParent((PartTokenParent)d.Token, d.Position, d.Rotation, d.owner);
                }
                else if (d.Token.GetType() == typeof(PartToken))
                {
                    SpawnBlock((PartToken)d.Token, d.Position, d.Rotation, d.owner);
                }
            }

            data.Clear();
        }


        if (loadData.data.Count > 0)
            loadData.StreamData(true, NetworkCallbacks.StreamAggregationChannel);
    }


    void SpawnBlock(PartToken token, Vector3 pos, Quaternion rot, int owner)
    {
        if (GlobalReferences.FrozenParts.ContainsKey(token.ID))
            return;

        GameObject newBlock = GlobalReferences.PartSpawner.SpawnGhostPart(token.TemplateID);

        newBlock.transform.position = pos;
        newBlock.transform.rotation = rot;

        Part newPart = newBlock.GetComponent<Part>();

        newPart.PartOwner = owner;

        newPart.FreezePart(token.ID);

        if (!GlobalReferences.PartIDLedger.ContainsKey(token.ID))
            GlobalReferences.PartIDLedger.Add(token.ID, GlobalReferences.GetNextID());

        if (token.Disabled)
        {
            newPart.Disable();
        }
    }

    public static void SpawnBlockParent(PartTokenParent token, Vector3 pos, Quaternion rot, int owner)
    {

        if (GlobalReferences.FrozenParts.ContainsKey(token.ID))
        {
            Debug.LogError("Frozen Parts already containts Key " + token.ID);
            return;
        }

        GameObject newBlock = GlobalReferences.PartSpawner.SpawnGhostPart(token.TemplateID, pos, rot);
        Destroy(newBlock.GetComponent<ConstantForce>());
        Destroy(newBlock.GetComponent<Rigidbody>());

        Part newPart = newBlock.GetComponent<Part>();
        newPart.SetInactive(token.Con);
        newPart.Parent = token.Parent;
        newPart.ParentCon = token.ParentCon;
        newPart.PartOwner = owner;

        if (token.Parent != -1 && GlobalReferences.FrozenParts.ContainsKey(token.Parent))
        {
            Part parentPart = GlobalReferences.FrozenParts[token.Parent].GetComponent<Part>();
            parentPart.Children.Add(token.ID);
            parentPart.ChildCons.Add(token.Con);
            try
            {
                parentPart.SetInactive(parentPart.Connections[token.ParentCon]);
            }
            catch
            {
                Debug.LogError("parentPart: " + parentPart.ID + " doesnt contain connection: " + token.ParentCon);
            }
        }
        else if (token.Parent != -1)
        {
            var wait = newBlock.AddComponent<NetworkPartWaitForParent>();
            wait.Initialize(token.Parent, token.ID, token.Con, token.ParentCon);
        }

        newPart.FreezePart(token.ID);

        if (!GlobalReferences.PartIDLedger.ContainsKey(token.ID)) 
            GlobalReferences.PartIDLedger.Add(token.ID, GlobalReferences.GetNextID());

        if (token.Disabled)
        {
            newPart.Disable();
        }

        //Debug.Log("#43 Block Transform Done: " + token.ID + ", Position = " + newBlock.transform.position.ToString("F2") + ", Rotation = " + newBlock.transform.rotation.eulerAngles.ToString("F2"));
    }
}

public class LoadAggregationContainer
{
    public List<PartSpawnData> data = new List<PartSpawnData>();

    public LoadAggregationContainer() { }

    public LoadAggregationContainer(List<GameObject> gos)
    {
        foreach (GameObject go in gos)
        {
            Part part = go.GetComponent<Part>();

            if (part == null)
                continue;

            PartSpawnData spawnData = new PartSpawnData(go.transform.position, go.transform.rotation, part);
            data.Add(spawnData);
        }
    }

    public void StreamData(bool spawnSelf, UdpChannelName channel, BoltConnection connection = null)
    {
        StringBuilder sb = new StringBuilder(JsonConvert.SerializeObject(this, Formatting.Indented));
        byte[] aggregationData = Encoding.UTF8.GetBytes(sb.ToString());

        if (connection == null)
        {
            foreach (BoltConnection boltCon in BoltNetwork.Connections)
            {
                boltCon.StreamBytes(channel, aggregationData);
            }
        }
        else
        {
            connection.StreamBytes(channel, aggregationData);
        }

        if (spawnSelf)
            NetworkPartSpawner.Data.AddRange(data);

        data.Clear();
    }

    public static void LoadData(byte[] data)
    {
        LoadAggregationContainer container = ReadData(data);

        NetworkPartSpawner.Data.AddRange(container.data);
    }

    public static void UpdateData(byte[] data)
    {
        LoadAggregationContainer container = ReadData(data);
        Dictionary<int, GameObject> checkFrozen = new Dictionary<int, GameObject>();
        foreach (KeyValuePair<int, GameObject> pair in GlobalReferences.FrozenParts)
        {
            checkFrozen.Add(pair.Key, pair.Value);
        }

        foreach (PartSpawnData item in container.data)
        {
            GameObject checkGo = GlobalReferences.FrozenParts[item.id];
            if (checkGo == null)
            {
                Debug.LogError("Part " + item.id + " not found in check");
                continue;
            }

            Vector3 oldPos = checkGo.transform.position;
            List<Connection> connections = checkGo.GetComponent<Part>().Connections;

            if (Vector3.Distance(oldPos, item.Position) > 0.001f)
            {
                CollisionVoxelContainer.RemoveGameObject(checkGo);
                
                foreach(Connection con in connections)
                {
                    ConnectionVoxelContainer.RemoveConnection(con);
                }
            }
            
            checkGo.transform.position = item.Position;
            checkGo.transform.rotation = item.Rotation;

            if (Vector3.Distance(oldPos, item.Position) > 0.001f)
            {
                CollisionVoxelContainer.StoreGameObject(checkGo);

                foreach (Connection con in connections)
                {
                    ConnectionVoxelContainer.StoreConnection(con);
                }
            }

            checkFrozen.Remove(item.id);
        }

        GameObject[] leftovers = checkFrozen.Values.ToArray<GameObject>();

        for (int i = leftovers.Length - 1; i >= 0; --i)
        {
            GameObject go = leftovers[i];
            Part leftover = go.GetComponent<Part>();
            Debug.LogError("Part " + leftover.ID + " was left over in check and will be deleted");
            leftover.LocalDelete();
        }
    }

    static LoadAggregationContainer ReadData(byte[] data)
    {
        string aggregationString;

        using (MemoryStream stream = new MemoryStream(data))
        {
            using (StreamReader sr = new StreamReader(stream))
            {
                aggregationString = sr.ReadToEnd();
            }
        }

        return JsonConvert.DeserializeObject<LoadAggregationContainer>(aggregationString);
    }
}

public class PartSpawnData
{
    public float posX;
    public float posY;
    public float posZ;

    public float rotX;
    public float rotY;
    public float rotZ;
    public float rotW;

    public int owner;

    public int templateID;

    public int id;

    public int parent;

    public int parentCon;

    public int con;

    public bool disabled;

    [JsonIgnore]
    public Vector3 Position
    {
        get { return new Vector3(posX, posY, posZ); }
    }

    [JsonIgnore]
    public Quaternion Rotation
    {
        get { return new Quaternion(rotX, rotY, rotZ, rotW); }
    }

    [JsonIgnore]
    public IProtocolToken Token
    {
        get
        {
            if (parent == -1)
            {
                return new PartToken(templateID, id, disabled);
            }
            else
            {
                return new PartTokenParent(templateID, id, parent, parentCon, con, disabled);
            }
        }
    }

    public PartSpawnData() { }

    public PartSpawnData(Vector3 _position, Quaternion _rotation, Part part)
    {
        posX = _position.x;
        posY = _position.y;
        posZ = _position.z;

        rotX = _rotation.x;
        rotY = _rotation.y;
        rotZ = _rotation.z;
        rotW = _rotation.w;

        owner = part.PartOwner;
        templateID = part.TemplateID;
        id = part.ID;
        parent = part.Parent;
        parentCon = part.ParentCon;
        con = part.ConToParent;
        disabled = part.Disabled;
    }

    public PartSpawnData(Vector3 _position, Quaternion _rotation, int _owner, int _templateID, int _id, int _parent, int _parentCon, int _con, bool _disabled)
    {
        posX = _position.x;
        posY = _position.y;
        posZ = _position.z;

        rotX = _rotation.x;
        rotY = _rotation.y;
        rotZ = _rotation.z;
        rotW = _rotation.w;

        owner = _owner;

        templateID = _templateID;
        id = _id;
        parent = _parent;
        parentCon = _parentCon;
        con = _con;
        disabled = _disabled;
    }


    public PartSpawnData(IProtocolToken _token, Vector3 _position, Quaternion _rotation, int _owner)
    {
        posX = _position.x;
        posY = _position.y;
        posZ = _position.z;

        rotX = _rotation.x;
        rotY = _rotation.y;
        rotZ = _rotation.z;
        rotW = _rotation.w;

        owner = _owner;


        if (_token.GetType() == typeof(PartToken))
        {
            PartToken token = (PartToken)_token;

            templateID = token.TemplateID;
            id = token.ID;
            disabled = token.Disabled;
            parent = -1;
            parentCon = -1;
            con = -1;
        }
        else
        {
            PartTokenParent token = (PartTokenParent)_token;

            templateID = token.TemplateID;
            id = token.ID;
            parent = token.Parent;
            parentCon = token.ParentCon;
            con = token.Con;
            disabled = token.Disabled;
        }
    }
}