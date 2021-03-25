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
using System.ComponentModel;
using UnityEngine;

/*
 * Visualizes the position and size of the game area
 */

public class GameAreaProxy : MonoBehaviour
{
    [SerializeField]
    LineRenderer lineRenderer;

    [SerializeField]
    GameObject area;

    public void Initialize(Region region)
    {
        Vector3[] pts = new Vector3[13];
        pts[0] = new Vector3(region.MinX, region.MaxY, region.MinZ);
        pts[1] = new Vector3(region.MinX, region.MinY, region.MinZ);
        pts[2] = new Vector3(region.MinX, region.MaxY, region.MinZ);

        pts[3] = new Vector3(region.MinX, region.MaxY, region.MaxZ);
        pts[4] = new Vector3(region.MinX, region.MinY, region.MaxZ);
        pts[5] = new Vector3(region.MinX, region.MaxY, region.MaxZ);

        pts[6] = new Vector3(region.MaxX, region.MaxY, region.MaxZ);
        pts[7] = new Vector3(region.MaxX, region.MinY, region.MaxZ);
        pts[8] = new Vector3(region.MaxX, region.MaxY, region.MaxZ);

        pts[9] = new Vector3(region.MaxX, region.MaxY, region.MinZ);
        pts[10] = new Vector3(region.MaxX, region.MinY, region.MinZ);
        pts[11] = new Vector3(region.MaxX, region.MaxY, region.MinZ);

        pts[12] = new Vector3(region.MinX, region.MaxY, region.MinZ);

        lineRenderer.SetPositions(pts);
        lineRenderer.material = MaterialHolder.DisableSphereMat;

        area.transform.position = new Vector3(region.MinX, region.MinY, region.MinZ);
        area.transform.localScale = new Vector3(region.MaxX - region.MinX, 1, region.MaxZ - region.MinZ);
        area.SetActive(true);

        foreach (SpriteRenderer sr in area.GetComponentsInChildren<SpriteRenderer>())
            sr.color = MaterialHolder.AffectedColor;
    }
}
