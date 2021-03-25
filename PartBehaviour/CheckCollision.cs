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
using System.Security.Cryptography;
using UnityEngine;

/*
 * Checks Collision through Collision Events
 * Freezes Part when free of Collision
 */

public class CheckCollision : MonoBehaviour
{
    public Vector3 respawnPosition;
    public Quaternion respawnRotation;

    public Connection closestConnection;
    public Connection bestOnPart;

    bool reset = false;

    private void OnCollisionEnter(Collision collision)
    {
        reset = true;
    }

    private void Start()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }

    private void Update()
    {
        if (reset)
        {
            ResetPart();
            return;
        }
        RealizeConnection();
        Destroy(this);
    }


    private void RealizeConnection()
    {
        ConnectionScanningHandler handler = GetComponent<ConnectionScanningHandler>();

        if (!CollisionDetection())
        {
            GetComponent<MeshRenderer>().enabled = true;

            if (handler != null)
                handler.TerminateConnection();

            if (BoltNetwork.IsRunning)
            {
                FreezeMultiPlayer();
            }
            else
            {
                FreezeSinglePlayer();
            }
        }
        else
        {
            ResetPart();
        }
    }

    void FreezeSinglePlayer()
    {
        Part p = gameObject.GetComponent<Part>();
        p.FreezePart();

        p.Parent = closestConnection.ParentPart.ID;
        p.ParentCon = closestConnection.ParentPart.Connections.IndexOf(closestConnection);

        p.ConToParent = bestOnPart.ParentPart.Connections.IndexOf(bestOnPart);

        ConnectionVoxelContainer.RemoveConnection(closestConnection);
        ConnectionVoxelContainer.RemoveConnection(bestOnPart);

        closestConnection.ParentPart.ChildCons.Add(bestOnPart.ParentPart.Connections.IndexOf(bestOnPart));
        closestConnection.ParentPart.Children.Add((int)p.ID);


        bestOnPart.ParentPart.SetInactive(bestOnPart);
        closestConnection.ParentPart.SetInactive(closestConnection);

        if (PlacementReferences.InfiniteParts && p.Respawn)
        {
            GameObject go = GlobalReferences.PartSpawner.SpawnPart(p.TemplateID);
            go.GetComponent<Part>().Respawn = true;

            if (PlacementReferences.PlacementType == PlacementReferences.PlaceChoreo.Choreo)
            {
                GlobalReferences.AffectPart(go);
                GlobalReferences.FreeParts.Remove(go);
            }
        }
        if (PlacementReferences.PlacementType == PlacementReferences.PlaceChoreo.Choreo)
        {
            GlobalReferences.ChangeAffectedNumber(PlacementReferences.AffectedParts);
        }
    }

    void FreezeMultiPlayer()
    {
        var checkFreeze = CheckBlockFreeze.Create(gameObject.GetComponent<NetworkBlockBehaviour>().entity);
        checkFreeze.BlockPosition = gameObject.transform.position;
        checkFreeze.BlockRotation = gameObject.transform.rotation;

        checkFreeze.OldBlockPosition = respawnPosition;
        checkFreeze.OldBlockRotation = respawnRotation;

        checkFreeze.ParentID = closestConnection.ParentPart.ID;

        checkFreeze.ParentCon = closestConnection.ParentPart.Connections.IndexOf(closestConnection);
        checkFreeze.ConnectionID = gameObject.GetComponent<Part>().Connections.IndexOf(bestOnPart);

        checkFreeze.Send();
    }

    void ResetPart()
    {
        KillTimer kill = GetComponent<KillTimer>();
        if (kill != null)
        {
            kill.Kill();
        }

        GetComponent<MeshRenderer>().enabled = true;

        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

        /*
        Vector3 minS = PartsHolder.MinSpawnArea;
        Vector3 maxS = PartsHolder.MaxSpawnArea;
        gameObject.transform.position = new Vector3(Random.Range(minS.x, maxS.x), Random.Range(minS.y, maxS.y), Random.Range(minS.z, maxS.z));
        */

        gameObject.transform.position = respawnPosition;
        gameObject.transform.rotation = respawnRotation;

        ConstantForce force = gameObject.GetComponent<ConstantForce>();
        if (force == null)
            force = gameObject.AddComponent<ConstantForce>();

        gameObject.layer = 8;

        ConnectionScanning scan = gameObject.GetComponent<ConnectionScanning>();
        if (scan == null)
        {
            scan = gameObject.AddComponent<ConnectionScanning>();
            scan.WaitSeconds = 1;
        }

        PartBehaviour behaviour = gameObject.GetComponent<PartBehaviour>();
        if (behaviour == null)
            behaviour = gameObject.AddComponent<PartBehaviour>();

        if (PlacementReferences.PlacementType == PlacementReferences.PlaceChoreo.PickNChose)
        {
            scan.enabled = false;
            behaviour.enabled = false;
        }

        var handler = GetComponent<ConnectionScanningHandler>();
        if (handler != null)
        {
            handler.FailedConnection(gameObject);
        }


        Destroy(this);

    }

    private bool CollisionDetection()
    {
        bool isCollide = false;

        List<GameObject> collisionGo = new List<GameObject>();
        MeshCollider[] selfColliders = gameObject.GetComponents<MeshCollider>();


        collisionGo = CollisionVoxelContainer.RevealCloseGos(gameObject);

        foreach (GameObject closeGo in collisionGo)
        {
            if (closeGo != null)
            {
                MeshCollider[] otherColliders = closeGo.GetComponents<MeshCollider>();
                foreach (MeshCollider otherCol in otherColliders)
                {
                    foreach (MeshCollider selfCol in selfColliders)
                    {
                        Vector3 vec = Vector3.up;
                        float f = 0;
                        if (Physics.ComputePenetration(selfCol, gameObject.transform.position, gameObject.transform.rotation, otherCol, closeGo.transform.position, closeGo.transform.rotation, out vec, out f))
                        {
                            isCollide = true;
                            return isCollide;
                        }
                    }
                }
            }
        }


        if (true)
        {
            foreach (GameObject go in GlobalReferences.AdditionalGeometry)
            {
                foreach (MeshCollider mc in selfColliders)
                {
                    Vector3 vec = Vector3.up;
                    float f = 0;
                    if (go.GetComponent<MeshCollider>() != null && Physics.ComputePenetration(mc, gameObject.transform.position, gameObject.transform.rotation, go.GetComponent<MeshCollider>(), go.transform.position, go.transform.rotation, out vec, out f))
                    {
                        isCollide = true;
                        return isCollide;
                    }
                }
            }
        }


        return isCollide;
    }
}
