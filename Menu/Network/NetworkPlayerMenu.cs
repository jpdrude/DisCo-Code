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

using Bolt.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

/*
 * Network Player Menu to spawn Basic Controllers
 * 
 * Holds Multiplayer Player
 * Initializes Controllers and Menus
 */

public class NetworkPlayerMenu : Bolt.GlobalEventListener
{
    [SerializeField]
    Camera cam;

    [SerializeField]
    GameObject VRController;

    [SerializeField]
    GameObject FPSController;

    [SerializeField]
    GameObject menu;

    public static DateTime time;

    public void SpawnFPSController()
    {
        time = DateTime.Now;

        cam.gameObject.SetActive(false);

        InitializeController(FPSController, GetSpawnPosition()).Initialize();

        menu.SetActive(false);
    }

    public void SpawnFPSController(List<bool> toolsetSettings, List<bool> placementSettings, List<bool> saveLoadSettings)
    {
        time = DateTime.Now;

        cam.gameObject.SetActive(false);

        InitializeController(FPSController, GetSpawnPosition(), toolsetSettings, placementSettings, saveLoadSettings);

        menu.SetActive(false);
    }

    /*
    public static void GetTimeSinceSpawnPress(string message)
    {
        Debug.Log("#50 " + message + " after " + DateTime.Now.Subtract(time).TotalSeconds.ToString("F3") + " Seconds");
    }
    */

    public void SpawnVRController()
    {
        cam.gameObject.SetActive(false);

        InitializeController(VRController, new Vector3(0, 0, 0)).Initialize();

        menu.SetActive(false);
    }

    public void SpawnVRController(List<bool> toolsetSettings, List<bool> placementSettings, List<bool> saveLoadSettings)
    {
        cam.gameObject.SetActive(false);

        InitializeController(VRController, new Vector3(1, 1, 0), toolsetSettings, placementSettings, saveLoadSettings);

        menu.SetActive(false);
    }

    /*
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SpawnVRController();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SpawnFPSController();
        }
    }
    */

    public static Vector3 GetSpawnPosition()
    {
        Vector3 pos = Vector3.zero;
        pos.x = UnityEngine.Random.Range(PartsHolder.Holder.MinX, PartsHolder.Holder.MaxX);
        pos.z = UnityEngine.Random.Range(PartsHolder.Holder.MinZ, PartsHolder.Holder.MaxZ);

        if(PartsHolder.Holder.Floor.activeSelf)
        {
            pos.y = PartsHolder.Holder.MinY + 0.8f;
        }
        else
        {
            int layerMask = 1 << 10;

            RaycastHit hit;

            if (Physics.Raycast(pos, new Vector3(0,1,0), out hit, Mathf.Infinity, layerMask))
            {
                pos.y = hit.point.y + 0.8f;
            }
            else if (Physics.Raycast(pos, new Vector3(0, -1, 0), out hit, Mathf.Infinity, layerMask))
            {
                pos.y = hit.point.y + 0.8f;
            }
        }

        return pos;
    }

    static Toolset InitializeController(GameObject preFab, Vector3 pos)
    {
        GameObject controller = Instantiate(preFab, pos, Quaternion.identity);

        ControllerReferences.Toolset = controller.GetComponentInChildren<Toolset>();
        ControllerReferences.ControllerTarget = ControllerReferences.Toolset.ControllerTarget;
        ControllerReferences.Controller = controller;

        if (ControllerReferences.ControllerTarget == null) Debug.LogError("Controller Target not found");
        Time.timeScale = 1;

        if (BoltNetwork.IsRunning)
        {
            GameObject model = BoltNetwork.Instantiate(BoltPrefabs.VR_character, new Vector3(0, 1, 2), Quaternion.identity);
            NetworkModelBinding binding = model.GetComponent<NetworkModelBinding>();

            ControllerReferences.MultiplayerCharacter = model;
            binding.Controller = GameObject.Find("FollowHead").gameObject;

            if (ControllerReferences.MultiPlayerRole != null && ControllerReferences.MultiPlayerRole != "")
                binding.MultiplayerName = ControllerReferences.MultiPlayerName + "\n(" + ControllerReferences.MultiPlayerRole + ")";
            else
                binding.MultiplayerName = ControllerReferences.MultiPlayerName;
        }

        return controller.GetComponentInChildren<Toolset>();
    }

    public static void InitializeController(GameObject preFab, Vector3 pos, List<bool> toolsetSettings, List<bool> placementSettings, List<bool> saveLoadSettings)
    {
        Toolset toolset = InitializeController(preFab, pos);

        toolset.Initialize(toolsetSettings);

        bool firstTool = true;

        foreach (Tool tool in toolset.Tools)
        {
            if (tool.ToolName == "PlacementType")
            {
                tool.Reinitialize(toolset, placementSettings, firstTool, true);
                firstTool = false;
            }
            else if (tool.ToolName == "SaveLoad")
                tool.Reinitialize(toolset, saveLoadSettings, firstTool, false);
        }
    }
}
