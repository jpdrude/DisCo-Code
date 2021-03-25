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

using UnityEngine;
using TMPro;

/*
 * Tool: Number of Parts
 * 
 * overrides tool to change value directly without using ToolItem
 * Changes Number of Parts for Choreography mode
 */

public class NumPartsTool : Tool
{
    TextMeshPro affectedPartsCounter;

    [SerializeField]
    int startAffectedParts = 20;


    public override void Initialize(Toolset toolSet)
    {
        base.Initialize(toolSet);

        affectedPartsCounter = ActiveItem.GetComponent<TextMeshPro>();
        PlacementReferences.AffectedParts = startAffectedParts;
        affectedPartsCounter.text = PlacementReferences.AffectedParts.ToString();

    }

    public override void Left() { return; }

    public override void Right() { return; }

    public override void LeftPressed()
    {
        if (PlacementReferences.AffectedParts > 0)
        {
            --PlacementReferences.AffectedParts;
            if (PlacementReferences.PlacementType == PlacementReferences.PlaceChoreo.Choreo)
            {
                GlobalReferences.ChangeAffectedNumber(PlacementReferences.AffectedParts);
            }
        }
        affectedPartsCounter.text = PlacementReferences.AffectedParts.ToString();
    }

    public override void RightPressed()
    {
        if (PlacementReferences.AffectedParts < GlobalReferences.FreeParts.Count + GlobalReferences.AffectedParts.Count)
        {
            ++PlacementReferences.AffectedParts;
            if (PlacementReferences.PlacementType == PlacementReferences.PlaceChoreo.Choreo)
            {
                GlobalReferences.ChangeAffectedNumber(PlacementReferences.AffectedParts);
            }
        }
        affectedPartsCounter.text = PlacementReferences.AffectedParts.ToString();
    }
}
