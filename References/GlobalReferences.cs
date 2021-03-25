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
using TMPro;
using System.Security.Cryptography;

/*
 * Contains a bulk of References
 * Everything concerned with Parts Containers
 * 
 * Methods to work with parts
 * 
 * Reset Game
 * 
 * Needs reworking
 */

public class GlobalReferences : MonoBehaviour
{
    //private variables
    #region
    [SerializeField]
    float _forceScale = 1;
    #endregion


    //properties
    #region    
    private static List<GameObject> parts = new List<GameObject>();
    public static List<GameObject> Parts
    {
        get { return parts; }
        set { parts = value; }
    }

    private static List<GameObject> freeParts = new List<GameObject>();
    public static List<GameObject> FreeParts
    {
        get { return freeParts; }
        set { freeParts = value; }
    }

    private static Dictionary<int, GameObject> frozenParts = new Dictionary<int, GameObject>();
    public static Dictionary<int, GameObject> FrozenParts
    {
        get { return frozenParts; }
        set { frozenParts = value; }
    }

    private static List<GameObject> affectedParts = new List<GameObject>();
    public static List<GameObject> AffectedParts
    {
        get { return affectedParts; }
    }

    private static List<GameObject> additionalGeometry = new List<GameObject>();
    public static List<GameObject> AdditionalGeometry
    {
        get { return additionalGeometry; }
        set { additionalGeometry = value; }
    }

    private static List<GameObject> blueprintGeometry = new List<GameObject>();
    public static List<GameObject> BlueprintGeometry
    {
        get { return blueprintGeometry; }
        set { blueprintGeometry = value; }
    }

    private static PartsHolder partSpawner = null;
    public static PartsHolder PartSpawner
    {
        get { return partSpawner; }
        set { partSpawner = value; }
    }

    
    private static int numOfParts = 0;
    
    public static int NumOfParts
    {
        get { return numOfParts; }
        set { numOfParts = value; }
    }

    static Dictionary<int, int> partIDLedger = new Dictionary<int, int>();
    public static Dictionary<int, int> PartIDLedger 
    { 
        get { return partIDLedger; } 
    }

    private static Vector3 controllerPos;
    public static Vector3 ControllerPos
    {
        get { return controllerPos; }
    }

    private static Vector3 controllerVel;
    public static Vector3 ControllerVel
    {
        get { return controllerVel; }
    }

    private static Dictionary<string, List<int[]>> ruleGroups = new Dictionary<string, List<int[]>>();
    public static Dictionary<string, List<int[]>> RuleGroups
    {
        get { return ruleGroups; }
        set
        {
            ruleGroups = value;

            for (int i = 0; i < GlobalReferences.RuleMatrix.GetLength(0); ++i)
            {
                for (int j = 0; j < GlobalReferences.RuleMatrix.GetLength(1); ++j)
                {
                    GlobalReferences.RuleMatrix[i, j] = false;
                }
            }

            foreach (List<int[]> mIdxs in ruleGroups.Values)
            {
                foreach (int[] mIdx in mIdxs)
                {
                    GlobalReferences.RuleMatrix[mIdx[0], mIdx[1]] = true;
                }
            }

        }
    }

    private static Dictionary<int, float> typeFilter = new Dictionary<int, float>();
    public static Dictionary<int, float> TypeFilter
    {
        get { return typeFilter; }
        set { typeFilter = value; }
    }

    private static List<float> partProbs = new List<float>();
    public static List<float> PartProbs
    {
        get { return partProbs; }
        set { partProbs = value; }
    }

    public static int PartProb
    {
        get
        {
            float max = 0;
            foreach (float f in typeFilter.Values)
            {
                max += f;
            }

            float rand = Random.Range(0.000f, max);
            float counting = 0f;
            foreach (KeyValuePair<int, float> filter in typeFilter)
            {
                counting = counting + filter.Value;
                if (rand <= counting)
                {
                    return filter.Key;
                }
            }

            return -1;
        }
    }

    private static List<GameObject> templateParts = new List<GameObject>();
    public static List<GameObject> TemplateParts
    {
        get { return templateParts; }
        set { templateParts = value; }
    }

    private static HashSet<string> activeRulesGrammer = new HashSet<string>();
    public static HashSet<string> ActiveRulesGrammer
    {
        get { return activeRulesGrammer; }
        set { activeRulesGrammer = value; }
    }
    
    private static List<Rule> rules = new List<Rule>();
    public static List<Rule> Rules
    {
        get { return rules; }
        set { rules = value; }
    }

    private static bool[,] ruleMatrix;
    public static bool[,] RuleMatrix
    {
        get { return ruleMatrix; }
        set { ruleMatrix = value; }
    }


    private static float driftApartFac = 0f;
    public static float DriftApartFac
    {
        get { return driftApartFac; }
        set { driftApartFac = value; }
    }

    private static Vector3 centerOfMass;
    public static Vector3 CenterOfMass
    {
        get { return centerOfMass; }
    }

    private static float forceScale;
    public static float ForceScale
    {
        get { return forceScale; }
    }

    #endregion


    //MonobBehaviour methods
    #region
    private void Awake()
    {
        reset();

        forceScale = _forceScale;

        var initialize = GetComponent<InitializeLauncher>();
        if (initialize != null && !BoltNetwork.IsRunning)
            initialize.Initialize();

        var lobby = GetComponent<InitializeLobby>();
        if (lobby != null)
            lobby.Initialize();
    }

    private void Update()
    {
        try
        {
            controllerVel = (ControllerReferences.ControllerTarget.transform.position - controllerPos) / Time.deltaTime;
            controllerPos = ControllerReferences.ControllerTarget.transform.position;

            centerOfMass = Vector3.zero;
            int i = 0;
            foreach (GameObject go in affectedParts)
            {
                if (go != null)
                {
                    centerOfMass += go.transform.position;
                    ++i;
                }
            }
            centerOfMass = centerOfMass / i;
        }
        catch { }
    }
    #endregion


    //static methods
    #region
    public static int TemplateIDFromName(string name)
    {
        for (int i = 0; i < templateParts.Count; ++i)
        {
            if (templateParts[i].GetComponent<Part>().name == name)
            {
                return i;
            }
        }

        return -1;
    }

    public static List<GameObject> ChangeAffectedNumber(int num)
    {
        CompareObjectDistance compDist = new CompareObjectDistance();
        Comparer<GameObject> defComp = compDist;

        if (num - 1 > affectedParts.Count)
        {
            freeParts.Sort(compDist);

            List<GameObject> transition = new List<GameObject>();
            foreach (GameObject go in freeParts)
            {
                if (num - 1 >= affectedParts.Count)
                {
                    if (typeFilter.ContainsKey(go.GetComponent<Part>().TemplateID))//TypeFilter.Contains(go.GetComponent<Part>().Name))
                    {
                        AffectPart(go);
                        transition.Add(go);
                    }
                }
                else
                {
                    break;
                }
            }
            foreach (GameObject go in transition)
            {
                freeParts.Remove(go);
            }
        }
        else
        {
            affectedParts.Sort(compDist);
            
            for(int i = affectedParts.Count - 1; i >= 0; --i)
            {
                if (num < affectedParts.Count)
                {
                    GameObject go = affectedParts[i];
                    go.GetComponent<ConnectionScanning>().enabled = false;
                    go.GetComponent<PartBehaviour>().enabled = false;
                    go.GetComponent<MeshRenderer>().material = MaterialHolder.UnaffectedMat;
                    go.GetComponent<ConstantForce>().force = Vector3.zero;
                    affectedParts.Remove(go);
                    FreeParts.Add(go);
                }
            }
        }

        return affectedParts;
    }

    public static int GetNextID()
    {
        ++numOfParts;
        return numOfParts - 1;
    }

    public static void AffectPart(GameObject go)
    {
        affectedParts.Add(go);
        go.GetComponent<ConnectionScanning>().enabled = true;
        go.GetComponent<PartBehaviour>().enabled = true;
        go.GetComponent<MeshRenderer>().material = MaterialHolder.AffectedMat;
    }

    public static void ClearAffectedList()
    {
        for (int i = AffectedParts.Count - 1; i >= 0; --i)
        {
            GameObject go = AffectedParts[i];

            if (go == null)
            {
                AffectedParts.RemoveAt(i);
                continue;
            }
            
            go.GetComponent<ConnectionScanning>().enabled = false;
            go.GetComponent<MeshRenderer>().material = MaterialHolder.UnaffectedMat;
            go.GetComponent<PartBehaviour>().enabled = false;
            go.GetComponent<ConstantForce>().force = Vector3.zero;

        }
        FreeParts.AddRange(AffectedParts);
        AffectedParts.Clear();
    }

    public static void RebuildIndices()
    {
        NumOfParts = 0;

        List<GameObject> tempFrozen = new List<GameObject>();
        foreach (GameObject go in FrozenParts.Values)
        {
            tempFrozen.Add(go);
        }

        FrozenParts.Clear();

        Dictionary<int, int> IdChanges = new Dictionary<int, int>();

        foreach(GameObject go in tempFrozen)
        {
            IdChanges.Add((int)go.GetComponent<Part>().ID, GetNextID());
        }
         foreach (GameObject go in tempFrozen)
        {
            go.GetComponent<Part>().ChangeIDsFromDic(IdChanges);
            FrozenParts.Add((int)go.GetComponent<Part>().ID, go);
        }
    }

    public static void DeletePart(int id)
    {
        GameObject go = frozenParts[id];
        int? parentID = go.GetComponent<Part>().Parent;
        if (parentID != null && frozenParts.ContainsKey((int)parentID))
        {
            List<int> _children = frozenParts[(int)parentID].GetComponent<Part>().Children;
            for (int i = _children.Count - 1; i >= 0; --i)
            {
                if (_children[i] == id)
                {
                    Part parentP = frozenParts[(int)parentID].GetComponent<Part>();
                    Part p = go.GetComponent<Part>();
                    parentP.Children.RemoveAt(i);
                    if (parentP.ChildCons.Count > i)
                    {
                        parentP.ChildCons.RemoveAt(i);
                    }
                    foreach (int j in go.GetComponent<Part>().ChildCons)
                    {
                        Part parentPart = frozenParts[(int)parentID].GetComponent<Part>();
                        if (parentPart.Connections.Count > j && j != -1)
                        {
                            parentPart.SetActive(parentPart.Connections[j]);
                        }
                    }
                    if (p.ChildCons.Count > i)
                    {
                        try
                        {
                            ConnectionVoxelContainer.StoreConnection(frozenParts[p.Children[i]].GetComponent<Part>().Connections[p.ChildCons[i]]);
                        }
                        catch
                        {
                            Debug.Log("Connection not found");
                        }
                        //ConnectionVoxelContainer.StoreConnection(p.ChildCons[i]);
                    }
                }
            }
        }

        Part part = go.GetComponent<Part>();

        int[] children = part.Children.ToArray();

        for (int i = 0; i < children.Length; ++i)
        {
            int child = children[i];

            if (frozenParts.ContainsKey(child))
            {
                Part childPart = frozenParts[child].GetComponent<Part>();
                childPart.Parent = -1;
                childPart.ParentCon = -1;
                childPart.SetActive(childPart.ConToParent);
                childPart.ConToParent = -1;
            }

            int parent = part.Parent;

            if (parent != -1 && frozenParts.ContainsKey(parent))
            {
                Part parentPart = frozenParts[parent].GetComponent<Part>();
                if (go.GetComponent<Part>().ParentCon != -1)
                {
                    parentPart.SetActive(part.ParentCon);
                    for (int j = 0; j < parentPart.Children.Count; ++j)
                    {
                        if (parentPart.Children[j] == part.ID)
                        {
                            parentPart.Children.RemoveAt(j);
                            parentPart.ChildCons.RemoveAt(j);
                            break;
                        }
                    }
                }
            }
        }
        Destroy(go);
    }

    static void reset()
    {
        ControllerReferences.ControllerTarget = null;

        parts = new List<GameObject>();

        freeParts = new List<GameObject>();

        frozenParts = new Dictionary<int, GameObject>();

        affectedParts = new List<GameObject>();

        additionalGeometry = new List<GameObject>();

        //typeFilter = new List<string>();
        typeFilter = new Dictionary<int, float>();

        numOfParts = 0;

        partIDLedger.Clear();

        templateParts = new List<GameObject>();

        partProbs = new List<float>();

        activeRulesGrammer = new HashSet<string>();

        rules = new List<Rule>();

        PlacementReferences.PlacementType = PlacementReferences.PlaceChoreo.Place;

        NetworkPartEventsListener.PartFilterTIs.Clear();

        NetworkPartEventsListener.RuleFilterTIs.Clear();

        NetworkPartSpawner.LoadData.data.Clear();

        NetworkPartSpawner.Data.Clear();

        FirstPersonController.Fly(false);

        PlacementReferences.Aiming = false;
    }
    #endregion
}

