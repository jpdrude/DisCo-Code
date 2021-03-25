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
 * Wasp Rule Class
 */

public class Rule {

    private string part1;
    public string Part1
    {
        get { return part1; }
    }

    private int conn1;
    public int Conn1
    {
        get { return conn1; }
    }

    private string part2;
    public string Part2
    {
        get { return part2; }
    }

    private int conn2;
    public int Conn2
    {
        get { return conn2; }
    }

    private int matrixRow;
    public int MatrixRow
    {
        get { return matrixRow; }
    }

    private int matrixColumn;
    public int MatrixColumn
    {
        get { return matrixColumn; }
    }

    private bool active;
    public bool Active
    {
        get { return active; }
        set { active = value; }
    }

    public Rule(string _part1, int _conn1, string _part2, int _conn2, int _matrixRow, int _matrixColumn, bool _active)
    {
        part1 = _part1;
        conn1 = _conn1;
        part2 = _part2;
        conn2 = _conn2;

        matrixRow = _matrixRow;
        matrixColumn = _matrixColumn;

        active = _active;
    }
}
