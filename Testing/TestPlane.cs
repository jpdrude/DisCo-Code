/*
Project DisCo (Discrete Choreography) (GPL) initiated by Jan Philipp Drude

This file is part of Project DisCo.

Copyright (c) 2019, Jan Philipp Drude <jpdrude@gmail.com>

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

public class TestPlane : MonoBehaviour {

    List<Connection> align = new List<Connection>();

    [SerializeField]
    Vector3 pos;

    [SerializeField]
    Vector3 xVec;

    [SerializeField]
    Vector3 yVec;

    [SerializeField]
    Vector3 zVec;

    List<GameObject> grid = new List<GameObject>();
    //List<TextMesh> txt = new List<TextMesh>();

    // Use this for initialization
    void Start () {
        Part part = gameObject.GetComponent<Part>();
        GameObject pln = Resources.Load<GameObject>("GridPlane");

        if (part != null)
        {
            foreach (Connection c in part.Connections)
            {
                grid.Add(Instantiate(pln));
                align.Add(c);
                //txt.Add(pln.GetComponentInChildren<TextMesh>());
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
        SetPlaneReference();
    }

    void SetPlaneReference()
    {
        for (int i = 0; i < grid.Count; ++i)
        {
            grid[i].transform.position = align[i].Pln.Origin;
            grid[i].transform.rotation = align[i].Pln.GetEulerQuaternion();
            //txt[i].text = string.Format("{0};{1};{2}", align[i].GridX, align[i].GridY, align[i].GridZ);
        }
    }
}
