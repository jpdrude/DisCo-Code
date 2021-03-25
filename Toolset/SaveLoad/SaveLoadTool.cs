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

/*
 * Tool Save/Load
 * 
 * Override to enable Left and Right Inputs for Load and Export Field
 */

public class SaveLoadTool : Tool
{
    [SerializeField]
    TILoad loadTool;

    [SerializeField]
    TIExportField exportField;

    public override void Left()
    {
        if (loadTool != null && loadTool.Loading)
            loadTool.Left();
        else if (exportField != null && exportField.ShowField)
            return;
        else
            base.Left();
    }

    public override void Right()
    {
        if (loadTool != null && loadTool.Loading)
            loadTool.Right();
        else if (exportField != null && exportField.ShowField)
            return;
        else
            base.Right();
    }

    public override void LeftPressed()
    {
        if (exportField != null && exportField.ShowField)
            exportField.LeftPressed();
        else
            base.LeftPressed();
    }

    public override void RightPressed()
    {
        if (exportField != null && exportField.ShowField)
            exportField.RightPressed();
        else
            base.RightPressed();
    }
}
