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
using Valve.VR.InteractionSystem;
using System;

/*
 * Toolitem: Teleport
 * Active Item
 * 
 * Holds and Moves Teleport Platform
 * Teleports to platform position
 */

public class TITeleport : ToolItem
{
    GameObject target = null;

    [SerializeField]
    GameObject teleportMarker;

    [SerializeField]
    GameObject controller;

    [SerializeField]
    GameObject cam;

    TeleportPlatform platform;
    public TeleportPlatform Platform { get { return platform; } }


    public override void Initialize(Toolset _toolSet)
    {
        base.Initialize(_toolSet);

        target = Instantiate(ToolSet.ControllerTarget, ToolSet.ControllerTarget.transform.position, ToolSet.ControllerTarget.transform.rotation,
                     ToolSet.ControllerTarget.transform.parent);
        target.name = "TeleportTarget";

        GameObject go = Instantiate(teleportMarker, target.transform);
        platform = go.GetComponent<TeleportPlatform>();
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;

        target.SetActive(false);
    }

    public override void ActivateItem()
    {
        target.SetActive(true);
    }

    public override void DeactivateItem()
    {
        base.DeactivateItem();

        target.SetActive(false);
    }

    public override void Click()
    {
        Teleport();
    }

    public override void ScrollUp()
    {
        target.transform.localPosition += Vector3.forward * Time.deltaTime;
    }

    public override void ScrollDown()
    {
        Vector3 newPos = target.transform.localPosition + Vector3.back * Time.deltaTime;
        if (newPos.z > 0.05f)
        {
            target.transform.localPosition = newPos;
        }
    }


    void Teleport()
    {
        
        Vector3 headPos = cam.transform.localPosition * controller.transform.localScale.x;
        headPos.y = 0;

        controller.transform.position = platform.Position - headPos;
    }
}
