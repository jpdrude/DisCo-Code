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
 * Makes parts respondent to controller in Choreography Mode
 */

public class PartBehaviour : MonoBehaviour
{
    public Vector3 pos;

    ConstantForce force;
    Vector3 f;
    float distF;
    //force to keep parts away from controller
    Vector3 nearF = Vector3.zero;
    //force to pull back parts to controller if too far away
    Vector3 farF = Vector3.zero;

    Vector3 apartF = Vector3.zero;
    float apartFak = 0f;

    float forceScale;

    private void Start()
    {
        force = gameObject.GetComponent<ConstantForce>();
        forceScale = GlobalReferences.ForceScale;
    }

    // Update is called once per frame
    void Update()
    {

        pos = gameObject.transform.position;

        distF = Vector3.Distance(GlobalReferences.ControllerPos, pos);

        farF = (GlobalReferences.ControllerPos - pos).normalized * distF * 0.2f / forceScale;


        distF = (float)Math.Log(distF + 1, 2) + 1;

        nearF = (pos - GlobalReferences.ControllerPos).normalized / (float)Math.Pow(distF, 10f) / forceScale;


        apartFak = Vector3.Distance(GlobalReferences.CenterOfMass, pos);
        apartFak = Gauss(apartFak, 1.3f, 0.76f) * UnityEngine.Random.Range(0.2f,0.4f);

        apartF = ((pos - GlobalReferences.CenterOfMass)).normalized * apartFak * forceScale * 5 * GlobalReferences.DriftApartFac;


        f = (GlobalReferences.ControllerVel * GlobalReferences.ControllerVel.magnitude * 0.2f * distF * 0.5f * forceScale  + nearF + farF + apartF);
        if (f.magnitude > 100)
        {
            f = f.normalized * 100;
        }

        if (!float.IsNaN(f.x) && !float.IsNaN(f.y) && !float.IsNaN(f.z))
            force.force = f;

            
    }
    
    private float Gauss(float x, float off, float shape)
    {
        float v1 = -1 * (x - off) * (x - off) / (2 * shape * shape);
        double v2 = 1 / (shape * Math.Sqrt(2 * Math.PI)) * Math.Pow(Math.E,v1);

        return (float)v2;
    }

}
