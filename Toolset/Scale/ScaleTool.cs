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
using TMPro;

/*
 * Tool: Scale
 * 
 * overrides tool to change value directly without using ToolItem
 * Changes Controller Scale
 */

public class ScaleTool : Tool
{
    [SerializeField]
    GameObject controller;

    static List<GameObject> keepLarge = new List<GameObject>();
    public static List<GameObject> KeepLarge { get { return keepLarge; } }

    [SerializeField]
    float scaleSpeed = 0.2f;

    [SerializeField]
    TextMeshPro textMesh;

    [SerializeField]
    GameObject scaleOrigin = null;

    [SerializeField]
    Camera cam;

    float baseClipping;

    float scale = 1f;

    float baseYPos = 5;

    public override void Initialize(Toolset _toolSet)
    {
        base.Initialize(_toolSet);
        baseYPos = controller.transform.position.y;
        baseClipping = cam.nearClipPlane;
    }
    public override void ActivateItem()
    {
        scale = 1;
        SetScale();

        Vector3 tempPos = controller.transform.position;
        tempPos.y = baseYPos;
        controller.GetComponent<FirstPersonController>().Teleport(tempPos);
    }

    public override void LeftPressed()
    {
        float tempScale = scale - Time.deltaTime * scaleSpeed;

        if (scale > 0)
        {
            scale = tempScale;
            SetScale();
        }
    }

    public override void RightPressed()
    {
        scale += Time.deltaTime * scaleSpeed;
        SetScale();
    }

    void SetScale()
    {
        Vector3 headPos = Vector3.zero;
        Vector3 newHeadPos = Vector3.zero;

        if (scaleOrigin != null)
            headPos = scaleOrigin.transform.position;

        controller.transform.localScale = new Vector3(scale, scale, scale);

        if (scaleOrigin != null)
            newHeadPos = scaleOrigin.transform.position;

        Vector3 headDif = (headPos - newHeadPos);
        headDif.y = 0;

        controller.transform.position += headDif;

        foreach (GameObject go in KeepLarge)
        {
            if (go != null)
                go.transform.localScale = Vector3.one / scale;
        }
        textMesh.text = ((int)(scale * 100)).ToString() + "%";

        cam.nearClipPlane = baseClipping * scale;
    }

    public override void Left() { return; }

    public override void Right() { return; }
}
