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

using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;

using Valve.Newtonsoft.Json;

/*
 * Contains Serialization Classes for Saving an Loading (also to Wasp)
 * Deals with Axis conversion and Template Part zeroing
 * 
 * Serializes/Deserializes Aggregations
 */

//Json Serializable Classes for Saving and Loading
#region
public class JsonAssembly
{
    [JsonProperty(PropertyName = "aggregation_name")]
    public string name;

    public Dictionary<string, JsonPart> parts;

    public JsonAssembly(string _name, Dictionary<string, JsonPart> _parts)
    {
        name = _name;
        parts = _parts;
    }
}

public class JsonPart
{
    public int? parent;

    public string name;

    public JsonTransform transform;

    [JsonProperty("is_constrained")]
    public bool isConstrained = false;

    public int? parentCon;

    [JsonProperty("connection_to_parent")]
    public int? conToParent;

    [JsonProperty("active_connections")]
    public List<int> activeConnections;

    public List<int> children;

    public JsonPart(int _parent, string _name, JsonTransform _transform, int _parentCon, int _conToParent, List<int> _activeCons, List<int> _children)
    {
        parent = _parent;
        name = _name;
        transform = _transform;
        parentCon = _parentCon;
        conToParent = _conToParent;
        activeConnections = _activeCons;
        children = _children;

        if (parent == -1 || parent == null)
        {
            parent = null;
            parentCon = null;
            conToParent = null;
        }

    }
}


public class JsonTransform
{
    public float M00;

    public float M01;

    public float M02;

    public float M03;

    public float M10;

    public float M11;

    public float M12;

    public float M13;

    public float M20;

    public float M21;

    public float M22;

    public float M23;

    public float M30;

    public float M31;

    public float M32;

    public float M33;

    //Constructor
    public JsonTransform(Matrix4x4 transformMatrix)
    {
        M00 = transformMatrix.m00;
        M01 = transformMatrix.m01;
        M02 = transformMatrix.m02;
        M03 = transformMatrix.m03;

        M10 = transformMatrix.m10;
        M11 = transformMatrix.m11;
        M12 = transformMatrix.m12;
        M13 = transformMatrix.m13;

        M20 = transformMatrix.m20;
        M21 = transformMatrix.m21;
        M22 = transformMatrix.m22;
        M23 = transformMatrix.m23;

        M30 = transformMatrix.m30;
        M31 = transformMatrix.m31;
        M32 = transformMatrix.m32;
        M33 = transformMatrix.m33;
    }

    //methods
    #region
    public Matrix4x4 GetMatrix()
    {
        Matrix4x4 m = new Matrix4x4();
        m.m00 = M00;
        m.m01 = M01;
        m.m02 = M02;
        m.m03 = M03;

        m.m10 = M10;
        m.m11 = M11;
        m.m12 = M12;
        m.m13 = M13;

        m.m20 = M20;
        m.m21 = M21;
        m.m22 = M22;
        m.m23 = M23;

        m.m30 = M30;
        m.m31 = M31;
        m.m32 = M32;
        m.m33 = M33;

        return m;
    }

    public static Vector3 MatrixToPosition(Matrix4x4 m)
    {
        Vector3 position = Vector3.zero;
        position.x = m.m03;
        position.y = m.m13;
        position.z = m.m23;

        return position;
    }

    public static Quaternion MatrixToRotation(Matrix4x4 m)
    {
        Vector3 forward;
        forward.x = m.m02;
        forward.y = m.m12;
        forward.z = m.m22;

        Vector3 upwards;
        upwards.x = m.m01;
        upwards.y = m.m11;
        upwards.z = m.m21;

        return Quaternion.LookRotation(forward, upwards);
    }

    public static Vector3 MatrixToScale(Matrix4x4 m)
    {
        Vector3 scale;
        scale.x = new Vector4(m.m00, m.m10, m.m20, m.m30).magnitude;
        scale.y = new Vector4(m.m01, m.m11, m.m21, m.m31).magnitude;
        scale.z = new Vector4(m.m02, m.m12, m.m22, m.m32).magnitude;
        return scale;
    }
    #endregion
}
#endregion

//Class for Saving and Loading
public static class AssemblyIO
{
    public static JsonAssembly Save(Dictionary<int, GameObject> parts, int ID, string path)
    {
        JsonAssembly assembly = BuildAssembly(parts, ID);

        StringBuilder jsonSb;
        jsonSb = new StringBuilder(JsonConvert.SerializeObject(assembly, Formatting.Indented));

        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            using (StreamWriter sw = new StreamWriter(stream))
            {
                sw.Write(jsonSb);
            }
        }

        return assembly;
    }

    private static JsonAssembly BuildAssembly(Dictionary<int, GameObject> parts, int ID)
    {
        Dictionary<string, JsonPart> jsonParts = new Dictionary<string, JsonPart>();

        Matrix4x4 transM = new Matrix4x4(new Vector4(1,0,0,0), new Vector4(0,0,1,0), new Vector4(0,1,0,0), new Vector4(0,0,0,1));

        foreach (KeyValuePair<int, GameObject> dicEntry in parts)
        {
            if (dicEntry.Value != null)
            {
                if (BoltNetwork.IsRunning && !GlobalReferences.PartIDLedger.ContainsKey(dicEntry.Key))
                {
                    GlobalReferences.PartIDLedger.Add(dicEntry.Key, GlobalReferences.GetNextID());
                    Debug.LogError("PartIdLedger didn't contain a Key for " + dicEntry.Key + ", was changed to: " + GlobalReferences.PartIDLedger[dicEntry.Key]);
                }

                Part p = dicEntry.Value.GetComponent<Part>();
                Matrix4x4 m = p.gameObject.transform.localToWorldMatrix;
                
                m = transM * m;
                m = m * Matrix4x4.TRS(p.PartOffset, Quaternion.identity, Vector3.one);
                string templateName = GlobalReferences.TemplateParts[p.TemplateID].GetComponent<Part>().name;

                if (!BoltNetwork.IsRunning)
                {
                    JsonPart jsonP = new JsonPart(p.Parent, templateName, new JsonTransform(m), p.ParentCon, p.ConToParent, p.ActiveConnections, p.Children);
                    jsonParts.Add(dicEntry.Key.ToString(), jsonP);
                }
                else
                {
                    int parent = p.Parent;

                    if (parent != -1 && BoltNetwork.IsRunning)
                    {
                        try
                        {
                            parent = GlobalReferences.PartIDLedger[p.Parent];
                        }
                        catch
                        {
                            Debug.LogError("PartIdLedger didn't contain the parent: " + parent + " for part: " + dicEntry.Key + ", parent will be set to null");
                            parent = -1;
                        }
                    }

                    List<int> children = new List<int>();

                    foreach (int i in p.Children)
                    {
                        if (BoltNetwork.IsRunning)
                        {
                            try
                            {
                                children.Add(GlobalReferences.PartIDLedger[i]);
                            }
                            catch
                            {
                                Debug.LogError("PartIdLedger didn't contain the child: " + i + " for part: " + dicEntry.Key + ", child will be ommited");
                            }
                        }
                        else
                            children.Add(i);
                    }

                    JsonPart jsonP = new JsonPart(parent, templateName, new JsonTransform(m), p.ParentCon, p.ConToParent, p.ActiveConnections, children);

                    if (BoltNetwork.IsRunning)
                        jsonParts.Add(GlobalReferences.PartIDLedger[dicEntry.Key].ToString(), jsonP);
                    else
                        jsonParts.Add(dicEntry.Key.ToString(), jsonP);
                }
            }
        }

        JsonAssembly assembly = new JsonAssembly(ID.ToString(), jsonParts);
        return assembly;
    }



    //Loading
    public static void Load(string path)
    {
        string jsonSr;

        using (FileStream stream = new FileStream(path, FileMode.Open))
        {   
           using (StreamReader sr = new StreamReader(stream))
            {
                jsonSr = sr.ReadToEnd();
            }
        }


        LoadFromString(jsonSr);
    }

    public static void LoadFromString(string jsonSr)
    {

        Dictionary<int, int> localPartIDLedger = new Dictionary<int, int>();

        Dictionary<int, PartSpawnData> spawnPartContainer = new Dictionary<int, PartSpawnData>();

        Dictionary<int, List<int>> childContainer = new Dictionary<int, List<int>>();
        Dictionary<int, List<int>> childConContainer = new Dictionary<int, List<int>>();

        JsonAssembly assembly;

        int numParts = 0;

        Matrix4x4 transM = new Matrix4x4(new Vector4(1, 0, 0, 0), new Vector4(0, 0, 1, 0), new Vector4(0, 1, 0, 0), new Vector4(0, 0, 0, 1));

        assembly = JsonConvert.DeserializeObject<JsonAssembly>(jsonSr);

        foreach (KeyValuePair<string, JsonPart> dicEntry in assembly.parts)
        {
            int id;
            
            if (!BoltNetwork.IsRunning)
                id = int.Parse(dicEntry.Key) + GlobalReferences.NumOfParts;
            else
            {
                id = int.Parse(dicEntry.Key);
                int newID = Random.Range(int.MinValue, int.MaxValue);

                while (localPartIDLedger.ContainsValue(newID) || GlobalReferences.FrozenParts.ContainsKey(newID))
                    newID = Random.Range(int.MinValue, int.MaxValue);

                localPartIDLedger.Add(id, newID);
            }

            JsonPart part = dicEntry.Value;
            int templateId = GlobalReferences.TemplateIDFromName(part.name);
            if (templateId == -1)
            {
                throw new System.Exception("Couldn't find Part from Name");
            }

            Matrix4x4 m = part.transform.GetMatrix();
            m = m * Matrix4x4.TRS(-GlobalReferences.TemplateParts[templateId].GetComponent<Part>().PartOffset, Quaternion.identity, Vector3.one);

            m = transM * m;
            Vector3 scale = JsonTransform.MatrixToScale(m);
            Quaternion rot = JsonTransform.MatrixToRotation(m);
            Vector3 pos = JsonTransform.MatrixToPosition(m);

            string name = part.name + "_" + (id);

            int parent = -1;
            if (part.parent != null)
            {
                parent = (int)part.parent;
                parent += GlobalReferences.NumOfParts;
            }

            int parentCon = -1;
            if (part.parentCon != null)
                parentCon = (int)part.parentCon;

            int con = -1;
            if (part.conToParent != null)
                con = (int)part.conToParent;

            if (BoltNetwork.IsRunning)
            {
                var token = new PartTokenParent();
                token.TemplateID = templateId;
                token.ID = id;

                token.Parent = -1;

                if (part.parent != null)
                    token.Parent = (int)part.parent;

                token.ParentCon = parentCon;

                token.Con = con;

                int owner = 0;

                if (BoltNetwork.IsClient)
                    owner = (int)BoltNetwork.Server.ConnectionId;

                spawnPartContainer.Add(id, new PartSpawnData(token, pos, rot, owner));

            }
            else
            {
                GameObject go = MonoBehaviour.Instantiate(GlobalReferences.TemplateParts[templateId]);
                go.name = name;
                go.SetActive(true);

                PartsHolder.ResetPart(go, GlobalReferences.TemplateParts[templateId], id);

                Part p = go.GetComponent<Part>();


                p.Parent = parent;

                p.ParentCon = parentCon;

                p.ConToParent = con;

                go.transform.rotation = rot;
                go.transform.position = pos;


                p.FreezePart(id);


                if (!GlobalReferences.Parts.Contains(go))
                {
                    GlobalReferences.Parts.Add(go);
                }

                if (id >= numParts)
                {
                    numParts = id + 1;
                }
            }
        }

        if (BoltNetwork.IsRunning)
        {
            foreach (PartSpawnData spawnData in spawnPartContainer.Values)
            {
                //PartTokenParent token = (PartTokenParent)spawnData.Token;
                if (spawnData.parent != -1 && localPartIDLedger.ContainsKey(spawnData.parent))
                    spawnData.parent = localPartIDLedger[spawnData.parent];

                spawnData.id = localPartIDLedger[spawnData.id];

                NetworkPartSpawner.LoadData.data.Add(spawnData);
            }
        }
        else
        {
            foreach(GameObject go in GlobalReferences.FrozenParts.Values)
            {
                Part part = go.GetComponent<Part>();

                if (part.Parent != -1)
                {
                    Part parentPart = GlobalReferences.FrozenParts[part.Parent].GetComponent<Part>();
                    parentPart.Children.Add(part.ID);
                    parentPart.ChildCons.Add(part.ConToParent);
                }
            }

            GlobalReferences.NumOfParts = numParts;
        }
    }
}
