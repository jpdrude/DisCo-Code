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
 * Connection Class
 */

public class Connection : ICloneable
{
    //properties
    #region
    AlignPlane pln;
    public AlignPlane Pln
    {
        get { return pln; }
    }

    private string conType;
    public string ConType
    {
        get { return conType; }
    }

    private string part;
    public string Part
    {
        get { return part; }
    }

    private int conId;
    public int ConID
    {
        get { return conId; }
    }

    private int matrixId;
    public int MatrixId
    {
        get { return matrixId; }
    }

    public AlignPlane FlipPln
    {
        get { return AlignPlane.FlipCopy(Pln); }
    }

    private List<Rule> rulesTable;
    public List<Rule> RulesTable
    {
        get { return rulesTable; }
    }

    private List<int> activeRules;
    public List<int> ActiveRules
    {
        get { return activeRules; }
        set { activeRules = value; }
    }

    private Part parentPart;
    public Part ParentPart
    {
        get { return parentPart; }
        set { parentPart = value; }
    }


    public int GridX
    {
        get
        {
            int gridX = (int)Math.Floor(pln.Origin.x / InitializeGameArea.ConnectionStep) - InitializeGameArea.ConOffsetX;
            if (gridX >= 0 && gridX < InitializeGameArea.ConX)
                return gridX;
            else
                return -1;
        }
    }

    public int GridY
    {
        get
        {
            int gridY = (int)Math.Floor(pln.Origin.y / InitializeGameArea.ConnectionStep) - InitializeGameArea.ConOffsetY;
            if (gridY >= 0 && gridY < InitializeGameArea.ConY)
                return gridY;
            else
                return -1;
        }
    }

    public int GridZ
    {
        get
        {
            int gridZ = (int)Math.Floor(pln.Origin.z / InitializeGameArea.ConnectionStep) - InitializeGameArea.ConOffsetZ;
            if (gridZ >= 0 && gridZ < InitializeGameArea.ConZ)
                return gridZ;
            else
                return -1;
        }
    }
    #endregion


    //methods
    #region
    public Connection(AlignPlane _plane, string _type, string _part, int _id, int _matrixId)
    {
        pln = _plane;
        conType = _type;
        part = _part;
        conId = _id;

        matrixId = _matrixId;
        rulesTable = new List<Rule>();
        activeRules = new List<int>();
    }
    
    public object Clone()
    {
        Connection cloneConn = new Connection((AlignPlane)pln.Clone(), conType, part, conId, matrixId);
        return cloneConn;
    }

    public void PlaceInGrid()
    {
        ConnectionVoxelContainer.Container[(int)Math.Round(pln.Origin.x), (int)Math.Round(pln.Origin.y), (int)Math.Round(pln.Origin.z)].Add(this);
    }

    public void RemoveFromGrid()
    {
        if (ConnectionVoxelContainer.Container[(int)Math.Round(pln.Origin.x), (int)Math.Round(pln.Origin.y), (int)Math.Round(pln.Origin.z)].Contains(this))
            ConnectionVoxelContainer.Container[(int)Math.Round(pln.Origin.x), (int)Math.Round(pln.Origin.y), (int)Math.Round(pln.Origin.z)].Remove(this);
    }

    public void GenerateRulesTable(List<Rule> rules)
    {
        int count = 0;
        foreach(Rule rule in rules)
        {
            if (rule.Part1 == Part && rule.Conn1 == ConID)
            {
                rulesTable.Add(rule);
                activeRules.Add(count);
                ++count;
            }
        }
    }


    public int CheckForRule(Connection c)
    {
        foreach (int i in ActiveRules)
        {
            if (RulesTable[i].Part2 == c.Part && RulesTable[i].Conn2 == c.ConID)
            {
                return i;
            }
        }
        return -1;
    }

    #endregion
}

