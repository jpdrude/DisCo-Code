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
 * Stores GameObjects according to their spacial position 
 * for collision purposes
 */

public static class CollisionVoxelContainer
{
    private static List<GameObject>[,,] container;
    public static List<GameObject>[,,] Container
    {
        get { return container; }
        set { container = value; }
    }

    private static int offsetX;
    public static int OffsetX
    {
        get { return offsetX; }
    }

    private static int offsetY;
    public static int OffsetY
    {
        get { return offsetY; }
    }

    private static int offsetZ;
    public static int OffsetZ
    {
        get { return offsetZ; }
    }


    private static int x;
    public static int X
    {
        get { return x; }
    }

    private static int y;
    public static int Y
    {
        get { return y; }
    }

    private static int z;
    public static int Z
    {
        get { return z; }
    }



    //initialize
    #region
    public static void Initialize(int _x, int _y, int _z, int offX, int offY, int offZ)
    {
        x = _x;
        y = _y;
        z = _z;

        container = new List<GameObject>[x, y, z];
        offsetX = offX;
        offsetY = offY;
        offsetZ = offZ;

        for (int i = 0; i < x; ++i)
        {
            for (int j = 0; j < y; ++j)
            {
                for (int k = 0; k < z; ++k)
                {
                    container[i, j, k] = new List<GameObject>();
                }
            }
        }
    }
    #endregion


    //methods
    #region
    public static void StoreGameObject(GameObject go)
    {
        int _x = -1;
        int _y = -1;
        int _z = -1;

        if ((int)CreateGridPos(go).x < 0)
        {
            _x = 0;
            go.GetComponent<Part>().OverrideGridPosition('x', 0);
        }
        else if ((int)CreateGridPos(go).x >= X)
        {
            _x = X - 1;
            go.GetComponent<Part>().OverrideGridPosition('x', X - 1);
        }
        else
        {
            _x = (int)CreateGridPos(go).x;
        }

        if ((int)CreateGridPos(go).y < 0)
        {
            _y = 0;
            go.GetComponent<Part>().OverrideGridPosition('y', 0);
        }
        else if ((int)CreateGridPos(go).y >= Y)
        {
            _y = Y - 1;
            go.GetComponent<Part>().OverrideGridPosition('y', Y - 1);
        }
        else
        {
            _y = (int)CreateGridPos(go).y;
        }

        if ((int)CreateGridPos(go).z < 0)
        {
            _z = 0;
            go.GetComponent<Part>().OverrideGridPosition('z', 0);
        }
        else if ((int)CreateGridPos(go).z >= Z)
        {
            _z = Z - 1;
            go.GetComponent<Part>().OverrideGridPosition('z', Y - 1);
        }
        else
        {
            _z = (int)CreateGridPos(go).z;
        }
        container[_x, _y, _z].Add(go);
    }

    public static void RemoveGameObject(GameObject go)
    {
        if (container[(int)CreateGridPos(go).x, (int)CreateGridPos(go).y, (int)CreateGridPos(go).z].Contains(go))
            container[(int)CreateGridPos(go).x, (int)CreateGridPos(go).y, (int)CreateGridPos(go).z].Remove(go);
        else
            Debug.Log("Connection Not Found");
    }

    public static void RemoveGameObject(GameObject go, int _x, int _y, int _z)
    {
        if (container[_x, _y, _z].Contains(go))
            container[_x, _y, _z].Remove(go);
        else
            Debug.Log("Connection Not Found");
    }

    public static List<GameObject> RevealCloseGos(GameObject go)
    {
        List<GameObject> gos = new List<GameObject>();
        if ((int)CreateGridPos(go).x != -1 && (int)CreateGridPos(go).y != -1 && (int)CreateGridPos(go).z != -1)
        {
            for (int i = (int)CreateGridPos(go).x - 1; i <= (int)CreateGridPos(go).x + 1; ++i)
            {
                for (int j = (int)CreateGridPos(go).y - 1; j <= (int)CreateGridPos(go).y + 1; ++j)
                {
                    for (int k = (int)CreateGridPos(go).z - 1; k <= (int)CreateGridPos(go).z + 1; ++k)
                    {
                        if (i >= 0 && j >= 0 && k >= 0 && i < X && j < Y && k < Z)
                        {
                            if (container[i, j, k].Count > 0)
                            {
                                gos.AddRange(container[i, j, k]);
                            }
                        }
                    }
                }
            }
        }


        return gos;
    }

    public static List<GameObject> RevealAllGos()
    {
        List<GameObject> gos = new List<GameObject>();

        for (int i = 0; i < x; ++i)
        {
            for (int j = 0; j < y; ++j)
            {
                for (int k = 0; k < z; ++k)
                {
                    if (container[i, j, k].Count > 0)
                        gos.AddRange(container[i, j, k]);
                }
            }
        }

        return gos;
    }

    public static void RemoveAllGos()
    {
        List<GameObject> gos = new List<GameObject>();

        for (int i = 0; i < x; ++i)
        {
            for (int j = 0; j < y; ++j)
            {
                for (int k = 0; k < z; ++k)
                {
                    container[i, j, k] = new List<GameObject>();     
                }
            }
        }
    }

    private static Vector3 CreateGridPos(GameObject go)
    {
        Vector3 pos = go.transform.position;
        Vector3 gridPos = Vector3.zero;
        Part p = go.GetComponent<Part>();

        gridPos.x = (float)Math.Floor(pos.x / InitializeGameArea.CollisionStep) - offsetX;
        if (gridPos.x < 0 && gridPos.x >= x && p.GridOverride.x == -1)
        {
            gridPos.x = -1;
        }
        else if (p.GridOverride.x != -1)
        {
            gridPos.x = p.GridOverride.x;
        }

        gridPos.y = (float)Math.Floor(pos.y / InitializeGameArea.CollisionStep) - offsetY;
        if (gridPos.y < 0 && gridPos.y >= y && p.GridOverride.y == -1)
        {
            gridPos.y = -1;
        }
        else if (p.GridOverride.y != -1)
        {
            gridPos.y = p.GridOverride.y;
        }

        gridPos.z = (float)Math.Floor(pos.z / InitializeGameArea.CollisionStep) - offsetZ;
        if (gridPos.z < 0 && gridPos.z >= z && p.GridOverride.z == -1)
        {
            gridPos.z = -1;
        }
        else if (p.GridOverride.z != -1)
        {
            gridPos.z = p.GridOverride.z;
        }

        return gridPos;
    }
    #endregion

}
