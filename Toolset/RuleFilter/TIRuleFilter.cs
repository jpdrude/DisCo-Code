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

using TMPro;
using UnityEngine;

/*
 * ToolItem: Rule Filter (template)
 * 
 * is instantiated for each rule group/rule grammar to set activity flags
 * holds name
 */

public class TIRuleFilter : ToolItem
{
    string groupKey;
    public string GroupKey { get { return groupKey; } }
    string grammar;

    bool active = true;
    public bool Active { get { return active; } }

    TextMeshPro textMesh;

    public void Initialize(string _groupKey, string _grammar, float activeSize)
    {
        groupKey = _groupKey;
        grammar = _grammar;

        textMesh = gameObject.AddComponent<TextMeshPro>();
        RectTransform rt = gameObject.GetComponent(typeof(RectTransform)) as RectTransform;
        rt.sizeDelta = new Vector2(0.1f, 0.07f);
        textMesh.alignment = TextAlignmentOptions.Center;
        textMesh.faceColor = MaterialHolder.AffectedColor;
        textMesh.text = grammar;
        textMesh.fontSize = activeSize;

        if (NetworkPartEventsListener.RuleFilterTIs.ContainsKey(groupKey))
            NetworkPartEventsListener.RuleFilterTIs.Remove(groupKey);

        NetworkPartEventsListener.RuleFilterTIs.Add(groupKey, this);
    }

    public override void ActivateItem()
    {
        active = !active;


        if (!BoltNetwork.IsRunning || ControllerReferences.IndependantMP)
        {
            if (active)
                Highlight();
            else
                Lowlight();

            //for (int i = GlobalReferences.ActiveRulesGrammer.Count - 1; i >= 0; --i)
            foreach (int[] gramm in GlobalReferences.RuleGroups[groupKey])
            {
                GlobalReferences.RuleMatrix[gramm[0], gramm[1]] = active;
            }
        }
        else
        {
            ChangeRuleFilter evnt = ChangeRuleFilter.Create();
            evnt.State = active;
            evnt.GroupKey = groupKey;
            evnt.Send();
        }

    }

    public void ChangeState(bool _state)
    {
        active = _state;

        if (active)
            Highlight();
        else
            Lowlight();
    }

    public override void DeactivateItem() { }
}
