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
using System.IO;
using System.Text;
using Valve.Newtonsoft.Json;

/*
 * Initializes Anaglyph
 * Changes Menu Colors to grey
 * Initializes Anaglyph Material
 * Loads Callibration Data if Anaglyph is Color
 */

public class InitAnaglyph : MonoBehaviour
{
    [SerializeField]
    Material standardAnaglyph;

    [SerializeField]
    Material trueAnaglyph;

    CallibrateAnaglyphMenu callibrateMenu;

    // Start is called before the first frame update
    void Start()
    {
        AnaglyphizerC anaglyph = GetComponent<AnaglyphizerC>();
        GameObject canvas = GameObject.Find("Canvas");

        callibrateMenu = canvas.GetComponentInChildren<CallibrateAnaglyphMenu>(true);

        if (anaglyph != null)
        { 
            if (MaterialHolder.Anaglyph == AnaglyphizerC.Mode.Color)
            {
                anaglyph.enabled = true;
                anaglyph.anaglyphMat = standardAnaglyph;

                callibrateMenu.LoadAnaglyph();
                callibrateMenu.Anaglyph = anaglyph;

                InitCanvas.ChangeButtonColors(new Color(0.8f, 0.8f, 0.8f), canvas);
                InitCanvas.ChangeSliderColor(new Color(0.8f, 0.8f, 0.8f), canvas);
            }
            else if (MaterialHolder.Anaglyph == AnaglyphizerC.Mode.True)
            {
                anaglyph.enabled = true;
                anaglyph.anaglyphMat = trueAnaglyph;

                InitCanvas.ChangeButtonColors(new Color(0.8f, 0.8f, 0.8f), canvas);
                InitCanvas.ChangeSliderColor(new Color(0.8f, 0.8f, 0.8f), canvas);
            }
        }


    }
}
