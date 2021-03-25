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
using System;
using TMPro;

/*
 * Tooitem: Load
 * 
 * Loads Game
 * uses left and right to go through files
 */

public class TILoad : ToolItem
{
    string[] loadList;
    int loadListIndex = 0;

    bool loading = false;
    public bool Loading { get { return loading; } }

    bool reallyLoad = false;

    TextMeshPro textMesh;

    [SerializeField]
    float activeFontSize;

    string caption;

    public override void Initialize(Toolset _toolSet)
    {
        base.Initialize(_toolSet);

        textMesh = GetComponent<TextMeshPro>();

        caption = textMesh.text;
    }

    public override void ActivateItem()
    {
        if (!loading && !reallyLoad)
        {
            loadList = SaveLoad.GetStoredFiles(SaveLoad.SavePath);
            Array.Sort(loadList, StringComparer.InvariantCulture);
            Array.Reverse(loadList);
            loading = true;
            textMesh.enableAutoSizing = true;
            if (loadList.Length > 0)
            {
                textMesh.text = loadList[0];
            }
            else
            {
                textMesh.text = "no Files found";
            }
        }
        else if (loading && !reallyLoad)
        {
            if (loadList.Length == 0)
            {
                ResetTool();
            }
            else
            {
                textMesh.text = "Really\nLoad?";
                reallyLoad = true;
                loading = false;
            }
        }
        else if (reallyLoad)
        {
            SaveLoad.LoadGame(loadList[loadListIndex]);
            ResetTool();
        }
    }

    public override void DeactivateItem()
    {
        ResetTool();
    }

    public override void UnfocusItem()
    {
        ResetTool();
    }

    public void Right()
    {
        if (loading && !reallyLoad)
        {
            if (loadListIndex < loadList.Length - 1)
            {
                ++loadListIndex;
                textMesh.text = loadList[loadListIndex];
            }
            else
            {
                loadListIndex = 0;
                textMesh.text = loadList[loadListIndex];
            }
        }
        else if (reallyLoad)
        {
            ResetTool();
        }
    }

    public void Left()
    {
        if (loading && !reallyLoad)
        {
            if (loadListIndex > 0)
            {
                --loadListIndex;
                textMesh.text = loadList[loadListIndex];
            }
            else
            {
                loadListIndex = loadList.Length - 1;
                textMesh.text = loadList[loadListIndex];
            }
        }
        else if (reallyLoad)
        {
            ResetTool();
        }
    }

    public void ResetTool()
    {
        loading = false;
        reallyLoad = false;

        textMesh.text = caption;
        textMesh.enableAutoSizing = false;
        textMesh.fontSize = activeFontSize;
    }

}
