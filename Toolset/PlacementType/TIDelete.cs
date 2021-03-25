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
 * Toolitem: Delete
 * 
 * Deletes chosen Part
 */

public class TIDelete : ToolItem
{
    [SerializeField]
    GameObject castOrigin;

    Camera cam;

    [SerializeField]
    LineRenderer lineRenderer;

    GameObject target;

    GameObject toDelete = null;
    public GameObject ToDelete { get { return toDelete; } }

    RaycastHit hit;
    int layerMaskDelete;

    Vector3[] lrPos = new Vector3[2];

    Vector3 rayFrom = Vector3.zero;
    Vector3 rayTo = Vector3.zero;

    public override void Initialize(Toolset _toolSet)
    {
        base.Initialize(_toolSet);

        lrPos[0] = Vector3.zero;
        lrPos[1] = Vector3.forward;

        cam = castOrigin.GetComponentInChildren<Camera>();

        layerMaskDelete = 1 << 9;

        this.enabled = false;
    }

    public override void ActivateItem()
    {
        PlacementReferences.PlacementType = PlacementReferences.PlaceChoreo.Delete;
        PlacementReferences.Aiming = true;
        this.enabled = true;
    }

    public override void DeactivateItem()
    {
        base.DeactivateItem();

        if (toDelete != null)
        {
            toDelete.GetComponent<MeshRenderer>().material = MaterialHolder.FrozenMat;
            ChangeChildMat(toDelete, MaterialHolder.FrozenMat);

            toDelete = null;
        }

        PlacementReferences.Aiming = false;
        this.enabled = false;
    }

    public override void Click()
    {
        if (toDelete != null)
            DeletePart(ToDelete);
    }

    private void Update()
    {
        CastDeleteRay();
    }

    public void DeletePart(GameObject toDelete)
    {
        toDelete.GetComponent<Part>().Delete();
    }

    public void CastDeleteRay(bool highlightChildren = false)
    {
        if (PlacementReferences.PlayMode == PlacementReferences.Mode.VR)
        {

            lrPos[0] = castOrigin.transform.position;
            lrPos[1] = castOrigin.transform.position + castOrigin.transform.TransformDirection(Vector3.forward) * 100;
            lineRenderer.SetPositions(lrPos);

            rayFrom = castOrigin.transform.position;
            rayTo = castOrigin.transform.forward;
        }
        else
        {
            rayFrom = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
            rayTo = cam.transform.forward;
        }

        if (Physics.Raycast(rayFrom, rayTo, out hit, Mathf.Infinity, layerMaskDelete))
        {
            if (toDelete != null)
            {
                LowlightPart(toDelete);
                if (highlightChildren)
                    ChangeChildMat(toDelete, false);
                
            }
            toDelete = hit.collider.gameObject;
            toDelete.GetComponent<MeshRenderer>().material = MaterialHolder.HighlightFrozenMat;
            if (highlightChildren)
                ChangeChildMat(toDelete, true);
        }
        else
        {
            if (toDelete != null)
            {
                LowlightPart(toDelete);
                if (highlightChildren)
                    ChangeChildMat(toDelete, false);
                toDelete = null;
            }
        }
    }

    void LowlightPart(GameObject go)
    {
        if (!go.GetComponent<Part>().Disabled)
            go.GetComponent<MeshRenderer>().material = MaterialHolder.FrozenMat;
        else
            go.GetComponent<MeshRenderer>().material = MaterialHolder.DisabledMat;
    }

    void ChangeChildMat(GameObject parent, bool highlight)
    {
        Part parentPart = parent.GetComponent<Part>();

        foreach (int childID in parentPart.Children)
        {
            GameObject child = GlobalReferences.FrozenParts[childID];

            if (child != null)
            {
                if (highlight)
                    child.GetComponent<MeshRenderer>().material = MaterialHolder.HighlightFrozenMat;
                else
                    LowlightPart(child);

                ChangeChildMat(child, highlight);
            }
        }
    }
}
