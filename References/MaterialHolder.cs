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
 * Holds Materials and Colors of Selected Material Set
 */

public static class MaterialHolder
{
    private static Material affectedMat = null;
    public static Material AffectedMat
    {
        get { return affectedMat; }
    }

    private static Material unaffectedMat = null;
    public static Material UnaffectedMat
    {
        get { return unaffectedMat; }
    }

    private static Material disabledMat = null;
    public static Material DisabledMat
    {
        get { return disabledMat; }
    }

    private static Material enabledMat = null;
    public static Material EnabledMat
    {
        get { return enabledMat; }
    }

    private static Color affectedColor = new Color(0.3383838f, 0.7676765f, 1);
    public static Color AffectedColor
    {
        get { return affectedColor; }
    }

    private static Color unaffecetedColor = new Color(0.2313177f, 0.5211295f, 0.678f);
    public static Color UnaffectedColor
    {
        get { return unaffecetedColor; }
    }

    private static Material activeFont;
    public static Material ActiveFont
    {
        get { return activeFont; }
    }

    private static Material inactiveFont;
    public static Material InactiveFont
    {
        get { return inactiveFont; }
    }

    private static Material toolsetMaterial;
    public static Material ToolsetMaterial
    {
        get { return toolsetMaterial; }
    }

    static Material frozenMat = null;
    public static Material FrozenMat { get { return frozenMat; } }

    static Material selectedMat = null;
    public static Material SelectedMat { get { return selectedMat; } }

    static Material unselectedMat = null;
    public static Material UnselectedMat { get { return unselectedMat; } }

    static Material highlightFrozenMat = null;
    public static Material HighlightFrozenMat { get { return highlightFrozenMat; } }

    static Material disableSphereMat = null;
    public static Material DisableSphereMat { get { return disableSphereMat; } }

    static Material enableSphereMat = null;
    public static Material EnableSphereMat { get { return enableSphereMat; } }

    static Material deleteSphereMat = null;
    public static Material DeleteSphereMat { get { return deleteSphereMat; } }

    static Material lineMat = null;
    public static Material LineMat { get { return lineMat; } }

    public static AnaglyphizerC.Mode Anaglyph { get; set; }


    private static string matSet = "Candy";
    public static string MatSet
    {
        get { return matSet; }
        set
        {
            matSet = value;

            if (string.Equals(matSet, "Candy"))
            {
                affectedColor = new Color(0.3383838f, 0.7676765f, 1);
                unaffecetedColor = new Color(0.2313177f, 0.5211295f, 0.678f);
            }
            else if (string.Equals(matSet, "BlackWhite"))
            {
                affectedColor = new Color(1, 1, 1);
                unaffecetedColor = new Color(0, 0, 0);
            }
            else if (string.Equals(matSet, "DroppingSun"))
            {
                affectedColor = new Color(1, 0.52f, 0.394f);
                unaffecetedColor = new Color(0.535f, 0.304f, 0.246f);
            }
            else if (string.Equals(matSet, "Space"))
            {
                affectedColor = new Color(0.3383838f, 0.7676765f, 1);
                unaffecetedColor = new Color(0.2313177f, 0.5211295f, 0.678f);
            }

            Initialize();
        }
    }



    public static void Initialize()
    {
        affectedMat = Resources.Load<Material>("Materials/" + matSet + "/affectedMaterial");
        unaffectedMat = Resources.Load<Material>("Materials/" + matSet + "/unaffectedMaterial");

        enabledMat = Resources.Load<Material>("Materials/" + matSet + "/PlacedMaterial");
        disabledMat = Resources.Load<Material>("Materials/" + matSet + "/lowlitPlacedMaterial");

        activeFont = Resources.Load<Material>("Materials/" + matSet + "/Toolset/Toolset_Font");
        inactiveFont = Resources.Load<Material>("Materials/" + matSet + "/Toolset/Toolset_Font_Inactive");

        toolsetMaterial = Resources.Load<Material>("Materials/" + matSet + "/Toolset/Toolset_Glass");


        frozenMat = Resources.Load<Material>("Materials/" + matSet + "/PlacedMaterial");

        selectedMat = Resources.Load<Material>("Materials/" + matSet + "/affectedMaterial");
        unselectedMat = Resources.Load<Material>("Materials/" + matSet + "/unaffectedMaterial");

        highlightFrozenMat = Resources.Load<Material>("Materials/" + matSet + "/highlightPlacedMaterial");

        enableSphereMat = Resources.Load<Material>("Materials/" + matSet + "/Toolset/enableSphereMat");
        disableSphereMat = Resources.Load<Material>("Materials/" + matSet + "/Toolset/disableSphereMat");
        deleteSphereMat = Resources.Load<Material>("Materials/" + matSet + "/Toolset/deleteSphereMat");

        lineMat = Resources.Load<Material>("Materials/" + matSet + "/LineMat");
    }

}
