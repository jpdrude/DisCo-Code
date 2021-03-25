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
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;

/*
 * Version of InitializeLauncher for Lobby
 * Purely for visual purposes
 */

public class InitializeLobby : MonoBehaviour
{

    [SerializeField]
    GameObject Floor;

    string LoadLoc;
    string SaveLoc;

    int VrFps = 0;

    float SnapThresh = 0.1f;

    float AngleTighteningFactor = 1.15f;

    int NumParts = 100;

    string colorScheme = "Candy";

    float ConRes = 0.1f;

    public void Initialize()
    {
        LoadJson(Application.dataPath + "/Resources/config.json");

        MaterialHolder.MatSet = colorScheme;

        ConnectionScanning.ConnectionThreshold = SnapThresh;
        ConnectionScanning.AngleTighteningFactor = AngleTighteningFactor;
        SaveLoad.SavePath = SaveLoc;
        RenderSettings.skybox = Resources.Load<Material>("Materials/" + colorScheme + "/Sky/Sky");

        GlobalReferences.PartSpawner = new PartsHolder(NumParts, LoadLoc);
    }

    private void LoadJson(string path)
    {
        JsonInitContainer container = null;

        DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(JsonInitContainer));

        using (FileStream stream = new FileStream(path, FileMode.Open))
        {
            container = serializer.ReadObject(stream) as JsonInitContainer;
        }

        if (container != null)
        {
            SaveLoc = container.saveLoc;
            LoadLoc = container.loadLoc;

            VrFps = container.vrFps;

            SnapThresh = container.snapThreshold;
            AngleTighteningFactor = container.angleTightFactor;

            NumParts = container.numberOfParts;

            colorScheme = container.colScheme;
        }
    }
}
