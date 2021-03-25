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
 * Toolitem: Pick n' Chose
 * 
 * Picks up free part and parents to controller
 * Snaps to aggregation when in proximity
 */

public class TIPickNChose : ToolItem
{
    GameObject pickup = null;
    GameObject pickupCarry = null;

    [SerializeField]
    GameObject castOrigin;

    Camera cam;

    [SerializeField]
    LineRenderer lineRenderer;

    GameObject target = null;

    RaycastHit hit;
    int layerMaskPickup;

    Vector3[] lrPos = new Vector3[2];

    Vector3 rayFrom = Vector3.zero;
    Vector3 rayTo = Vector3.zero;

    ConnectionScanningHandler handler = null;

    public override void Initialize(Toolset _toolSet)
    {
        if (target != null)
            Destroy(target);

        base.Initialize(_toolSet);

        lrPos[0] = Vector3.zero;
        lrPos[1] = Vector3.forward;

        target = Instantiate(ToolSet.ControllerTarget, ToolSet.ControllerTarget.transform.position, ToolSet.ControllerTarget.transform.rotation,
                             ToolSet.ControllerTarget.transform.parent);
        target.name = "PickNChoseTarget";

        ScaleTool.KeepLarge.Add(target);

        cam = castOrigin.GetComponentInChildren<Camera>();
        layerMaskPickup = 1 << 8;
    }

    public override void ActivateItem()
    {
        PlacementReferences.PlacementType = PlacementReferences.PlaceChoreo.PickNChose;
        PlacementReferences.Aiming = true;
        this.enabled = true;
    }

    public override void DeactivateItem()
    {
        base.DeactivateItem();

        if (pickupCarry != null) ReleasePart();

        PlacementReferences.Aiming = false;
        this.enabled = false;
    }

    public override void Click()
    {
        if (pickup != null)
            PickupPart();
        else if (pickupCarry != null)
            ReleasePart();
    }

    public override void ScrollUp()
    {
        if (pickupCarry == null) return;


        pickupCarry.transform.localPosition += Vector3.forward * Time.deltaTime;
    }

    public override void ScrollDown()
    {
        if (pickupCarry == null) return;


        Vector3 newPos = pickupCarry.transform.localPosition - Vector3.forward * Time.deltaTime;

        if (newPos.z > -0.99f)
            pickupCarry.transform.localPosition = newPos;
    }

    public override void NextOptionPressed()
    {
        if (pickupCarry == null) return;

        pickupCarry.transform.RotateAround(pickupCarry.transform.position, pickupCarry.transform.up, Time.deltaTime * 100);
    }

    public override void PrevOptionPressed()
    {
        if (pickupCarry == null) return;

        pickupCarry.transform.RotateAround(pickupCarry.transform.position, pickupCarry.transform.forward, Time.deltaTime * 100);
    }

    private void Update()
    {
        CastPickUpRay();
    }

    public void PickupPart()
    {
        if (BoltNetwork.IsRunning)
        {
            BoltEntity entity = pickup.GetComponent<BoltEntity>();

            if (!entity.IsOwner)
            {
                Vector3 pos = entity.gameObject.transform.position;
                Quaternion rot = entity.gameObject.transform.rotation;
                int tempID = entity.gameObject.GetComponent<Part>().TemplateID;

                var destroy = BlockDestroy.Create(entity);
                destroy.Send();

                PartToken token = new PartToken();
                token.TemplateID = tempID;
                //token.Free = true;
                //token.Position = pos;
                //token.Rotation = rot;

                GameObject go = BoltNetwork.Instantiate(BoltPrefabs.AbstractBlock, token, pos, rot);
                pickup = go;
            }
        }

        Pickup();
    }

    public void ReleasePart(bool keepHandler = false)
    {
        if (pickupCarry != null)
        {
            pickupCarry.transform.parent = null;
            pickupCarry.GetComponent<ConnectionScanning>().enabled = false;

            if (!keepHandler)
                Destroy(pickupCarry.GetComponent<ConnectionScanningHandler>());

            pickupCarry.GetComponent<Rigidbody>().isKinematic = false;
            pickupCarry.GetComponent<MeshRenderer>().material = MaterialHolder.UnselectedMat;
            PlacementReferences.Aiming = true;
            pickupCarry = null;
        }
    }

    public void Pickup()
    {
        pickupCarry = pickup;
        pickup = null;

        pickupCarry.GetComponent<MeshRenderer>().material = MaterialHolder.SelectedMat;
        pickupCarry.transform.SetParent(target.transform, true);
        pickupCarry.GetComponent<Rigidbody>().isKinematic = true;
        pickupCarry.GetComponent<ConnectionScanning>().enabled = true;

        if (pickupCarry.GetComponent<ConnectionScanningHandler>() == null)
        {
            handler = pickupCarry.AddComponent<ConnectionScanningHandler>();
            handler.ConnectionTerminated += new ConnectionScanningHandler.ConnectionHandler(AfterConnection);
            handler.ConnectionFailed += new ConnectionScanningHandler.ConnectionHandler(AfterFailedConnection);
        }

        PlacementReferences.Aiming = false;
    }

    private void CastPickUpRay()
    {
        if (pickupCarry == null)
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

            if (Physics.Raycast(rayFrom, rayTo, out hit, Mathf.Infinity, layerMaskPickup))
            {
                if (pickup != null)
                {
                    pickup.GetComponent<MeshRenderer>().material = MaterialHolder.UnselectedMat;
                }
                pickup = hit.collider.gameObject;
                pickup.GetComponent<MeshRenderer>().material = MaterialHolder.SelectedMat;
            }
            else
            {
                if (pickup != null)
                {
                    pickup.GetComponent<MeshRenderer>().material = MaterialHolder.UnselectedMat;
                    pickup = null;
                }
            }
        }
    }

    private void AfterConnection(GameObject go)
    {
        if (pickupCarry != null)
        {
            pickupCarry.transform.parent = null;
            Destroy(pickupCarry.GetComponent<ConnectionScanningHandler>());
            pickupCarry = null;
            pickup = null;
        }
        PlacementReferences.Aiming = true;
        handler = null;
    }

    private void AfterFailedConnection(GameObject go)
    {
        pickup = go;
        go.GetComponent<MeshRenderer>().material = MaterialHolder.SelectedMat;
        PickupPart();
    }
}
