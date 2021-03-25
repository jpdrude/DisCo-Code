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
using Bolt;
using System.Text;
using UnityEngine.UIElements;

/*
 * Game Events listener
 *      - New Game
 *      - Simulation  
 */

public class NetworkEventListener : GlobalEventListener
{
    public override void OnEvent(NewGameEvent evnt)
    {
        List<GameObject> tempGos = new List<GameObject>();
        tempGos.AddRange(GlobalReferences.FreeParts);
        tempGos.AddRange(GlobalReferences.AffectedParts);

        for (int i = tempGos.Count - 1; i >= 0; --i)
        {
            var entity = tempGos[i].GetComponent<NetworkBlockBehaviour>().entity;

            if (entity.IsAttached && entity.IsOwner)
            {
                BoltNetwork.Destroy(tempGos[i]);
            }
        }

        foreach (GameObject go in GlobalReferences.FrozenParts.Values)
        {
            Destroy(go);
        }



        GlobalReferences.FreeParts.Clear();
        GlobalReferences.FrozenParts.Clear();
        GlobalReferences.AffectedParts.Clear();
        GlobalReferences.NumOfParts = 0;
        GlobalReferences.Parts.Clear();
        GlobalReferences.PartIDLedger.Clear();

        GlobalReferences.PartSpawner.SpawnMultiple(PartsHolder.NumParts);
    }

    public override void OnEvent(ChangeNumOfParts evnt)
    {
        GlobalReferences.NumOfParts = evnt.Number;
    }

    public override void OnEvent(StartSimulation evnt)
    {
        PlacementReferences.Scanning = false;
    }

    public override void OnEvent(EndSimulation evnt)
    {
        if (!evnt.FromSelf)
        {
            ConnectionVoxelContainer.RemoveAllConnections();
            CollisionVoxelContainer.RemoveAllGos();

            foreach (GameObject go in GlobalReferences.FrozenParts.Values)
            {
                Part part = go.GetComponent<Part>();

                CollisionVoxelContainer.StoreGameObject(go);

                foreach (int i in part.ActiveConnections)
                {
                    ConnectionVoxelContainer.StoreConnection(part.Connections[i]);
                }
            }
        }
        PlacementReferences.Scanning = true;
    }


    void ResetAllParts()
    {
        foreach(GameObject go in GlobalReferences.FrozenParts.Values)
        {

            Part part = go.GetComponent<Part>();

            int templateId = part.TemplateID;
            int id = (int)part.ID;
            Vector3 scale = go.transform.localScale;
            Vector3 pos = go.transform.position;
            Quaternion rot = go.transform.rotation;

            var token = new PartTokenComplex();
            token.TemplateID = templateId;
            token.ID = id;
            //token.Position = pos;
            //token.Rotation = rot;
            //token.Scale = scale;

            if (part.Parent != -1)
            {
                token.Parent = part.Parent;
            }
            else
            {
                token.Parent = -1;
            }

            if (part.ParentCon != -1)
            {
                token.ParentCon = part.ParentCon;
            }
            else
            {
                token.ParentCon = -1;
            }

            token.ActiveCons = part.ActiveConnections;
            //token.Children = part.Children;
            //token.ChildCons = part.ChildCons;

            var compare = BlockCompare.Create();
            compare.CompareToken = token;
            compare.BlockEntity = go.GetComponent<NetworkBlockBehaviour>().entity;
            compare.BlockPosition = go.transform.position;
            compare.BlockRotation = go.transform.rotation;
            compare.Send();
        }

        var changeNumParts = ChangeNumOfParts.Create();
        changeNumParts.Number = GlobalReferences.NumOfParts;
        changeNumParts.Send();

    }

    public override void OnEvent(BlockCompare evnt)
    {
        if (BoltNetwork.IsClient)
        {

            PartTokenComplex token = (PartTokenComplex)evnt.CompareToken;
            GameObject go = evnt.BlockEntity.gameObject;
            NetworkBlockBehaviour behaviour = go.GetComponent<NetworkBlockBehaviour>();
            go.transform.position = evnt.BlockPosition;
            go.transform.rotation = evnt.BlockRotation;

            int tempID = token.TemplateID;

            int id = token.ID;

            int parent = token.Parent;

            int parentCon = token.ParentCon;

            List<int> activeCons = token.ActiveCons;

            behaviour.state.SetTransforms(behaviour.state.BlockTransform, transform);

            Part part = go.GetComponent<Part>();

            if (part == null)
            {
                part = behaviour.InitializeNetworkedPart(tempID, id);
            }

            if (part.Parent == -1 && parent != -1)
            {
                part.Parent = parent;
                part.ParentCon = parentCon;
            }

            if (activeCons.Count > 0)
                part.ActiveConnections = activeCons;


            if (id != -1 && id != part.ID && GlobalReferences.FrozenParts.ContainsKey(id))
                part.ChangeID(id);
        }
    }
}
