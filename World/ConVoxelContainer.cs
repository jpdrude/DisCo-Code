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

/*
 * Stores Connections of aggregated parts according to their spacial position
 * For connection scanning
 */

public static class ConnectionVoxelContainer
{
    //properties
    #region
    private static List<Connection>[,,] container;
    public static List<Connection>[,,] Container
    {
        get { return container; }
        set { container = value; }
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
    #endregion


    //initialize
    #region
    public static void Initialize(int _x, int _y, int _z, int offX, int offY, int offZ)
    {
        x = _x;
        y = _y;
        z = _z;

        container = new List<Connection>[x,y,z];
        offsetX = offX;
        offsetY = offY;
        offsetZ = offZ;  

        for (int i = 0; i < x; ++i)
        {
            for (int j = 0; j < y; ++j)
            {
                for (int k = 0; k < z; ++k)
                {
                    container[i, j, k] = new List<Connection>();
                }
            }
        }
    }
    #endregion


    //methods
    #region
    public static void StoreConnection(Connection c)
    {

        if (c != null && c.GridX != -1 && c.GridY != -1 && c.GridZ != -1)
        {
            container[c.GridX, c.GridY, c.GridZ].Add(c);

            PlaceNeighborConnections(c);
        }
        else if (c != null)
        {
            //MonoBehaviour.Destroy(c.ParentPart.gameObject);
        }
    }

    public static void RemoveConnection(Connection c)
    {
        for (int xx = c.GridX - 1; xx <= c.GridX + 1; ++xx)
        {
            for (int yy = c.GridY - 1; yy <= c.GridY + 1; ++yy)
            {
                for (int zz = c.GridZ - 1; zz <= c.GridZ + 1; ++zz)
                {
                    if (xx >= 0 && xx < X && yy >= 0 && yy < Y && zz >= 0 && zz < Z)
                    {
                        if (container[xx, yy, zz].Contains(c))
                        {
                            container[xx, yy, zz].Remove(c);
                        }
                    }
                }
            }
        }
    }

    public static void RemoveConnection(Connection c, int _x, int _y, int _z)
    {
        for (int xx = _x - 1; xx <= _x + 1; ++xx)
        {
            for (int yy = _y - 1; yy <= _y + 1; ++yy)
            {
                for (int zz = _z - 1; zz <= _z + 1; ++zz)
                {
                    if (xx >= 0 && xx < X && yy >= 0 && yy < Y && zz >= 0 && zz < Z)
                    {
                        if (container[xx,yy,zz].Contains(c))
                        {
                            container[xx, yy, zz].Remove(c);
                        }   
                    }
                }
            }
        }
    }

    public static List<Connection> RevealConnections(Connection c)
    {
        /*
        List<Connection> cons = new List<Connection>();
        if (c != null && c.GridX != -1 && c.GridY != -1 && c.GridZ != -1)
        {
            for (int i = c.GridX - 1; i <= c.GridX + 1; ++i)
            {
                for (int j = c.GridY - 1; j <= c.GridY + 1; ++j)
                {
                    for (int k = c.GridZ - 1; k <= c.GridZ + 1; ++k)
                    {
                        if (i >= 0 && j >= 0 && k >= 0 && i < X && j < Y && k < Z)
                        {
                            if (container[i, j, k].Count > 0)
                            {
                                cons.AddRange(container[i, j, k]);
                            }
                        }
                    }
                }
            }
        }

        return cons;
        */
        if (c.GridX <= X && c.GridY <= Y && c.GridZ <= Z && c.GridX >= 0 && c.GridY >= 0 && c.GridZ >= 0)
        {
            return container[c.GridX, c.GridY, c.GridZ];
        }
        else
        {
            return new List<Connection>();
        }
    }

    public static List<Connection> RevealCloseConnections(Connection c)
    {
        List<Connection> cons = new List<Connection>();

        if (c != null && c.GridX != -1 && c.GridY != -1 && c.GridZ != -1)
        {
            if (container[c.GridX, c.GridY, c.GridZ].Count > 0)
            {
                cons.AddRange(container[c.GridX, c.GridY, c.GridZ]);
            } 
        }

        return cons;
    }

    public static List<Connection> RevealAllConnections()
    {
        List<Connection> cons = new List<Connection>();

        for (int i = 0; i < x; ++i)
        {
            for (int j = 0; j < y; ++j)
            {
                for (int k = 0; k < z; ++k)
                {
                    if (container[i, j, k].Count > 0)
                        cons.AddRange(container[i, j, k]);
                }
            }
        }

        return cons;
    }

    public static int CountAllConnections()
    {
        int count = 0;

        for (int i = 0; i < x; ++i)
        {
            for (int j = 0; j < y; ++j)
            {
                for (int k = 0; k < z; ++k)
                {
                    count += container[i, j, k].Count;
                }
            }
        }

        return count;
    }

    public static void RemoveAllConnections()
    {

        for (int i = 0; i < x; ++i)
        {
            for (int j = 0; j < y; ++j)
            {
                for (int k = 0; k < z; ++k)
                {
                    container[i, j, k] = new List<Connection>();
                        
                }
            }
        }
    }

    public static void PlaceNeighborConnections(Connection c)
    {
        bool xPlus = false;
        bool xMinus = false;

        bool yPlus = false;
        bool yMinus = false;

        bool zPlus = false;
        bool zMinus = false;

        if (c.Pln.Origin.x % InitializeGameArea.ConnectionStep > InitializeGameArea.ConnectionStep - ConnectionScanning.ConnectionThreshold && c.GridX + 1 < X)
        {
            container[c.GridX + 1, c.GridY, c.GridZ].Add(c);
            xPlus = true;
        }

        if (c.Pln.Origin.x % InitializeGameArea.ConnectionStep < ConnectionScanning.ConnectionThreshold && c.GridX - 1 >= 0)
        {
            container[c.GridX - 1, c.GridY, c.GridZ].Add(c);
            xMinus = true;
        }

        if (c.Pln.Origin.y % InitializeGameArea.ConnectionStep > InitializeGameArea.ConnectionStep - ConnectionScanning.ConnectionThreshold && c.GridY + 1 < Y)
        {
            container[c.GridX, c.GridY + 1, c.GridZ].Add(c);
            yPlus = true;
        }

        if (c.Pln.Origin.y % InitializeGameArea.ConnectionStep < ConnectionScanning.ConnectionThreshold && c.GridY - 1 >= 0)
        {
            container[c.GridX, c.GridY - 1, c.GridZ].Add(c);
            yMinus = true;
        }

        if (c.Pln.Origin.z % InitializeGameArea.ConnectionStep > InitializeGameArea.ConnectionStep - ConnectionScanning.ConnectionThreshold && c.GridZ + 1 < Z)
        {
            container[c.GridX, c.GridY, c.GridZ + 1].Add(c);
            zPlus = true;
        }

        if (c.Pln.Origin.z % InitializeGameArea.ConnectionStep < ConnectionScanning.ConnectionThreshold && c.GridZ - 1 >= 0)
        {
            container[c.GridX, c.GridY, c.GridZ - 1].Add(c);
            zMinus = true;
        }

        if (xPlus && yPlus)
        {
            container[c.GridX + 1, c.GridY + 1, c.GridZ].Add(c);
        }

        if (xPlus && yMinus)
        {
            container[c.GridX + 1, c.GridY - 1, c.GridZ].Add(c);
        }

        if (xMinus && yPlus)
        {
            container[c.GridX - 1, c.GridY + 1, c.GridZ].Add(c);
        }

        if (xMinus && yMinus)
        {
            container[c.GridX - 1, c.GridY - 1, c.GridZ].Add(c);
        }

        if (xPlus && zPlus)
        {
            container[c.GridX + 1, c.GridY, c.GridZ + 1].Add(c);
        }

        if (xPlus && zMinus)
        {
            container[c.GridX + 1, c.GridY, c.GridZ - 1].Add(c);
        }

        if (xMinus && zPlus)
        {
            container[c.GridX - 1, c.GridY, c.GridZ + 1].Add(c);
        }

        if (xMinus && zMinus)
        {
            container[c.GridX - 1, c.GridY, c.GridZ - 1].Add(c);
        }

        if (yPlus && zPlus)
        {
            container[c.GridX, c.GridY + 1, c.GridZ + 1].Add(c);
        }

        if (yPlus && zMinus)
        {
            container[c.GridX, c.GridY + 1, c.GridZ - 1].Add(c);
        }

        if (yMinus && zPlus)
        {
            container[c.GridX, c.GridY - 1, c.GridZ + 1].Add(c);
        }

        if (yMinus && zMinus)
        {
            container[c.GridX, c.GridY - 1, c.GridZ - 1].Add(c);
        }

        if (xPlus && yPlus && zPlus)
        {
            container[c.GridX + 1, c.GridY + 1, c.GridZ + 1].Add(c);
        }

        if (xPlus && yPlus && zMinus)
        {
            container[c.GridX + 1, c.GridY + 1, c.GridZ - 1].Add(c);
        }

        if (xMinus && yPlus && zPlus)
        {
            container[c.GridX - 1, c.GridY + 1, c.GridZ + 1].Add(c);
        }

        if (xMinus && yPlus && zMinus)
        {
            container[c.GridX - 1, c.GridY + 1, c.GridZ - 1].Add(c);
        }

        if (xPlus && yMinus && zPlus)
        {
            container[c.GridX + 1, c.GridY - 1, c.GridZ + 1].Add(c);
        }

        if (xPlus && yMinus && zMinus)
        {
            container[c.GridX + 1, c.GridY - 1, c.GridZ - 1].Add(c);
        }

        if (xMinus && yMinus && zPlus)
        {
            container[c.GridX - 1, c.GridY - 1, c.GridZ + 1].Add(c);
        }

        if (xMinus && yMinus && zMinus)
        {
            container[c.GridX - 1, c.GridY - 1, c.GridZ - 1].Add(c);
        }
    }
    #endregion
}
