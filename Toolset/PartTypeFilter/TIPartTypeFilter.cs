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
using UnityEngine.UIElements;

/*
 * ToolItem: Part Type Filter (template)
 * 
 * is instantiated for each template part to set activity flags
 * holds mesh to visualize templatePart
 */

public class TIPartTypeFilter : ToolItem
{
    Vector3 focusScale;

    [SerializeField]
    float unfocusFactor = 0.4f;

    bool active = true;
    public bool Active { get { return active; } }

    int templateID;
    public int TemplateID { get { return templateID; } }

    public void Initialize(int tempID, Mesh mesh, Vector3 offset, float scale)
    {
        focusScale = new Vector3(scale, scale, scale);

        templateID = tempID;

        SetMesh(mesh, offset);

        if (NetworkPartEventsListener.PartFilterTIs.ContainsKey(templateID))
            NetworkPartEventsListener.PartFilterTIs.Remove(templateID);

        NetworkPartEventsListener.PartFilterTIs.Add(templateID, this);
    }

    public override void Initialize(Toolset _toolSet) { base.Initialize(_toolSet); }

    void SetMesh(Mesh mesh, Vector3 offset)
    {
        GetComponent<MeshFilter>().sharedMesh = mesh;
        GetComponent<MeshRenderer>().material = MaterialHolder.EnabledMat;

        transform.localRotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
        transform.localScale = focusScale;
        transform.localPosition += offset;
    }

    public override void SetFocus(Vector3 pos, float size, bool focus)
    {
        base.SetFocus(pos, size, focus);

        if (focus)
            transform.localScale = focusScale;
        else
            transform.localScale = focusScale * unfocusFactor;
    }

    public override void Highlight()
    {
        GetComponent<MeshRenderer>().material = MaterialHolder.EnabledMat;
    }

    public override void Lowlight()
    {
        GetComponent<MeshRenderer>().material = MaterialHolder.DisabledMat;
    }

    public override void ActivateItem()
    {
        active = !active;

        if (!BoltNetwork.IsRunning || ControllerReferences.IndependantMP)
        {
            if (active)
            {
                GlobalReferences.TypeFilter.Add(templateID, GlobalReferences.PartProbs[templateID]);
                Highlight();
            }
            else
            {
                GlobalReferences.TypeFilter.Remove(templateID);
                Lowlight();
            }

            GlobalReferences.ClearAffectedList();
            if (PlacementReferences.PlacementType == PlacementReferences.PlaceChoreo.Choreo)
            {
                GlobalReferences.ChangeAffectedNumber(PlacementReferences.AffectedParts);
            }
        }
        else
        {
            ChangePartFilter evnt = ChangePartFilter.Create();
            evnt.State = active;
            evnt.ID = templateID;
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

    public override void DeactivateItem(){ }

    public override void Unhide()
    {
        base.Unhide();
    }
}
