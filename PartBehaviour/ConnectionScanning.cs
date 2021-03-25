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
using Unity.Jobs;
using Unity.Collections;
using UnityEngine.UIElements;

/*
 * Scans for open connections in Choreo, PichNChose and Shoot
 * Adds CheckCollision if Conncetion is allowed
 */

public class ConnectionScanning : MonoBehaviour
{
    //private variables
    #region
    List<Connection> connections;
    List<Connection> foundC = new List<Connection>();

    Connection closestConnection = null;
    Connection bestOnPart = null;

    float angle = 1000;
    float rot = 1000;

    float lastDistAngle = 10000;
    #endregion


    //properties
    #region
    static float connectionThreshold = 0.1f;
    public static float ConnectionThreshold
    {
        get { return connectionThreshold; }
        set { connectionThreshold = value; }
    }

    float distAngle = 0;
    public float DistAngle
    {
        get { return distAngle; }
    }

    static float angleTighteningFactor = 1.15f;
    static public float AngleTighteningFactor
    {
        get { return angleTighteningFactor; }
        set { angleTighteningFactor = value; }
    }

    float waitSeconds = 0;
    public float WaitSeconds
    {
        get { return waitSeconds; }
        set { waitSeconds = value; }
    }
    #endregion

    //MonoBehaviour methods
    #region
    void Start()
    {
        connections = gameObject.GetComponent<Part>().Connections;
    }


    void Update()
    {
        if (!PlacementReferences.Scanning)
            return;

        if (waitSeconds > 0)
        {
            WaitSeconds -= Time.deltaTime;
            return;
        }


        angle = float.MaxValue;
        lastDistAngle = float.MaxValue;

        CheckForCloseConnections();

        RealizeConnection();
    }
    #endregion

    //methods
    #region
    private void CheckForCloseConnections()
    {
        distAngle = float.MaxValue;
        lastDistAngle = float.MaxValue;
        rot = float.MaxValue;

        foreach (Connection c in connections)
        {
            foundC = ConnectionVoxelContainer.RevealConnections(c);

            if (foundC.Count == 0)
                return;

            //if (CheckWithJobs(c, foundC)) return;

            if (CheckWithoutJobs(c, foundC)) return;

            //if (CheckWithJobsAngle(c, foundC)) return;
        }
    }

   
    bool CheckWithoutJobs(Connection c, List<Connection> foundC)
    {
        if (foundC.Count > 0)
        {
            foreach (Connection _c in foundC)
            {
                if (_c.Pln.Parent != null && RuleActive(_c, c))
                {
                    distAngle = Vector3.Distance(c.Pln.Origin, _c.Pln.Origin);
                    if (distAngle > lastDistAngle || distAngle > connectionThreshold)
                        continue;

                    angle = AlignPlane.BuildAngle(c.Pln.ZVector, _c.Pln.ZVector, true);
                    distAngle += AngleTightening(angle);
                    if (distAngle > lastDistAngle || distAngle > connectionThreshold)
                        continue;

                    rot = AlignPlane.BuildAngle(c.Pln.XVector, _c.Pln.XVector, false) * 10 * connectionThreshold;
                    distAngle += rot / 1000 * connectionThreshold;


                    if (distAngle < lastDistAngle && _c.CheckForRule(c) != -1)
                    {
                        closestConnection = _c;
                        bestOnPart = c;
                        lastDistAngle = distAngle;
                        if (lastDistAngle < connectionThreshold / 5)
                        {
                            return true;
                        }
                    }
                }
            }

        }
        return false;
    }

   
    private void RealizeConnection()
    {
        if (lastDistAngle < connectionThreshold && ConnectionVoxelContainer.RevealConnections(bestOnPart).Contains(closestConnection))
        {
            Vector3 pos = gameObject.transform.position;
            Quaternion rot = gameObject.transform.rotation;

            if (!AlignPlane.Orient(bestOnPart.Pln, closestConnection.Pln, gameObject))
            {
                gameObject.transform.position = pos;
                gameObject.transform.rotation = rot;
                return;
            }

            //PlacementBehaviour.ReleasePart(true);


            Destroy(gameObject.GetComponent<ConstantForce>());
            Destroy(gameObject.GetComponent<PartBehaviour>());
            Destroy(gameObject.GetComponent<ConnectionScanning>());

            gameObject.layer = 13;


            var check = gameObject.AddComponent<CheckCollision>();
            check.respawnPosition = pos;
            check.respawnRotation = rot;
            check.closestConnection = closestConnection;
            check.bestOnPart = bestOnPart;

            if (transform.parent != null)
                transform.parent = null;

            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;
            Destroy(this);

            /*
            if (BoltNetwork.IsRunning)
            {
                var checkFreeze = CheckBlockFreeze.Create(gameObject.GetComponent<NetworkBlockBehaviour>().entity);
                checkFreeze.BlockPosition = gameObject.transform.position;
                checkFreeze.BlockRotation = gameObject.transform.rotation;

                checkFreeze.OldBlockPosition = pos;
                checkFreeze.OldBlockRotation = rot;

                if (closestConnection.ParentPart.ID != null)
                {
                    checkFreeze.ParentID = (int)closestConnection.ParentPart.ID;
                }
                else
                {
                    checkFreeze.ParentID = -1;
                }

                checkFreeze.ParentCon = closestConnection.ParentPart.Connections.IndexOf(closestConnection);
                checkFreeze.ConnectionID = gameObject.GetComponent<Part>().Connections.IndexOf(bestOnPart);

                checkFreeze.Send();
            }
            */
        }
    }

    public static bool CollisionDetection(GameObject collider)
    {
        bool isCollide = false;
        List<GameObject> collisionGo = new List<GameObject>();
        MeshCollider[] selfColliders = collider.GetComponents<MeshCollider>();
        collisionGo = CollisionVoxelContainer.RevealCloseGos(collider);

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
                        if (Physics.ComputePenetration(selfCol, collider.transform.position, collider.transform.rotation, otherCol, closeGo.transform.position, closeGo.transform.rotation, out vec, out f))
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
                    if (go.GetComponent<MeshCollider>() != null && Physics.ComputePenetration(mc, collider.transform.position, collider.transform.rotation, go.GetComponent<MeshCollider>(), go.transform.position, go.transform.rotation, out vec, out f))
                    {
                        isCollide = true;
                        return isCollide;
                    }
                }
            }
        }

        return isCollide;
    }

    public static bool RuleActive(Connection row, Connection column)
    {
        return GlobalReferences.RuleMatrix[row.MatrixId, column.MatrixId];
    }

    
    private float AngleTightening(float angle)
    {
        return 0.01f * (float)Math.Pow(1.15, (angle - 15));
    }
    #endregion
}
