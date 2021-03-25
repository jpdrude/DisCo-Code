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
using System;

/*
 * Teleport Platform for VR
 * Proxy for Teleport target
 * 
 * Holds Position and Rotation
 * Performs grounding if desired
 */

public class TeleportPlatform : MonoBehaviour
{
    [SerializeField]
    Material inactiveMat;

    bool ground = false;

    Quaternion rotation = Quaternion.identity;
    Vector3 eulerRotation = Vector3.zero;

    public float Angle { get { return eulerRotation.y / (float)Math.PI * 180; } }

    GameObject projected = null;

    RaycastHit hit;
    int layerMaskTeleport;

    Vector3 rayFrom = Vector3.zero;
    Vector3 rayTo = Vector3.zero;

    public Vector3 Position
    {
        get
        {
            if (ground)
                return projected.transform.position;
            else
                return transform.position;
        }
    }

    public void SetGrounded()
    {
        projected = Instantiate(gameObject, transform.parent);

        layerMaskTeleport = 1 << 10;
        layerMaskTeleport = layerMaskTeleport | 1;

        ground = true;

        GetComponent<MeshRenderer>().material = inactiveMat;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = rotation;

        if (ground)
        {
            projected.transform.position = Raycast(Vector3.down);
        }
    }

    public void Activate()
    {
        gameObject.SetActive(true);

        if (ground)
            projected.SetActive(true);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
        projected.SetActive(false);
    }

    public Vector3 Raycast(Vector3 dir)
    {
        rayFrom = transform.position;
        rayTo = transform.TransformDirection(dir);

        if (Physics.Raycast(rayFrom, rayTo, out hit, Mathf.Infinity, layerMaskTeleport))
        {
            return hit.point;
        }

        return transform.position;
    }

    public void Rotate(float rotSpeed)
    {
        eulerRotation.y += Time.deltaTime * rotSpeed;
        rotation = Quaternion.Euler(eulerRotation);
    }
}
