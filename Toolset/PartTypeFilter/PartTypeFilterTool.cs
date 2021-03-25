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
 * Tool: Part Type Filter
 * 
 * Override to instantiate Toolitems for all template parts
 */

public class PartTypeFilterTool : Tool
{
    public override void Initialize(Toolset toolSet)
    {
        base.Initialize(toolSet);

        var partProbs = new Dictionary<int, float>();
        var filterItems = new List<ToolItem>();

        try
        {
            int count = 0;
            foreach (GameObject go in GlobalReferences.TemplateParts)
            {
                int templateID = go.GetComponent<Part>().TemplateID;

                if (GlobalReferences.PartProbs[templateID] > 0.000001)
                {
                    var tempGo = Instantiate(ActiveItem.gameObject, ActiveItem.transform.position, ActiveItem.transform.rotation, transform);
                    TIPartTypeFilter filterItem = tempGo.GetComponent<TIPartTypeFilter>();
                    filterItems.Add(filterItem);

                    float scale = ScaleFactor(go.GetComponent<MeshFilter>().sharedMesh, 0.04f);

                    filterItem.Initialize(templateID, go.GetComponent<MeshFilter>().sharedMesh, FilterOffset * count, scale);

                    partProbs.Add(templateID, GlobalReferences.PartProbs[templateID]);

                if (count != 0)
                    filterItem.gameObject.SetActive(false);
                else
                    filterItem.SetFocus(UnfocusPos, -1, false);

                    ++count;
                }
            }
            GlobalReferences.TypeFilter = partProbs;
        }
        catch(Exception e)
        {
            Debug.LogError("Error in setting up Type Filters: " + e.Message);
        }

        Destroy(ActiveItem.gameObject);

        ChangeToolItemList(filterItems);

    }

    private float ScaleFactor(Mesh m, float maxSize)
    {
        float _scale;

        Vector3 b = m.bounds.size;
        _scale = b.x;
        if (b.y > _scale)
            _scale = b.y;
        if (b.z > _scale)
            _scale = b.z;

        _scale = maxSize / _scale;

        return _scale;
    }
}
