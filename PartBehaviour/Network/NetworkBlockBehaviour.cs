using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Security.Cryptography;
using Bolt;

public class NetworkBlockBehaviour : Bolt.EntityEventListener<IAbstractBlockState>
{
    Part part;

    public override void Attached()
    {
        PartToken token = (PartToken)entity.AttachToken;

            int tempID = 0;
            int id = -1;
            Vector3 pos = Vector3.zero;
            Quaternion rot = Quaternion.identity;

            if (token != null)
            {
                tempID = token.TemplateID;
                id = token.ID;
            }

            state.SetTransforms(state.BlockTransform, transform);

            InitializeNetworkedPart(tempID, id);

    }

    public override void Detached()
    {

    }


    public Part InitializeNetworkedPart(int tempID, int id = -1)
    {
        if (GlobalReferences.TemplateParts.Count == 0)
            return null;

        GameObject template = GlobalReferences.TemplateParts[tempID];
        gameObject.AddComponent<MeshRenderer>().material = MaterialHolder.UnaffectedMat;
        gameObject.AddComponent<MeshFilter>().sharedMesh = template.GetComponent<MeshFilter>().sharedMesh;
        part = gameObject.AddComponent<Part>();

        MeshCollider[] cols = template.GetComponents<MeshCollider>();
        foreach (MeshCollider mc in cols)
        {
            MeshCollider newMc = gameObject.AddComponent<MeshCollider>();
            newMc.convex = true;
            newMc.sharedMesh = mc.sharedMesh;
        }

        PartsHolder.ResetPart(gameObject, GlobalReferences.TemplateParts[tempID], id);

        gameObject.layer = 8;

        GlobalReferences.Parts.Add(gameObject);

        GlobalReferences.FreeParts.Add(gameObject);

        return part;
    }



    //Events
    #region
    public override void OnEvent(CheckBlockFreeze evnt)
    {
        if (BoltNetwork.IsServer)
        {
            Part p = gameObject.GetComponent<Part>();
            Part parentPart = GlobalReferences.FrozenParts[evnt.ParentID].GetComponent<Part>();

            Connection bestOnPart = p.Connections[evnt.ConnectionID];
            Connection closestConnection = parentPart.Connections[evnt.ParentCon];

            gameObject.transform.position = evnt.BlockPosition;
            gameObject.transform.rotation = evnt.BlockRotation;
            //AlignPlane.Orient(bestOnPart.Pln, closestConnection.Pln, gameObject);

            if (!ConnectionScanning.CollisionDetection(gameObject) && CheckOriginAlignment(bestOnPart, closestConnection, evnt.BlockPosition, evnt.BlockRotation))// && CheckConncetionAlignment(bestOnPart, closestConnection))//, transform, parentPart.transform))
            {

                var token = new PartTokenParent();
                token.TemplateID = p.TemplateID;

                token.ID = UnityEngine.Random.Range(int.MinValue, int.MaxValue);

                while (GlobalReferences.FrozenParts.ContainsKey(token.ID))
                    token.ID = UnityEngine.Random.Range(int.MinValue, int.MaxValue);


                token.Parent = evnt.ParentID;
                token.ParentCon = evnt.ParentCon;

                token.Con = evnt.ConnectionID;

                var spawn = SpawnFrozenBlock.Create();
                spawn.token = token;
                spawn.Position = evnt.BlockPosition;
                spawn.Rotation = evnt.BlockRotation;
                spawn.Send();

                var destroy = BlockDestroy.Create(entity);
                destroy.Send();

                //Debug.Log("#43 Spawning Block: " + token.ID + ", Position = " + evnt.BlockPosition.ToString("F2") + ", Rotation = " + evnt.BlockRotation.eulerAngles.ToString("F2"));
            }
            else
            {
                var decline = DeclineBlockFreeze.Create(entity);
                decline.OldBlockPosition = evnt.OldBlockPosition;
                decline.OldBlockRotation = evnt.OldBlockRotation;

                decline.Send();
            }
        }
    }

    public static bool CheckConncetionAlignment(Connection conA, Connection conB, float epsilon = 0.0001f)
    {
        if (conB == null)
            return true;


        float angleX = Vector3.Angle(conA.Pln.XVector, conB.Pln.XVector);
        float angleY = Vector3.Angle(conA.Pln.YVector, conB.Pln.YVector);
        float dist = Vector3.Distance(conA.Pln.Origin, conB.Pln.Origin);

        if (dist < epsilon && Math.Abs(angleX) < epsilon && Math.Abs(angleY - 180) < epsilon)
        {
            return true;
        }
        else
        {
            Debug.Log("#44 Part: " + conA.ParentPart.ID + " connects with Tolerances: Distance: " + dist.ToString("F3") + ", AngleX: " + angleX.ToString("F3") + ", AngleY: " + angleY.ToString("F3"));
            return false;
        }
    }
    public static bool CheckOriginAlignment(Connection conA, Connection conB, Vector3 position, Quaternion rotation, float epsilon = 0.0001f)
    {
        if (conB == null)
            return true;


        float dist = Vector3.Distance(rotation * conA.Pln.LocalOrigin + position, conB.Pln.Origin);

        if (dist < epsilon)
        {
            return true;
        }
        else
        {
            Debug.Log("#44 OriginCheck: Would connects with Tolerances: Distance: " + dist.ToString("F3"));
            return false;
        }
    }

    public static bool CheckOriginAlignment(Connection conA, Connection conB, Vector3 positionA, Quaternion rotationA, Vector3 positionB, Quaternion rotationB, float epsilon = 0.0001f)
    {
        if (conB == null)
            return true;


        float dist = Vector3.Distance(rotationA * conA.Pln.LocalOrigin + positionA, rotationB * conB.Pln.LocalOrigin + positionB);
        float angleX = Vector3.Angle(rotationA * conA.Pln.LocalXVector, rotationB * conB.Pln.LocalXVector);
        float angleY = Vector3.Angle(rotationA * conA.Pln.LocalYVector, rotationB * conB.Pln.LocalYVector);

        if (dist < epsilon && Math.Abs(angleX) < epsilon && Math.Abs(angleY - 180) < epsilon)
        {
            return true;
        }
        else
        {
            Debug.Log("#44 OriginCheck: Would connects with Tolerances: Distance: " + dist.ToString("F3") + ", AngleX: " + angleX.ToString("F3") + ", AngleY: " + angleY.ToString("F3"));
            return false;
        }
    }

    public override void OnEvent(DeclineBlockFreeze evnt)
    {
        if (entity.IsOwner)
        {
            if (PlacementReferences.PlacementType == PlacementReferences.PlaceChoreo.Shoot)
            {
                BoltNetwork.Destroy(gameObject);
                return;
            }

            gameObject.transform.position = evnt.OldBlockPosition;
            gameObject.transform.rotation = evnt.OldBlockRotation;

            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

            if (gameObject.GetComponent<ConstantForce>() == null)
                gameObject.AddComponent<ConstantForce>();

            gameObject.layer = 8;

            if (gameObject.GetComponent<ConnectionScanning>() == null)
            {
                var scan = gameObject.AddComponent<ConnectionScanning>();
                scan.WaitSeconds = 1;
            }

            if (gameObject.GetComponent<PartBehaviour>() == null)
                gameObject.AddComponent<PartBehaviour>();
        }
    }

    public override void OnEvent(BlockFreeze evnt)
    {
        Destroy(gameObject.GetComponent<ConstantForce>());
        Part p = gameObject.GetComponent<Part>();

        gameObject.transform.position = evnt.BlockPosition;
        gameObject.transform.rotation = evnt.BlockRotation;
        
        p.FreezePart(evnt.ID);
        p.Parent = evnt.ParentID;
        p.ParentCon = evnt.ParentCon;

        Part parentPart = GlobalReferences.FrozenParts[evnt.ParentID].GetComponent<Part>();

        ConnectionVoxelContainer.RemoveConnection(parentPart.Connections[evnt.ParentCon]);
        ConnectionVoxelContainer.RemoveConnection(gameObject.GetComponent<Part>().Connections[evnt.ConnectionID]);

        parentPart.ChildCons.Add(evnt.ConnectionID);
        parentPart.SetInactive(parentPart.Connections[evnt.ParentCon]);
        p.SetInactive(p.Connections[evnt.ConnectionID]);

        parentPart.Children.Add((int)p.ID);

        gameObject.transform.position = evnt.BlockPosition;
        gameObject.transform.rotation = evnt.BlockRotation;

        if (PlacementReferences.InfiniteParts && entity.IsOwner && p.Respawn)
        {
            GameObject go = GlobalReferences.PartSpawner.SpawnPart(p.TemplateID);
            go.GetComponent<Part>().Respawn = true;

            if (PlacementReferences.PlacementType == PlacementReferences.PlaceChoreo.Choreo)
            {
                GlobalReferences.AffectPart(go);
                GlobalReferences.FreeParts.Remove(go);
            }
        }
        if (PlacementReferences.PlacementType == PlacementReferences.PlaceChoreo.Choreo && entity.IsOwner)
        {
            GlobalReferences.ChangeAffectedNumber(PlacementReferences.AffectedParts);
        }

        if (entity.IsOwner)
        {
            //entity.Freeze(true);
            
        }
    }

    public override void OnEvent(BlockDestroy evnt)
    {
        if (entity.IsOwner)
        {
            GlobalReferences.AffectedParts.Remove(gameObject);
            GlobalReferences.FreeParts.Remove(gameObject);

            Part p = GetComponent<Part>();
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

            BoltNetwork.Destroy(gameObject);
        }
    }


    public override void OnEvent(BlockDisable evnt)
    {
        part.Disable();
    }

    public override void OnEvent(BlockEnable evnt)
    {
        part.Enable();
    }

   
    #endregion
}
