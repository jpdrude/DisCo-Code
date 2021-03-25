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

using Valve.Newtonsoft.Json;
using System.Text;
using System.IO;

/*
 * Wasp Data Serialization
 * 
 * Contains the Serialization Classes and Methods to build the corresponding DisCo Components
 * 
 * - Parts
 *      - Geometry
 *      - Colliders
 *      - Name
 *      - Connections
 * - Rules
 * - Rule Groups
 * - Environment
 *      - Game Area
 *      - Ground Plane
 *      - Additional Geometry
 *      - Blueprint Geometry
 *      - Field Resolution
 */

//Json Serializable Classes for Wasp Connection
#region
public class JsonContainer
{
    [JsonProperty("PartData")]
    public List<PartData> partData;

    [JsonProperty("RuleData")]
    public List<RuleData> ruleData;

    public Environment environment;

    [JsonProperty("RuleGroupsData")]
    public List<RuleGroupsData> ruleGroupsData;
}

public class Environment
{
    public List<AdditionalGeometryData> AdditionalGeometry;

    public List<BluePrintGeometryData> BlueprintGeometry;

    public bool GroundPlane;

    public float FieldResolution;

    public GameAreaData GameArea;
}

public class GameAreaData
{
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;
    public float minZ;
    public float maxZ;
}

public class BluePrintGeometryData
{
    public List<int[]> faces;
    public List<float[]> vertices;
    public string name;
}

public class AdditionalGeometryData
{
    public List<int[]> faces;
    public List<float[]> vertices;
    public string name;
}

public class RuleData
{
    public string part1;

    public int conn1;

    public string part2;

    public int conn2;

    public bool active;
}

public class RuleGroupsData
{
    public string RuleGroupName;

    public string[] RuleGrammar;
}

public class PartData
{
    public string name;

    public int SpawnNumber;

    public PartGeometry geometry;

    public ColliderData collider;

    public List<ConnectionData> connections;

    public float Probability;
}

public class ColliderData
{
    public bool multiple;

    public List<PartGeometry> geometry;
}

public class PartGeometry
{
    public List<int[]> faces;
    public List<float[]> vertices;
}

public class ConnectionData
{
    public int id;

    public string part;

    public string type;

    public ConnectionPlaneData plane;
}

public class ConnectionPlaneData
{
    public float[] origin;
    public float[] xaxis;
    public float[] yaxis;
}
#endregion

//Load Json with Template Parts from Wasp
public static class ImplementWasp
{
    public static List<GameObject> templateParts = new List<GameObject>();

    public static JsonContainer jsonCont = new JsonContainer();

    private static Dictionary<string, int> ruleLookup = new Dictionary<string, int>();

    //main method

    public static Environment Initialize(string path, byte[] data = null)
    {
        templateParts = new List<GameObject>();
        ruleLookup = new Dictionary<string, int>();

        if (path != null)
            jsonCont = LoadFromWasp(path);
        else
            jsonCont = LoadFromWaspFile(data);

        GlobalReferences.RuleMatrix = BuildRuleMatrix();

        GlobalReferences.Rules = LoadRules();

        GlobalReferences.RuleGroups = LoadRuleGrammar();

        GlobalReferences.AdditionalGeometry = BuildAdditional();

        GlobalReferences.BlueprintGeometry = BuildBlueprint();

        return jsonCont.environment;
    }
    
    public static List<GameObject> Load()
    {
        int idCounter = 0;
        foreach (PartData p in jsonCont.partData)
        {
            templateParts.Add(BuildPart(p, idCounter));
            ++idCounter;
        }

        foreach (GameObject templateGo in templateParts)
        {
            if (templateGo.GetComponent<Part>().SpawnNumber > 0)
            {
                PlacementReferences.InfiniteParts = false;
                break;
            }
        } 

        return templateParts;
    }

    //initialization methods
    #region
    private static List<GameObject> BuildAdditional()
    {
        List<GameObject> addGeos = new List<GameObject>();
        Material mat = Resources.Load<Material>("Materials/" + MaterialHolder.MatSet +"/AddGeoMat");

        int i = 0;
        foreach (AdditionalGeometryData add in jsonCont.environment.AdditionalGeometry)
        {
            GameObject go = new GameObject();
            MeshRenderer mr = go.AddComponent<MeshRenderer>();
            MeshFilter mf = go.AddComponent<MeshFilter>();
            MeshCollider mc = go.AddComponent<MeshCollider>();

            Mesh m = BuildObjMesh(add.vertices, add.faces);

            go.transform.RotateAround(Vector3.zero, Vector3.right, -90);
            go.transform.localScale = new Vector3(1, -1, 1);
            go.layer = 10;

            if (add.name != null && add.name != "")
                go.name = add.name;
            else
                 go.name = "AddGeo_" + i.ToString();

            mf.sharedMesh = m;
            mc.sharedMesh = m;
            mr.material = mat;


            addGeos.Add(go);

            ++i;
        }

        return addGeos;
    }

    private static List<GameObject> BuildBlueprint()
    {
        List<GameObject> blueprintGeo = new List<GameObject>();
        Material mat = Resources.Load<Material>("Materials/" + MaterialHolder.MatSet + "/Blueprint");

        int i = 0;
        foreach (BluePrintGeometryData bp in jsonCont.environment.BlueprintGeometry)
        {
            GameObject go = new GameObject();
            MeshRenderer mr = go.AddComponent<MeshRenderer>();
            MeshFilter mf = go.AddComponent<MeshFilter>();

            Mesh m = BuildObjMesh(bp.vertices, bp.faces);

            go.transform.RotateAround(Vector3.zero, Vector3.right, -90);
            go.transform.localScale = new Vector3(1, -1, 1);
            go.layer = 10;

            if (bp.name != null && bp.name != "")
                go.name = bp.name;
            else
                go.name = "AddGeo_" + i.ToString();
            

            mf.sharedMesh = m;
            mr.material = mat;
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;


            blueprintGeo.Add(go);

            ++i;
        }

        return blueprintGeo;
    }

    private static List<Rule> LoadRules()
    {
        List<Rule> rules = new List<Rule>();

        foreach (RuleData r in jsonCont.ruleData)
        {
            if (r.active)
            {
                int row = ruleLookup[r.part1 + "|" + r.conn1];
                int column = ruleLookup[r.part2 + "|" + r.conn2];
                Rule _r = new Rule(r.part1, r.conn1, r.part2, r.conn2, row, column, true);
                GlobalReferences.RuleMatrix[row, column] = true;
                rules.Add(_r);
            }
        }

        return rules;

    }

    private static List<int[]> BuildRuleSet(RuleGroupsData rg)
    {
        List<int[]> groups = new List<int[]>();
        foreach (RuleData r in jsonCont.ruleData)
        {
            foreach (string _rg in rg.RuleGrammar)
            {
                
                if (string.Compare(_rg, BuildRuleGrammar(r)) == 0)
                {
                    int row = ruleLookup[r.part1 + "|" + r.conn1];
                    int column = ruleLookup[r.part2 + "|" + r.conn2];

                    groups.Add(new int[2] { row, column });
                }
                
            }
        }

        return groups;
    }

    private static Dictionary<string, List<int[]>> LoadRuleGrammar()
    {
        Dictionary<string, List<int[]>> RuleGroups = new Dictionary<string, List<int[]>>();

        if (jsonCont.ruleGroupsData != null && jsonCont.ruleGroupsData.Count > 0)
        {
            foreach (RuleGroupsData rg in jsonCont.ruleGroupsData)
            {
                if (!RuleGroups.ContainsKey(rg.RuleGroupName))
                {
                    RuleGroups.Add(rg.RuleGroupName, BuildRuleSet(rg));

                    if (!GlobalReferences.ActiveRulesGrammer.Contains(rg.RuleGroupName))
                    {
                        GlobalReferences.ActiveRulesGrammer.Add(rg.RuleGroupName);
                    }
                }
            }

            return RuleGroups;
        }
        else
        {
            foreach (RuleData r in jsonCont.ruleData)
            {
                string _rg = BuildRuleGrammar(r);
                RuleGroupsData rg = new RuleGroupsData();
                rg.RuleGroupName = _rg;
                rg.RuleGrammar = new string[1] { _rg };

                if (!RuleGroups.ContainsKey(_rg))
                {
                    RuleGroups.Add(_rg, BuildRuleSet(rg));

                    if (!GlobalReferences.ActiveRulesGrammer.Contains(rg.RuleGroupName))
                    {
                        GlobalReferences.ActiveRulesGrammer.Add(rg.RuleGroupName);
                    }
                }
            }

            return RuleGroups;
        }
    }

    private static bool[,] BuildRuleMatrix()
    {
        int i = 0;
        foreach (PartData p in jsonCont.partData)
        {
            foreach (ConnectionData c in p.connections)
            {
                ruleLookup[p.name + "|" + c.id] = i;
                ++i;
            }
        }

        return new bool[i, i];
    }

    private static string BuildRuleGrammar(RuleData r)
    {
            string grammerA = "";
            string grammerB = "";
            string grammer = "";

            foreach (PartData p in jsonCont.partData)
            {
                foreach (ConnectionData con in p.connections)
                {
                    if (con.id == r.conn1 && p.name == r.part1)
                    {
                        grammerA = con.type;
                    }

                    if (con.id == r.conn2 && p.name == r.part2)
                    {
                        grammerB = con.type;
                    }
                }
            }

            grammer = grammerA + ">" + grammerB;

        return grammer;
        
    }

    private static GameObject BuildPart(PartData p, int templateId)
    {
        GameObject tempGo = null;
        tempGo = new GameObject();

        Part tempPart = tempGo.AddComponent<Part>();

        GlobalReferences.PartProbs.Add(p.Probability);
        GlobalReferences.TypeFilter.Add(templateId, p.Probability);
        //GlobalReferences.TypeFilter.Add(p.name);
        tempGo.name = p.name;

        List<Mesh> colliderMeshes = BuildColliders(p.collider);
        Mesh meshGeometry = BuildObjMesh(p.geometry.vertices, p.geometry.faces);

        Vector3 offset = -meshGeometry.bounds.center;

        List<Connection> cons = new List<Connection>();

        foreach (ConnectionData conD in p.connections)
        {
            Vector3 origin = new Vector3(conD.plane.origin[0], conD.plane.origin[1], conD.plane.origin[2]);
            Vector3 xAxis = new Vector3(conD.plane.xaxis[0], conD.plane.xaxis[1], conD.plane.xaxis[2]);
            Vector3 yAxis = new Vector3(conD.plane.yaxis[0], conD.plane.yaxis[1], conD.plane.yaxis[2]);

            Connection con = new Connection(new AlignPlane(origin + offset, xAxis, yAxis,
                                            tempGo.transform), conD.type, conD.part, conD.id, ruleLookup[p.name + "|" + conD.id]);
            con.GenerateRulesTable(GlobalReferences.Rules);
            cons.Add(con);
        }

        List<Vector3> newVertices = new List<Vector3>();
        foreach (Vector3 vec in meshGeometry.vertices)
        {
            newVertices.Add(vec + offset);
        }
        meshGeometry.SetVertices(newVertices);
        meshGeometry.RecalculateNormals();
        meshGeometry.RecalculateBounds();

        foreach (Mesh colMesh in colliderMeshes)
        {
            newVertices.Clear();
            foreach (Vector3 vec in colMesh.vertices)
                newVertices.Add(vec + offset);

            colMesh.SetVertices(newVertices);
            colMesh.RecalculateNormals();
            colMesh.RecalculateBounds();
        }

        if (colliderMeshes.Count > 0)
            tempPart.Initialize(p.name, cons, -1, templateId, offset, meshGeometry , p.SpawnNumber, colliderMeshes);
        else
            tempPart.Initialize(p.name, cons, -1, templateId, offset, meshGeometry, p.SpawnNumber);
        
        tempGo.SetActive(false);
        tempGo.layer = 8;

        return tempGo;
    }

    private static Mesh BuildObjMesh(List<float[]> vertices, List<int[]> faces)
    {
        Mesh m = new Mesh();

        List<Vector3> mVerts = new List<Vector3>();
        List<int> mFaces = new List<int>();

        foreach (float[] vertexData in vertices)
            mVerts.Add(new Vector3(vertexData[0], vertexData[1], vertexData[2]));

        foreach (int[] faceData in faces)
        {
            mFaces.Add(faceData[0]);
            mFaces.Add(faceData[1]);
            mFaces.Add(faceData[2]);

            if (faceData.Length == 4)
            {
                mFaces.Add(faceData[0]);
                mFaces.Add(faceData[2]);
                mFaces.Add(faceData[3]);
            }
        }


        m.SetVertices(mVerts);
        m.triangles = mFaces.ToArray();
        m.RecalculateBounds();
        m.RecalculateNormals();
        m.RecalculateTangents();

        return m;
    }

    private static List<Mesh> BuildColliders(ColliderData colliderData)
    {

        List<Mesh> colliderMeshes = new List<Mesh>();

        foreach (PartGeometry geo in colliderData.geometry)
        {
            colliderMeshes.Add(BuildObjMesh(geo.vertices, geo.faces));
        }

        return colliderMeshes;
    }

    private static JsonContainer LoadFromWasp(string path)
    {
        JsonContainer partsAndRules = null;

        using (FileStream stream = new FileStream(path, FileMode.Open))
        {
            using (StreamReader sr = new StreamReader(stream))
            {
                partsAndRules = JsonConvert.DeserializeObject<JsonContainer>(sr.ReadToEnd());
            }
        }

        return partsAndRules;
    }

    private static JsonContainer LoadFromWaspFile(byte[] data)
    {

        JsonContainer partsAndRules = null;

        using (MemoryStream stream = new MemoryStream(data))
        {
            using (StreamReader sr = new StreamReader(stream))
            {
                partsAndRules = JsonConvert.DeserializeObject<JsonContainer>(sr.ReadToEnd());
            }
        }

        return partsAndRules;
    }
    #endregion
}
