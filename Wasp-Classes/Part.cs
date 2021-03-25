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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SocialPlatforms;

/*
 * Wasp Part
 * 
 * Contains Fields
 *      - TemplateID
 *      - ID
 *      - Connections
 *      - Active Connections
 *      - Owner
 *      - Parent
 *      - Connection on Parent
 *      - Connection to Parent
 *      - Children
 *      - Connections on Children
 *      - etc...
 * 
 * Contains Methods to
 *      - Freeze
 *      - Enable/Disable
 *      - Clone
 */

public class Part : MonoBehaviour, ICloneable
{

    //local variables
    #region

    Material placedMat;

    

    [SerializeField]
    bool showConnections;

    List<GameObject> connectionHolder;

    [SerializeField]
    List<Vector3> connectionGridPositions = new List<Vector3>();

    #endregion


    //properties
    #region
    [SerializeField]
    private new string name;
    public string Name
    {
        get { return name; }
    }

    [SerializeField]
    private int templateID;
    public int TemplateID
    {
        get { return templateID; }
    }

    [SerializeField]
    private int id;
    public int ID
    {
        get { return id; }
    }

    [SerializeField]
    private int parent;
    public int Parent
    {
        get { return parent; }
        set { parent = value; }
    }

    [SerializeField]
    private List<int> children = new List<int>();
    public List<int> Children
    {
        get { return children; }
        set { children = value; }
    }

    [SerializeField]
    private int parentCon;
    public int ParentCon
    {
        get { return parentCon; }
        set { parentCon = value; }
    }

    int conToParent = -1;
    public int ConToParent
    {
        get
        {
            if (conToParent != -1)
                return conToParent;

            if (parent == -1)
                return -1;
            else
            {
                try
                {
                    Part parentPart = GlobalReferences.FrozenParts[parent].GetComponent<Part>();
                    for (int i = 0; i < parentPart.Children.Count; ++i)
                        if (parentPart.Children[i] == id)
                            return parentPart.ChildCons[i];
                }
                catch { Debug.Log("parent ID not found in Frozen Parts"); }

                return -2;
            }
        }
        set { conToParent = value; }
    }

    [SerializeField]
    private List<int> childCons = new List<int>();
    public List<int> ChildCons
    {
        get { return childCons; }
        set { childCons = value; }
    }

    [SerializeField]
    bool respawn = false;
    public bool Respawn
    {
        get { return respawn; }
        set { respawn = value; }
    }

    [SerializeField]
    int partOwner = 0;
    public int PartOwner
    {
        get { return partOwner; }
        set { partOwner = value; }
    }

    [SerializeField]
    bool disabled = false;
    public bool Disabled { get { return disabled; } }

    public bool isOwner
    {
        get
        {
            if (!BoltNetwork.IsRunning)
                return true;

            if (BoltNetwork.IsServer && partOwner == 0)
                return true;

            if (BoltNetwork.IsClient && BoltNetwork.Server.ConnectionId == partOwner)
                return true;

            return false;
        }
    }

    private Mesh geometry;
    public Mesh Geometry
    {
        get { return geometry; }
    }

    private int spawnNumber = 0;
    public int SpawnNumber
    {
        get { return spawnNumber; }
    }

    [SerializeField]
    private List<Connection> connections = new List<Connection>();
    public List<Connection> Connections
    {
        get { return connections; }
    }

    private List<int> activeConnections = new List<int>();
    public List<int> ActiveConnections
    {
        get { return activeConnections; }
        set { activeConnections = value; }
    }

    private Vector3 gridOverride = new Vector3(-1, -1, -1);
    public Vector3 GridOverride
    {
        get { return gridOverride; }
    }

    [SerializeField]
    private Vector3 partOffset = Vector3.zero;
    public Vector3 PartOffset { get { return partOffset; } }

    public float DistToController
    {
        get { return Vector3.Distance(gameObject.transform.position, GlobalReferences.ControllerPos); }
    }
    #endregion


    //Monobehaviour Methods
    #region
    private void Update()
    {
        if (!showConnections)
            return;

        if (connectionHolder == null)
        {
            GameObject sprite = Resources.Load<GameObject>("ConnectionSprite");
            connectionHolder = new List<GameObject>();
            foreach (Connection c in connections)
            {
                GameObject go = Instantiate(sprite);
                go.transform.position = c.Pln.Origin + c.Pln.ZVector.normalized * 0.002f;
                go.transform.rotation = c.Pln.GetEulerQuaternion();
                go.transform.parent = transform;
                go.transform.localRotation *= Quaternion.Euler(90, 0, 0);
                connectionHolder.Add(go);

                if (ConnectionVoxelContainer.Container[c.GridX, c.GridY, c.GridZ].Contains(c))
                    connectionGridPositions.Add(new Vector3(c.GridX, c.GridY, c.GridZ));
                else
                    connectionGridPositions.Add(Vector3.one * -1);
            }

            this.enabled = false;
        }
    }
    #endregion

    //initialize
    #region
    public void Initialize(string _name, List<Connection> _connections, int _id, int _templateId, Vector3 _partOffset, Mesh _geometry = null, int _spawnNumber = 0, List<Mesh> _collider = null, int _parent = -1)
    {
        name = _name;
        parent = _parent;
        id = _id;
        templateID = _templateId;
        partOffset = _partOffset;
        geometry = _geometry;
        spawnNumber = _spawnNumber;


        int count = 0;
        foreach (Connection _conn in _connections)
        {
            _conn.Pln.Parent = transform;
            connections.Add(_conn);
            activeConnections.Add(count);
            ++count;
            _conn.GenerateRulesTable(GlobalReferences.Rules);
            _conn.ParentPart = this;
        }

        //check geometry and colliders
        #region
        if (geometry != null && (gameObject.GetComponent<MeshFilter>() != null || gameObject.GetComponent<MeshRenderer>() != null))
        {
            if (gameObject.GetComponent<MeshFilter>() != null)
                Destroy(gameObject.GetComponent<MeshFilter>());
            if (gameObject.GetComponent<MeshRenderer>() != null)
                Destroy(gameObject.GetComponent<MeshRenderer>());
        }

        if (geometry != null)
        {
            MeshFilter mf = gameObject.AddComponent<MeshFilter>();
            mf.mesh = geometry;
            MeshRenderer mr = gameObject.AddComponent<MeshRenderer>();
            mr.material = Resources.Load<Material>("Materials/" + MaterialHolder.MatSet + "/unaffectedMaterial");
        }
        else if (geometry == null && (gameObject.GetComponent<MeshFilter>() == null || gameObject.GetComponent<MeshRenderer>() == null))
        {
            throw new System.Exception("Get your Geometry straight!");
        }
        else
        {
            geometry = gameObject.GetComponent<MeshFilter>().sharedMesh;
        }


        if (_collider == null && gameObject.GetComponent<MeshCollider>() == null && gameObject.GetComponent<BoxCollider>() == null && gameObject.GetComponent<SphereCollider>() == null && gameObject.GetComponent<CapsuleCollider>() == null)
        {
            MeshCollider mc = gameObject.AddComponent<MeshCollider>();
            mc.convex = true;
            mc.sharedMesh = geometry;
        }
        else if (_collider != null)
        {
            int j = 0;
            foreach (Mesh m in _collider)
            {
                MeshCollider mc = gameObject.AddComponent<MeshCollider>();
                mc.convex = true;
                mc.sharedMesh = m;
                ++j;
            }

        }
        #endregion

        //Add Components
        #region
        if (gameObject.GetComponent<Rigidbody>() == null)
        {
            Rigidbody rg = gameObject.AddComponent<Rigidbody>();
            rg.useGravity = false;
            rg.drag = 1;
            rg.angularDrag = 3;
        }

        if (gameObject.GetComponent<ConstantForce>() == null)
        {
            gameObject.AddComponent<ConstantForce>();
        }

        if (gameObject.GetComponent<PartBehaviour>() == null)
        {
            PartBehaviour behav = gameObject.AddComponent<PartBehaviour>();
            behav.enabled = false;

        }

        if (gameObject.GetComponent<ConnectionScanning>() == null)
        {
            ConnectionScanning scan = gameObject.AddComponent<ConnectionScanning>();
            scan.enabled = false;
        }
        #endregion
    }

    //Initialize Override
    public void Initialize(List<Connection> _connections)
    {
        int count = 0;
        foreach (Connection _conn in _connections)
        {
            _conn.Pln.Parent = transform;
            connections.Add(_conn);
            activeConnections.Add(count);
            ++count;
            _conn.GenerateRulesTable(GlobalReferences.Rules);
            _conn.ParentPart = this;
        }

        //Add Components
        #region
        if (gameObject.GetComponent<Rigidbody>() == null)
        {
            Rigidbody rg = gameObject.AddComponent<Rigidbody>();
            rg.useGravity = false;
            rg.drag = 1;
            rg.angularDrag = 3;
        }

        if (gameObject.GetComponent<ConstantForce>() == null)
        {
            gameObject.AddComponent<ConstantForce>();
        }

        if (gameObject.GetComponent<PartBehaviour>() == null)
        {
            gameObject.AddComponent<PartBehaviour>();
        }

        if (gameObject.GetComponent<ConnectionScanning>() == null)
        {
            gameObject.AddComponent<ConnectionScanning>();
        }
        #endregion
    }
    #endregion


    //methods
    #region
    public object Clone()
    {
        GameObject go = new GameObject();
        go.transform.position = transform.position;
        go.transform.rotation = transform.rotation;

        List<Connection> _conn = new List<Connection>();
        foreach (Connection c in connections)
        {
            Connection _c = (Connection)c.Clone();
            _c.Pln.Parent = go.transform;
            _conn.Add(_c);
        }

        Part p = go.AddComponent<Part>();
        if (gameObject.GetComponent<MeshCollider>() != null)
        {
            List<Mesh> colMeshes = new List<Mesh>();
            MeshCollider[] cols = gameObject.GetComponents<MeshCollider>();

            foreach (MeshCollider mc in cols)
                colMeshes.Add(mc.sharedMesh);

            p.Initialize(name, _conn, ID, TemplateID, PartOffset, gameObject.GetComponent<MeshFilter>().sharedMesh, spawnNumber, colMeshes);
        }
        else
        {
            if (gameObject.GetComponent<BoxCollider>() != null)
            {
                BoxCollider[] bc = gameObject.GetComponents<BoxCollider>();
                foreach (BoxCollider b in bc)
                {
                    BoxCollider _bc = go.AddComponent<BoxCollider>();
                    _bc.material = b.material;
                    _bc.size = b.size;
                    _bc.center = b.center;
                }
            }
            else if (gameObject.GetComponent<SphereCollider>() != null)
            {
                SphereCollider[] sc = gameObject.GetComponents<SphereCollider>();
                foreach (SphereCollider s in sc)
                {
                    SphereCollider _sc = go.AddComponent<SphereCollider>();
                    _sc.material = s.material;
                    _sc.radius = s.radius;
                    _sc.center = s.center;
                }
            }
            else if (gameObject.GetComponent<CapsuleCollider>() != null)
            {
                CapsuleCollider[] cc = gameObject.GetComponents<CapsuleCollider>();
                foreach (CapsuleCollider c in cc)
                {
                    CapsuleCollider _cc = go.AddComponent<CapsuleCollider>();
                    _cc.material = c.material;
                    _cc.radius = c.radius;
                    _cc.height = c.height;
                    _cc.center = c.center;
                }
            }
            p.Initialize(_conn);
        }
        return go;
    }

    public void ResetPart(List<Rule> rules)
    {
        int count = 0;
        activeConnections = new List<int>();

        foreach (Connection conn in Connections)
        {
            conn.GenerateRulesTable(rules);
            activeConnections.Add(count);
            ++count;
        }

    }

    public void SetActive(Connection c)
    {
        int i = 0;
        foreach (Connection con in Connections)
        {
            if (c == con)
            {
                if (!ActiveConnections.Contains(i))
                {
                    activeConnections.Add(i);
                    ConnectionVoxelContainer.StoreConnection(con);
                    return;
                }
            }
            ++i;
        }
    }

    public void SetActive(int conID)
    {
        if (!ActiveConnections.Contains(conID) && connections.Count > conID && conID >= 0)
        {
            activeConnections.Add(conID);
            ConnectionVoxelContainer.StoreConnection(connections[conID]);
        }
    }

    public void SetInactive(Connection c)
    {
        int i = 0;
        foreach (Connection con in Connections)
        {
            if (c == con)
            {
                if (ActiveConnections.Contains(i))
                {
                    activeConnections.Remove(i);

                    ConnectionVoxelContainer.RemoveConnection(con);
                    return;
                }
            }
            ++i;
        }
    }

    public void SetInactive(int conID)
    {
        if (ActiveConnections.Contains(conID) && connections.Count > conID)
        {
            activeConnections.Remove(conID);
            ConnectionVoxelContainer.RemoveConnection(connections[conID]);
        }
    }

    public override string ToString()
    {
        return Name + "_" + ID.ToString();
    }

    public void FreezePart(int _id = -1)
    {
        if (!TestDeleteSpawn())
        {
            Destroy(gameObject);
            return;
        }

        placedMat = Resources.Load<Material>("Materials/" + MaterialHolder.MatSet + "/PlacedMaterial");
        gameObject.GetComponent<MeshRenderer>().material = placedMat;

        gameObject.layer = 9;

        if (GetComponent<ConstantForce>() != null)
            Destroy(gameObject.GetComponent<ConstantForce>());

        if (GetComponent<Rigidbody>() != null)
            Destroy(gameObject.GetComponent<Rigidbody>());

        if (GetComponent<PartBehaviour>() != null)
            Destroy(gameObject.GetComponent<PartBehaviour>());

        if (GetComponent<ConnectionScanning>() != null)
            Destroy(gameObject.GetComponent<ConnectionScanning>());

        if (GetComponent<KillTimer>() != null)
            Destroy(GetComponent<KillTimer>());

        gameObject.isStatic = true;

        foreach (int i in ActiveConnections)
        {
            ConnectionVoxelContainer.StoreConnection(connections[i]);
        }

        CollisionVoxelContainer.StoreGameObject(gameObject);


        if (GlobalReferences.AffectedParts.Contains(gameObject))
        {
            GlobalReferences.AffectedParts.Remove(gameObject);
        }

        if (GlobalReferences.FreeParts.Contains(gameObject))
        {
            GlobalReferences.FreeParts.Remove(gameObject);
        }

        if (_id == -1)
        {
            id = GlobalReferences.GetNextID();
        }
        else
        {
            id = _id;
        }

        GlobalReferences.FrozenParts.Add((int)ID, gameObject);
        gameObject.name = name + "_" + id;

        if (TICtrlZ.LastID != ID)
            TICtrlZ.History.Push(new HistoryItem(ID));

        //Debug.Log("#43 Block Transform Frozen: " + ID + ", Position = " + transform.position.ToString("F2") + ", Rotation = " + transform.rotation.eulerAngles.ToString("F2"));
    }

    bool TestDeleteSpawn()
    {
        if (PlacementReferences.InfiniteParts)
        {
            return true;
        }

        if (BoltNetwork.IsRunning)
        {
            return true;
        }

        if (Respawn)
        {
            return true;
        }

        foreach (GameObject go in GlobalReferences.FreeParts)
        {
            if (go.GetComponent<Part>().templateID == templateID)
            {
                Destroy(go);

                return true;
            }
        }

        foreach (GameObject go in GlobalReferences.AffectedParts)
        {
            if (go.GetComponent<Part>().templateID == templateID)
            {
                Destroy(go);

                return true;
            }
        }

        return false;
    }

    public void OverrideGridPosition(char dimension, int v)
    {
        if (dimension == 'x')
        {
            gridOverride.x = v;
        }
        else if (dimension == 'y')
        {
            gridOverride.y = v;
        }
        else if (dimension == 'z')
        {
            gridOverride.z = v;
        }
        else
        {
            throw new Exception("dimension must be x, y or z");
        }
    }

    public void ChangeIDLevel(int offset)
    {
        id += offset;
        if (parent != -1)
        {
            parent += offset;
        }
        for (int i = 0; i < children.Count; ++i)
        {
            children[i] = children[i] + offset;
        }
    }

    public void ChangeIDsFromDic(Dictionary<int, int> dic)
    {
        if (parent != -1 && dic.ContainsKey(parent))
        {
            parent = dic[parent];
        }

        if (id != -1 && dic.ContainsKey(id))
        {
            id = dic[id];
        }

        List<int> tempchild = new List<int>();
        for (int i = 0; i < children.Count; ++i)
        {
            if (dic.ContainsKey(children[i]))
            {
                tempchild.Add(dic[children[i]]);
            }
        }

        children = tempchild;

        gameObject.name = name + "_" + id;
    }

    public void ChangeID(int _id)
    {
        if (!GlobalReferences.FrozenParts.ContainsValue(gameObject))
        {
            id = _id;
        }
        else
        {
            throw new Exception("Can't change ID of Frozen Part without recalculating Tree. Use Dictionary for ID changes");
        }
    }

    public void OnDestroy()
    {
        if (id != -1 && TICtrlZ.LastID != ID)
        {
            TICtrlZ.History.Push(new HistoryItem(this, transform.position, transform.rotation));
        }

        try
        {
            GlobalReferences.FrozenParts.Remove((int)ID);
        }
        catch { }
        try
        {
            GlobalReferences.FreeParts.Remove(gameObject);
        }
        catch { }
        try
        {
            GlobalReferences.Parts.Remove(gameObject);
        }
        catch { }
        try
        {
            GlobalReferences.AffectedParts.Remove(gameObject);
        }
        catch { }
    }

    public void Disable()
    {
        if (disabled)
            return;

        foreach (int i in ActiveConnections)
        {
            ConnectionVoxelContainer.RemoveConnection(Connections[i]);
        }

        GetComponent<MeshRenderer>().material = MaterialHolder.DisabledMat;

        disabled = true;
    }

    public void Enable()
    {
        if (!disabled)
            return;

        foreach (int i in ActiveConnections)
        {
            ConnectionVoxelContainer.StoreConnection(Connections[i]);
        }

        GetComponent<MeshRenderer>().material = MaterialHolder.EnabledMat;

        disabled = false;
    }

    public void LocalDelete()
    {
        GlobalReferences.DeletePart(id);

        if (!PlacementReferences.InfiniteParts && Respawn)
        {
            GameObject go = PartsHolder.Holder.SpawnPart(GetComponent<Part>().TemplateID);
            go.GetComponent<Part>().Respawn = true;
        }
    }

    public void Delete()
    {
        if (!BoltNetwork.IsRunning)
            LocalDelete();
        else
        {
            var del = DeleteFrozenBlock.Create();
            del.ID = id;
            del.Send();
        }
    }

    #endregion

}

public class CompareObjectDistance : Comparer<GameObject>
{
    public override int Compare(GameObject x, GameObject y)
    {
        if (x.GetComponent<Part>().DistToController.CompareTo(y.GetComponent<Part>().DistToController) != 0)
        {
            return x.GetComponent<Part>().DistToController.CompareTo(y.GetComponent<Part>().DistToController);
        }
        else
        {
            return 0;
        }
    }
}
