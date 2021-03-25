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

/*
 * Class to perform Simulation/Export
 */

public class SimulateStatics
{
    static bool simulationInProgress = false;
    public static bool SimulationInProgress 
    { 
        get { return simulationInProgress; }
        set { simulationInProgress = value; }
    }

    static bool simulationDone = false;
    public static bool SimulationDone 
    { 
        get { return simulationDone; } 
        set { simulationDone = value; }
    }

    static List<Rigidbody> rbs = new List<Rigidbody>();

    static List<SimulationProcessHandler> handlers = new List<SimulationProcessHandler>();
    static int handlerCallBack = 0;

    static int expCount = 0;


    static string tempPath = Application.dataPath + "/Resources/temp.json";
    static string expTempPath = Application.dataPath + "/Resources/exptemp.json";

    static int countDigits = 0;

    static string folder;

    static Dictionary<int, Vector3> savePositions = new Dictionary<int, Vector3>();
    static Dictionary<int, Quaternion> saveRotations = new Dictionary<int, Quaternion>();


    public static void ParentAndSimulate(TIExportAR exportItem = null, bool export = false)
    {
        //SaveTemp();
        savePositions.Clear();
        saveRotations.Clear();

        rbs = new List<Rigidbody>();

        foreach (GameObject go in GlobalReferences.FrozenParts.Values)
        {
            Part part = go.GetComponent<Part>();
            int parentId = part.Parent;

            savePositions.Add(part.ID, go.transform.position);
            saveRotations.Add(part.ID, go.transform.rotation);

            MonoBehaviour.Destroy(go.GetComponent<ConstantForce>());

            if (parentId != -1 && GlobalReferences.FrozenParts.ContainsKey(parentId))
            {
                go.transform.parent = GlobalReferences.FrozenParts[parentId].transform;
            }
            else
            {
                Rigidbody rb = go.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rbs.Add(rb);
                }
                else
                {
                    rbs.Add(go.AddComponent<Rigidbody>());
                }

                if (export)
                {
                    SimulationProcessHandler handler = go.AddComponent<SimulationProcessHandler>();
                    handlers.Add(handler);
                    handler.ExportItem = exportItem;
                    handler.Sim = export;
                    handler.SimulationTerminated += new SimulationProcessHandler.SimulationHandler(AfterSimulation);
                }
            }
        }
        foreach (Rigidbody rb in rbs)
        {
            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.None;
            rb.ResetCenterOfMass();
        }

        ChangeColliders(false);

    }

    public static void ExportToAR(TIExportAR exportItem)
    {
        folder = "ar-export_" + SaveLoad.BuildDateString() + "/";
        countDigits = GlobalReferences.FrozenParts.Count.ToString().Length;

        Export(exportItem);

        simulationInProgress = true;

        ControllerReferences.ControllerTarget.SetActive(false);

        expCount = GlobalReferences.FrozenParts.Count;
    }

    static void Export(TIExportAR exportItem)
    {
        if (GlobalReferences.FrozenParts.Count > 1)
        {
            int k = 0;

            foreach (int kk in GlobalReferences.FrozenParts.Keys)
            {
                if (kk > k)
                {
                    k = kk;
                }
            }

            GlobalReferences.DeletePart(k);

            ParentAndSimulate(exportItem, true);

            --expCount;
        } 
        else
        {
            exportItem.ResetTool();

            simulationInProgress = false;

            int k = 0;
            foreach (int i in GlobalReferences.FrozenParts.Keys)
            {
                k = i;
            }

            GlobalReferences.DeletePart(k);

            expCount = 0;
            AssemblyIO.Load(expTempPath);

            ControllerReferences.ControllerTarget.SetActive(true);
        }
    }

    public static JsonAssembly SaveTemp()
    {
        return AssemblyIO.Save(GlobalReferences.FrozenParts, 1, tempPath);
    }

    public static JsonAssembly SaveExportTemp()
    {
        return AssemblyIO.Save(GlobalReferences.FrozenParts, 1, expTempPath);
    }

    public static void ResetSimulation(JsonAssembly assembly = null)
    {
        rbs.Clear();

        ConnectionVoxelContainer.RemoveAllConnections();
        CollisionVoxelContainer.RemoveAllGos();

        foreach (KeyValuePair<int, Vector3> pos in savePositions)
        {
            GameObject go = GlobalReferences.FrozenParts[pos.Key];
            go.transform.parent = null;
            go.transform.position = pos.Value;
            go.transform.rotation = saveRotations[pos.Key];

            Part part = go.GetComponent<Part>();

            CollisionVoxelContainer.StoreGameObject(go);

            foreach (int i in part.ActiveConnections)
            {
                ConnectionVoxelContainer.StoreConnection(part.Connections[i]);
            }
        }

        savePositions.Clear();
        saveRotations.Clear();

        simulationDone = false;
    }
    public static void AfterSimulation(TIExportAR exportItem = null, bool export = false)
    {
        ++handlerCallBack;
        if (handlerCallBack >= handlers.Count)
        {

            if (!export)
            {
                ConnectionVoxelContainer.RemoveAllConnections();
                CollisionVoxelContainer.RemoveAllGos();

                foreach (GameObject go in GlobalReferences.FrozenParts.Values)
                {
                    go.transform.parent = null;
                    Part part = go.GetComponent<Part>();

                    CollisionVoxelContainer.StoreGameObject(go);

                    foreach (int i in part.ActiveConnections)
                    {
                        ConnectionVoxelContainer.StoreConnection(part.Connections[i]);
                    }
                }

                for (int i = rbs.Count -1; i >= 0; --i)
                {
                    MonoBehaviour.Destroy(rbs[i]);
                }

                simulationInProgress = false;
                simulationDone = false;
            }
            else
            {
                string c = expCount.ToString();
                while (c.Length < countDigits)
                {
                    c = "0" + c;
                }
                SaveLoad.SaveGame(c + "_", true, folder);
            }


            for (int i = handlers.Count - 1; i >= 0; --i)
            {
                MonoBehaviour.Destroy(handlers[i]);
            }
            handlers.Clear();
            handlerCallBack = 0;

            if (export)
            {
                Export(exportItem);
            }
        }
    }

    private static void ChangeColliders(bool onOff)
    {
        foreach (GameObject go in GlobalReferences.FreeParts)
        {
            List<Collider> cols = new List<Collider>();
            cols.AddRange(go.GetComponents<MeshCollider>());
            cols.AddRange(go.GetComponents<BoxCollider>());
            cols.AddRange(go.GetComponents<SphereCollider>());
            cols.AddRange(go.GetComponents<CapsuleCollider>());

            foreach (Collider col in cols)
            {
                col.enabled = onOff;
            }
        }
    }

    //Enumerations
    #region
    public enum Simulations
    {
        Simulate,
        ARexport
    }
    #endregion
}
