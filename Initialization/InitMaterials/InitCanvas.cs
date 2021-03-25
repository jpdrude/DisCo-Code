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
using UnityEngine.UI;

/*
 * Initializes Menu visuals to Material Set
 */

public class InitCanvas : MonoBehaviour
{
    [SerializeField]
    List<Image> panels = new List<Image>();
    // Start is called before the first frame update
    void Start()
    {
        Color pressed = MaterialHolder.AffectedColor;


        ChangeButtonColors(pressed, gameObject);
        ChangeDropdownColor(pressed, gameObject);
        ChangeSliderColor(pressed, gameObject);

        foreach (Image img in panels)
        {
            Color col = MaterialHolder.AffectedColor;
            col.a = 0.6f;
            img.color = col;
        }
    }

    public static void ChangeButtonColors(Color pressed, GameObject go)
    {
        Button[] buttons = go.GetComponentsInChildren<Button>(true);


        foreach (Button b in buttons)
        {

            b.colors = BuildColors(pressed, b.colors);
        }
    }

    static void ChangeDropdownColor(Color pressed, GameObject go)
    {
        Dropdown[] dropdowns = go.GetComponentsInChildren<Dropdown>(true);


        foreach(Dropdown d in dropdowns)
        {
            d.colors = BuildColors(pressed, d.colors);
        }
    }

    public static void ChangeSliderColor(Color pressed, GameObject go)
    {
        Slider[] sliders = go.GetComponentsInChildren<Slider>(true);

        foreach (Slider s in sliders)
        {
            s.colors = BuildColors(pressed, s.colors);
        }
    }

    public static ColorBlock BuildColors(Color pressed, ColorBlock baseBlock)
    {
        Color unaffected = new Color(pressed.r * 0.7f, pressed.g * 0.7f, pressed.b * 0.7f);
        Color affected = new Color(pressed.r * 1.3f, pressed.g * 1.3f, pressed.b * 1.3f);

        ColorBlock colors = baseBlock;
        colors.normalColor = unaffected;
        colors.pressedColor = pressed;
        colors.disabledColor = pressed;
        colors.highlightedColor = affected;
        colors.selectedColor = affected;

        return colors;
    }
}
