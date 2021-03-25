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

using System.Collections.Generic;
using UnityEngine;

/*
 * Base class for
 *      - Kill Sphere
 *      - Enable/Disable Sphere
 *      - Delte Particle Sphere
 *      
 * Instantiates Sphere 
 * Contains
 *      - Scale
 *      - Click
 *      - Trigger
 */

public class TriggerSphere : MonoBehaviour
{
    float scaleFac = 0.2f;
    public float ScaleFac { get { return scaleFac; } set { scaleFac = value; } }

    List<GameObject> purgatory = new List<GameObject>();
    public List<GameObject> Purgatory { get { return purgatory; } }

    float scale;
    public float Scale 
    { 
        get 
        {
            float _scale = scale;
            Transform go = transform.parent;
            while (go != null)
            {
                _scale *= go.localScale.x;
                go = go.transform.parent;
            }

            return _scale; 
        
        } 
    }

    Vector3 sphereBottom;

    Vector3 sphereDirection;

    bool clicked = false;
    public bool Clicked { get { return clicked; } }

    public virtual void Initialize(float _scale, Material mat, Vector3 _sphereBottom, Vector3 _sphereDirection)
    {
        scale = _scale;
        GetComponent<MeshRenderer>().material = mat;
        sphereBottom = _sphereBottom;
        sphereDirection = _sphereDirection;

        gameObject.transform.localPosition = sphereBottom + sphereDirection * (scale / 2);
        gameObject.transform.localScale = Vector3.one * scale;
    }

    public void Click() 
    {
        clicked = true;
    }

    public virtual void Unclick()
    {
        clicked = false;
    }


    public virtual void ScrollUp()
    {
        scale += Time.deltaTime * scaleFac;
        transform.localScale = Vector3.one * scale;
        transform.localPosition = sphereBottom + sphereDirection * (scale / 2);
    }

    public virtual void ScrollDown()
    {
        float newScale = scale - Time.deltaTime * scaleFac;
        if (newScale > 0.01f)
        {
            scale = newScale;
            transform.localScale = Vector3.one * scale;
            transform.localPosition = sphereBottom + sphereDirection * (scale / 2);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!purgatory.Contains(other.gameObject))
        {
            purgatory.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (purgatory.Contains(other.gameObject))
        {
            purgatory.Remove(other.gameObject);
        }
    }

    private void OnDisable()
    {
        purgatory.Clear();
    }
}
