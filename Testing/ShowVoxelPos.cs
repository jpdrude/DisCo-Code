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
using TMPro;

public class ShowVoxelPos : MonoBehaviour
{
    List<TextMeshPro> tms = new List<TextMeshPro>();
    List<Connection> cons = new List<Connection>();

    // Start is called before the first frame update
    void Start()
    {
        foreach (Connection c in gameObject.GetComponent<Part>().Connections)
        {
            GameObject go = new GameObject();
            go.transform.parent = gameObject.transform;
            go.transform.localPosition = c.Pln.LocalOrigin;
            go.transform.rotation = c.Pln.GetEulerQuaternion();
            go.transform.Rotate(new Vector3(-90, 0, 0));

            TextMeshPro tm = go.AddComponent<TextMeshPro>();
            tms.Add(tm);
            cons.Add(c);
            tm.fontSize = 0.07f;
            tm.rectTransform.sizeDelta = new Vector2(0.06f, 0.02f);
            tm.alignment = TextAlignmentOptions.Justified;
            tm.alignment = TextAlignmentOptions.Center;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < cons.Count; ++i)
        {
            tms[i].text = string.Format("{0}, {1}, {1}", cons[i].GridX, cons[i].GridY, cons[i].GridZ);
        }
    }
}
