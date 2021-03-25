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
using System.Text;
using Bolt;

/*
 * Tool: Rule Filter
 * 
 * Override to instantiate Rule Filter according to RuleGroups or grammar
 */

public class RuleFilterTool : Tool
{
    Vector3 filterOffset;

    public override void Initialize(Toolset toolSet)
    {
        base.Initialize(toolSet);

        filterOffset = FilterOffset;

        var filterItems = new List<ToolItem>();

        foreach (string s in GlobalReferences.RuleGroups.Keys)
        {
            string grammar = FormatRule(s);

            var tempGo = Instantiate(ActiveItem.gameObject);
            tempGo.transform.parent = transform;
            tempGo.transform.localPosition = BasePos;
            tempGo.transform.localRotation = ActiveItem.transform.localRotation;

            var filterItem = tempGo.GetComponent<TIRuleFilter>();
            filterItems.Add(filterItem);

            filterItem.Initialize(s, grammar, ActiveFontSize);
        }

        for (int i = 0; i < filterItems.Count; ++i)
        {
            filterItems[i].transform.localPosition += (FilterOffset * i);

            if (i != 0)
                filterItems[i].gameObject.SetActive(false);
            else
            {
                filterItems[i].SetFocus(UnfocusPos, InactiveFontSize, false);
            }
        }

        Destroy(ActiveItem.gameObject);

        ChangeToolItemList(filterItems);
    }

    private string FormatRule(string s)
    {
        string grammer = s;

        if (grammer.Length > 12)
        {
            StringBuilder sb = new StringBuilder();
            int saveLength = 0;

            foreach (char c in grammer)
            {
                sb.Append(c);
                if (c == '>' || c == '<')
                {
                    if (sb.Length > 12)
                    {
                        filterOffset.x = 12 * filterOffset.x / sb.Length;
                    }
                    sb.Append("\n");
                    saveLength = sb.Length;
                }
            }

            if (sb.Length - saveLength > saveLength)
            {
                filterOffset.x = 12 * filterOffset.x / (sb.Length - saveLength);
            }
            else if (saveLength <= 12 && sb.Length - saveLength > 12)
            {
                filterOffset.x = 12 * filterOffset.x / (sb.Length - saveLength);
            }

            grammer = sb.ToString();
        }

        return grammer;
    }
}
