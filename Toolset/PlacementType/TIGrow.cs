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

using System.Collections.Generic;
using UnityEngine;
using TMPro;

/*
 * Toolitem: Grow
 * 
 * Player can chose aggregated part to grow from
 * Then Chose connection on part to grow from
 * then go through possible Parts that can connect to that connection
 * finally go through different available connections on that part
 * 
 * Checks collision
 */

public class TIGrow : ToolItem
{
    [SerializeField]
    GameObject castOrigin;

    Camera cam;

    [SerializeField]
    LineRenderer lineRenderer;

    GameObject target;

    Vector3[] lrPos = new Vector3[2];

    Vector3 rayFrom = Vector3.zero;
    Vector3 rayTo = Vector3.zero;

    bool chosePart = false;
    bool choseConnection = false;
    bool choseChild = false;
    public bool ChoseChild { get { return choseChild; } }
    bool choseChildConnection = false;

    GameObject focusPart;
    GameObject chosenPart;

    GameObject focusConnection;
    Connection chosenConnection;

    GameObject connectionSprite;

    List<GameObject> connectionProxies = new List<GameObject>();
    GameObject connectionProxyParent;

    List<GameObject> childProxies = new List<GameObject>();
    int childIndex = -1;
    int conIndex = 0;

    TextMeshPro text;


    private void Update()
    {
        if (chosePart && !choseConnection)
            CastChosePartRay();
        else if (choseConnection && !choseChild)
            CastChoseConnectionRay();
    }

    public override void Initialize(Toolset _toolSet)
    {
        base.Initialize(_toolSet);

        lrPos[0] = Vector3.zero;
        lrPos[1] = Vector3.forward;

        cam = castOrigin.GetComponentInChildren<Camera>();

        connectionSprite = Resources.Load<GameObject>("ConnectionSprite");
        connectionSprite.GetComponent<SpriteRenderer>().color = MaterialHolder.AffectedColor;

        connectionProxyParent = new GameObject();
        connectionProxyParent.name = "ConncetionProxyParent";

        text = GetComponent<TextMeshPro>();

        this.enabled = false;
    }

    public override void ActivateItem()
    {
        if (choseChild && !choseChildConnection)
        {
            choseChildConnection = true;
            text.text = "Con\n< >";
        }
        else if (choseChildConnection)
        {
            if (!childProxies[childIndex].GetComponent<CheckGrowCollision>().Colliding)
            {
                FreezeObject();
                LowlightPart(chosenPart);

                if (!BoltNetwork.IsRunning)
                    childProxies.RemoveAt(childIndex);

                ResetTool();
                ActivateItem();
            }
        }
        else
        {
            base.ActivateItem();
            chosePart = true;
            PlacementReferences.Aiming = true;
            this.enabled = true;
        }
    }

    public override void DeactivateItem()
    {
        base.DeactivateItem();

        ResetTool();
    }

    public override void UnfocusItem()
    {
        base.UnfocusItem();

        ResetTool();

        if (ToolSet.ActiveTool.ActiveItem == this)
            ActivateItem();
    }

    public override void Click()
    {
        if (chosePart && !choseConnection && focusPart != null)
        {
            chosenPart = focusPart;
            choseConnection = true;
            SpawnConnectionProxies(chosenPart);
        }
        else if (choseConnection && focusConnection != null)
        {
            chosenConnection = focusConnection.GetComponent<ConnectionContainer>().Connection;
            childProxies = SpawnChildProxies(chosenConnection);
            text.text = "Part\n< >";

            choseChild = true;

            DestroyConnectionProxies();

            PlacementReferences.Aiming = false;
        }
        else if (choseChild)
            ActivateItem();
    }

    List<GameObject> SpawnChildProxies(Connection c)
    {
        List<GameObject> childPartsProxies = new List<GameObject>();

        for (int i = 0; i < GlobalReferences.TemplateParts.Count; ++i)
        {
            GameObject go = PartsHolder.Holder.SpawnGhostPart(i);
            go.AddComponent<CheckGrowCollision>();
            Destroy(go.GetComponent<ConnectionScanning>());

            List<Connection> templateCons = new List<Connection>();
            Part templatePart = go.GetComponent<Part>();

            foreach (int id in templatePart.ActiveConnections)
                templateCons.Add(templatePart.Connections[id]);

            foreach (Connection templateC in templateCons)
            {
                if (ConnectionScanning.RuleActive(templateC, c))
                {
                    AlignPlane.Orient(templateC.Pln, c.Pln, go);
                    go.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                    childPartsProxies.Add(go);
                    go.SetActive(false);
                    break;
                }
            }

            if (!childPartsProxies.Contains(go))
                Destroy(go);
        }

        if (childPartsProxies.Count > 0)
        {
            childPartsProxies[0].SetActive(true);
            childIndex = 0;
        }

        return (childPartsProxies);
    }

    void SpawnConnectionProxies(GameObject part)
    {
        foreach (int conID in part.GetComponent<Part>().ActiveConnections)
        {
            Connection c = part.GetComponent<Part>().Connections[conID];
            GameObject go = Instantiate(connectionSprite);
            //BoxCollider col = go.AddComponent<BoxCollider>();
            //col.size = new Vector3(0.2f, 0.2f, 0.2f);
            go.transform.position = c.Pln.Origin + c.Pln.ZVector.normalized * 0.002f;
            go.transform.rotation = c.Pln.GetEulerQuaternion();
            go.transform.parent = connectionProxyParent.transform;
            go.transform.localRotation *= Quaternion.Euler(90, 0, 0);
            go.layer = 17;
            go.GetComponentInChildren<SpriteRenderer>().enabled = false;
            go.name = "ConnectionProxy_" + c.ConID;
            ConnectionContainer conCont = go.AddComponent<ConnectionContainer>();
            conCont.Connection = c;
            connectionProxies.Add(go);
        }
    }

    void CastChosePartRay()
    {

        RaycastHit hit = CastRay(1 << 9);
        

        if (hit.collider != null)
        {
            if (focusPart != null)
            {
                LowlightPart(focusPart);
            }
            focusPart = hit.collider.gameObject;
            focusPart.GetComponent<MeshRenderer>().material = MaterialHolder.HighlightFrozenMat;
        }
        else
        {
            if (focusPart != null)
            {
                LowlightPart(focusPart);
                focusPart = null;
            }
        }
    }

    void CastChoseConnectionRay()
    {
        RaycastHit hit = CastRay(1 << 9);

        if (hit.collider != null && hit.collider.gameObject == chosenPart)
        {
            if (connectionProxies.Count <= 0)
                return;

            Vector3 hitPoint = hit.point;
            GameObject closest = connectionProxies[0];
            float bestDist = Vector3.Distance(closest.GetComponent<ConnectionContainer>().Connection.Pln.Origin, hitPoint);

            for (int i = 1; i < connectionProxies.Count; ++i)
            {
                float dist = Vector3.Distance(connectionProxies[i].GetComponent<ConnectionContainer>().Connection.Pln.Origin, hitPoint);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    closest = connectionProxies[i];
                }
            }

            if (focusConnection != null)
                focusConnection.GetComponent<SpriteRenderer>().enabled = false;

            focusConnection = closest;
            focusConnection.GetComponentInChildren<SpriteRenderer>().enabled = true;
        }
        else if (focusConnection != null)
        {
            focusConnection.GetComponentInChildren<SpriteRenderer>().enabled = false;
            focusConnection = null;
        }

        /*
        if (hit.collider != null)
        {
            if (focusConnection != null)
            {
                focusConnection.GetComponentInChildren<SpriteRenderer>().enabled = false;
            }
            focusConnection = hit.collider.gameObject;
            focusConnection.GetComponentInChildren<SpriteRenderer>().enabled = true;
        }
        else
        {
            if (focusConnection != null)
            {
                focusConnection.GetComponentInChildren<SpriteRenderer>().enabled = false;
                focusConnection = null;
            }
        }
        */
    }

    RaycastHit CastRay(int mask)
    {
        RaycastHit hit;

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

        Physics.Raycast(rayFrom, rayTo, out hit, Mathf.Infinity, mask);
            
        return hit;
    }

    void LowlightPart(GameObject go)
    {
        if (!go.GetComponent<Part>().Disabled)
            go.GetComponent<MeshRenderer>().material = MaterialHolder.FrozenMat;
        else
            go.GetComponent<MeshRenderer>().material = MaterialHolder.DisabledMat;
    }

    void FreezeObject()
    {
        if (BoltNetwork.IsRunning)
        {
            Vector3 pos = childProxies[childIndex].transform.position;
            Quaternion rot = childProxies[childIndex].transform.rotation;
            Part part = childProxies[childIndex].GetComponent<Part>();

            int id = Random.Range(int.MinValue, int.MaxValue);
            while (GlobalReferences.FrozenParts.ContainsKey(id) && id == -1)
                id = Random.Range(int.MinValue, int.MaxValue);

            Part parentPart = chosenPart.GetComponent<Part>();
            int parentID = parentPart.Connections.IndexOf(chosenConnection);

            PartTokenParent token = new PartTokenParent(part.TemplateID, id, parentPart.ID, parentID, conIndex, false);

            var spawn = SpawnFrozenBlock.Create();
            spawn.token = token;
            spawn.Position = pos;
            spawn.Rotation = rot;
            if (BoltNetwork.IsServer)
                spawn.Owner = 0;
            else
                spawn.Owner = (int)BoltNetwork.Server.ConnectionId;
            spawn.Send();
        }
        else
        {
            Part part = childProxies[childIndex].GetComponent<Part>();
            part.FreezePart();
            
            Part parentPart = chosenPart.GetComponent<Part>();

            part.Parent = parentPart.ID;
            part.ParentCon = parentPart.Connections.IndexOf(chosenConnection);
            part.ConToParent = conIndex;

            parentPart.Children.Add(part.ID);
            parentPart.ChildCons.Add(conIndex);

            Destroy(part.gameObject.GetComponent<CheckGrowCollision>());
        }
    }

    public void Left()
    {
        if (choseChild && !choseChildConnection)
        {
            if (childIndex == -1)
                return;

            childProxies[childIndex].SetActive(false);
            --childIndex;
            if (childIndex < 0)
                childIndex = childProxies.Count - 1;

            childProxies[childIndex].SetActive(true);
        }
        else if (choseChildConnection)
        {
            Part childPart = childProxies[childIndex].GetComponent<Part>();
            --conIndex;
            if (conIndex < 0)
                conIndex = childPart.ActiveConnections.Count - 1;

            if (ConnectionScanning.RuleActive(chosenConnection, childPart.Connections[childPart.ActiveConnections[conIndex]]))
            {
                bool orientCheck = false;
                for (int i = 0; i < 10; ++i)
                {
                    orientCheck = AlignPlane.Orient(childPart.Connections[childPart.ActiveConnections[conIndex]].Pln, chosenConnection.Pln, childProxies[childIndex]);
                    if (orientCheck)
                        break;
                }
                if (!orientCheck)
                    Left();
            }
            else
                Left();
        }
    }

    public void Right()
    {
        if (choseChild && !choseChildConnection)
        {
            if (childIndex == -1)
                return;

            childProxies[childIndex].SetActive(false);
            ++childIndex;
            if (childIndex >= childProxies.Count)
                childIndex = 0;

            childProxies[childIndex].SetActive(true);
            childProxies[childIndex].GetComponent<MeshRenderer>().material = MaterialHolder.SelectedMat;
        }
        else if (choseChildConnection)
        {
            Part childPart = childProxies[childIndex].GetComponent<Part>();
            ++conIndex;
            if (conIndex >= childPart.ActiveConnections.Count)
                conIndex = 0;

            if (ConnectionScanning.RuleActive(chosenConnection, childPart.Connections[childPart.ActiveConnections[conIndex]]))
            {
                bool orientCheck = false;
                for (int i = 0; i < 10; ++i)
                {
                    orientCheck = AlignPlane.Orient(childPart.Connections[childPart.ActiveConnections[conIndex]].Pln, chosenConnection.Pln, childProxies[childIndex]);
                    if (orientCheck)
                        break;
                }
                if (!orientCheck)
                    Right();
            }
            else
                Right();

            childProxies[childIndex].GetComponent<MeshRenderer>().material = MaterialHolder.SelectedMat;
        }
    }

    void ResetTool()
    {
        DestroyConnectionProxies();

        if (focusPart != null)
            LowlightPart(focusPart);

        if (chosenPart != null)
            LowlightPart(chosenPart);

        chosePart = false;
        choseConnection = false;
        choseChild = false;
        choseChildConnection = false;

        chosenPart = null;
        focusPart = null;

        focusConnection = null;
        chosenConnection = null;

        text.text = "Grow";

        childIndex = -1;
        conIndex = 0;

        DestroyChildProxies();

        PlacementReferences.Aiming = false;

        this.enabled = false;
    }

    void DestroyConnectionProxies()
    {
        for (int i = connectionProxies.Count - 1; i >= 0; --i)
            Destroy(connectionProxies[i]);

        connectionProxies.Clear();
    }

    void DestroyChildProxies()
    {
        for (int i = childProxies.Count - 1; i >= 0; --i)
        {
            Destroy(childProxies[i]);
        }

        childProxies.Clear();
    }
}
