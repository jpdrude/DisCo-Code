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
using System.Security.Cryptography;
using UnityEngine;

/*
 * If part is spawned through Load from streamed file it checks if it's parent is already spawned
 * sets it when it is found
 */

public class NetworkPartWaitForParent : MonoBehaviour
{
    int parentID;
    int id;
    int con;
    int parentCon;

    public void Initialize(int _parentID, int _id, int _con, int _parentCon)
    {
        parentID = _parentID;
        id = _id;
        con = _con;
        parentCon = _parentCon;
    }

    void Update()
    {
        if (GlobalReferences.FrozenParts.ContainsKey(parentID))
        {
            Part parentPart = GlobalReferences.FrozenParts[parentID].GetComponent<Part>();

            if (!parentPart.Children.Contains(id))
            {
                parentPart.Children.Add(id);
                parentPart.ChildCons.Add(con);
                parentPart.SetInactive(parentPart.Connections[parentCon]);
            }

            Destroy(this);
        }
    }
}
