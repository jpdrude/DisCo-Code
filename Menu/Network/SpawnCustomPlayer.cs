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
using UnityEngine.UI;

/*
 * Instantiates Custom Player from Wasp Player Setup
 */

public class SpawnCustomPlayer : MonoBehaviour
{
    public Player player { get; set; }

    [SerializeField]
    Text buttonName;

    [SerializeField]
    Camera cam;

    [SerializeField]
    GameObject vrController;

    [SerializeField]
    GameObject fpsController;

    [SerializeField]
    GameObject customPlayerMenu;

    private void Start()
    {
        buttonName.text = player.playerName;
    }

    public void InstantiatePlayer()
    {
        cam.gameObject.SetActive(false);

        ControllerReferences.MultiPlayerRole = player.playerName;
        ControllerReferences.MultiPlayer = player;

        if (player.VrFps == 0)
            NetworkPlayerMenu.InitializeController(vrController, new Vector3(0, 0, 0), player.ToolsetSettings, player.PlacementSettings, player.SaveLoadSettings);
        else
            NetworkPlayerMenu.InitializeController(fpsController, NetworkPlayerMenu.GetSpawnPosition(), player.ToolsetSettings, player.PlacementSettings, player.SaveLoadSettings);


        customPlayerMenu.SetActive(false);
    }


}
