/*
Project DisCo (Discrete Choreography) (GPL) initiated by Jan Philipp Drude

This file is part of Project DisCo.

Copyright (c) 2019, Jan Philipp Drude <jpdrude@gmail.com>

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
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/*
 * System initialization
 * 
 * Spawns Parts
 * Hold Game Area Info
 * 
 * Needs reworking
 */

public class PartsHolder
{
  //variables for spawn area initialization
    #region

    float minX;
    public float MinX { get { return minX; } }

    float maxX;
    public float MaxX { get { return maxX; } }

    float minY;
    public float MinY { get { return minY; } }

    float maxY;
    public float MaxY { get { return maxY; } }

    float minZ;
    public float MinZ { get { return minZ; } }

    float maxZ;
    public float MaxZ { get { return maxZ; } }

    GameObject floor = null;
    public GameObject Floor { get { return floor; } }
    #endregion


    //properties
    #region
    static int numParts = 100;
    public static int NumParts
    {
        get { return numParts; }
    }

    static PartsHolder holder;
    public static PartsHolder Holder { get { return holder; } }
    #endregion


    #region
    public PartsHolder(int _numParts, string loadPath, byte[] loadData = null)
    {
        holder = this;

        InitializeGameArea initArea = new InitializeGameArea();

        Environment environment;

        if (loadPath != null)
            environment = ImplementWasp.Initialize(loadPath);
        else
            environment = ImplementWasp.Initialize(null, loadData);

        minX = environment.GameArea.minX;
        maxX = environment.GameArea.maxX;

        minY = environment.GameArea.minZ;
        maxY = environment.GameArea.maxZ;

        minZ = environment.GameArea.minY;
        maxZ = environment.GameArea.maxY;

        TIExportField.Resolution = environment.FieldResolution;

        GlobalReferences.TemplateParts = ImplementWasp.Load();

        Debug.Log("Template Parts:" + GlobalReferences.TemplateParts.Count + " Parts were loaded");

        floor = GameObject.Find("Floor");
        floor.transform.position = new Vector3(0, minY, 0);
        floor.SetActive(environment.GroundPlane);

        GameObject areaProxy = GameObject.Find("GameAreaProxy");
        if (areaProxy != null)
            areaProxy.GetComponent<GameAreaProxy>().Initialize(new Region(minX, maxX, minY, maxY, minZ, maxZ));

        initArea.Initialize(minX, maxX, minY, maxY, minZ, maxZ, ConnectionScanning.ConnectionThreshold * 1.5f, ComputeColGridSize());

        numParts = _numParts;

        SpawnMultiple(NumParts);
    }
    #endregion


    //methods
    #region
    public void SpawnMultiple(int num)
    {
        if (PlacementReferences.InfiniteParts)
        {
            for (int i = 0; i < num; ++i)
            {
                GameObject newPart = SpawnPart();
                newPart.SetActive(true);
                newPart.GetComponent<Part>().Respawn = true;
            }
        }
        else
        {
            foreach (GameObject go in GlobalReferences.TemplateParts)
            {
                for (int i = 0; i < go.GetComponent<Part>().SpawnNumber; ++i)
                {
                    GameObject newPart = SpawnPart(go.GetComponent<Part>().TemplateID);
                    newPart.SetActive(true);
                    newPart.GetComponent<Part>().Respawn = true;
                }
            }
        }
    }


    public GameObject SpawnPart(int templateID = -1)
    {
        GameObject go = null;
        if (!BoltNetwork.IsRunning)
            go = SpawnGhostPart(templateID);
        else
            go = SpawnNetworkPart(templateID);

        GlobalReferences.FreeParts.Add(go);

        return go;
    }

    public GameObject SpawnGhostPart(int templateID = -1, Vector3? position = null, Quaternion? rotation = null)
    {
        int chose = GlobalReferences.PartProb;
        if (templateID != -1)
        {
            chose = templateID;
        }

        Vector3 pos;
        Quaternion rot;

        if (position == null)
            pos = new Vector3(UnityEngine.Random.Range(minX, maxX), UnityEngine.Random.Range(minY, maxY), UnityEngine.Random.Range(minZ, maxZ));
        else
            pos = (Vector3)position;

        if (rotation == null)
            rot = Quaternion.Euler(new Vector3(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360)));
        else
            rot = (Quaternion)rotation;

        GameObject go = null;


        go = MonoBehaviour.Instantiate(GlobalReferences.TemplateParts[chose], pos, rot);
        ResetPart(go, GlobalReferences.TemplateParts[chose], -1);

        GlobalReferences.Parts.Add(go);

        go.SetActive(true);

        return go;
    }

    public GameObject SpawnNetworkPart(int templateID = -1)
    {
        int chose = GlobalReferences.PartProb;
        if (templateID != -1)
        {
            chose = templateID;
        }

        Vector3 pos = new Vector3(UnityEngine.Random.Range(minX, maxX), UnityEngine.Random.Range(minY, maxY), UnityEngine.Random.Range(minZ, maxZ));
        Quaternion rot = Quaternion.Euler(new Vector3(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360)));

        PartToken token = new PartToken();
        token.TemplateID = chose;

        return BoltNetwork.Instantiate(BoltPrefabs.AbstractBlock, token, pos, rot);
    }


    public static void ResetPart(GameObject go, GameObject templateGo, int id)
    {
        List<Connection> tempCons = new List<Connection>();
        foreach (Connection con in templateGo.GetComponent<Part>().Connections)
        {
            Connection c = (Connection)con.Clone();
            tempCons.Add(c);
        }

        Part templatePart = templateGo.GetComponent<Part>();
        go.GetComponent<Part>().Initialize(templatePart.Name, tempCons, id, templatePart.TemplateID, templatePart.PartOffset);
    }

    static float ComputeColGridSize()
    {
        float maxSize = 0f;

        foreach (GameObject go in GlobalReferences.TemplateParts)
        {
            Mesh m = go.GetComponent<MeshFilter>().mesh;
            float size = Vector3.Distance(m.bounds.min, m.bounds.max);

            if (size > maxSize)
            {
                maxSize = size;
            }
        }

        return maxSize * 1.2f;
    }

    IEnumerator Activate()
    {   
        for (int i = 0; i < GlobalReferences.Parts.Count; ++i)
        {
            GlobalReferences.Parts[i].SetActive(true);
            yield return new WaitForSeconds(0.001f);
        }

        GlobalReferences.ChangeAffectedNumber(PlacementReferences.AffectedParts);
    }
    #endregion
}
