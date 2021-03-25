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
using UnityEngine.Analytics;
using TMPro;

/*
 * Toolitem: Chose Curve
 * 
 * Lets player chose position from available constraint Curves
 */

public class TINextCurve : ToolItem
{
    [SerializeField]
    FirstPersonController controller;

    [SerializeField]
    List<ToolItem> constraints;

    TextMeshPro text;

    Player player;

    bool chosing = false;
    public bool Chosing { get { return chosing; } }

    public override void Initialize(Toolset _toolSet)
    {
        base.Initialize(_toolSet);

        text = GetComponent<TextMeshPro>();

        player = ControllerReferences.MultiPlayer;
    }

    public override void ActivateItem()
    {
        if (!chosing)
        {
            Highlight();

            text.text = "Chose\n< >";
            chosing = true;

            foreach (ToolItem constraint in constraints)
                constraint.Lowlight();

            player.HighlightAllCurves();

            player.SavePos();
        }
        else
        {
            chosing = false;
            ResetText();
            controller.GoToCurve();
            player.HideAllPlaceholders();
            player.HighlightCurrentCurve();
        }
    }

    public override void DeactivateItem()
    {
        chosing = false;
        ResetText();
        player.RestorePos();
        player.HideAllPlaceholders();

        if (controller.Constraint.Type == GeometryConstraint.ConstraintType.Point)
            player.HighlightCurrentPoint();
        else if (controller.Constraint.Type == GeometryConstraint.ConstraintType.Curve)
            player.HighlightCurrentCurve();
        else if (controller.Constraint.Type == GeometryConstraint.ConstraintType.Box)
            player.HighlightCurrentBox();
    }

    public override void FocusItem()
    {
        Highlight();
    }

    public override void UnfocusItem()
    {
        base.UnfocusItem();

        Lowlight();

        DeactivateItem();
    }

    void ResetText()
    {
        text.text = "Change Crv";
    }

    public void Left()
    {
        player.GetPrevCurve();
        player.HighlightAllCurves();
    }

    public void Right()
    {
        player.GetNextCurve();
        player.HighlightAllCurves();
    }
}
