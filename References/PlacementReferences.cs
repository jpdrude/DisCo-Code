﻿/*
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

/*
 * Holds references for placement Tools
 */

public class PlacementReferences : MonoBehaviour
{
    private static PlaceChoreo placementType = PlaceChoreo.Place;
    public static PlaceChoreo PlacementType
    {
        get { return placementType; }
        set { placementType = value; }
    }

    private static bool infiniteParts = true;
    public static bool InfiniteParts
    {
        get { return infiniteParts; }
        set { infiniteParts = value; }
    }

    private static bool scanning = true;
    public static bool Scanning
    {
        get { return scanning; }
        set { scanning = value; }
    }

    public static int AffectedParts { get; set; }

    private static Mode playMode = Mode.VR;
    public static Mode PlayMode
    {
        get { return playMode; }
        set { playMode = value; }
    }

    private static bool aiming = false;
    public static bool Aiming
    {
        get { return aiming; }
        set { aiming = value; }
    }

    public enum PlaceChoreo
    {
        Place = 0,
        Choreo = 1,
        Shoot = 2,
        PickNChose = 3,
        Delete = 4,
        RecDelete = 5,
        DeleteBall = 6,
        DisableScanning = 7,
        DrawField = 8,
        DeleteField = 9
    }

    public enum Mode
    {
        VR = 0,
        FPS = 1
    }
}
