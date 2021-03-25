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

using Bolt;
using ExitGames.Client.Photon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Xml.Schema;
using UnityEngine;
using Valve.VR;

/*
 * Keeps all clients in synch through events:
 * 
 *      - Spawn Frozen Part
 *      - Delete Part
 *      - Enable/Disable Part
 *      - Change Part Filter
 *      - Change Rule Filter
 *      - Player Joined
 *      - Controller Spawned
 *      - Ctrl-Z
 */

public class NetworkPartEventsListener : Bolt.GlobalEventListener
{
    static Dictionary<string, TIRuleFilter> ruleFilterTIs = new Dictionary<string, TIRuleFilter>();
    public static Dictionary<string, TIRuleFilter> RuleFilterTIs { get { return ruleFilterTIs; } set { ruleFilterTIs = value; } }

    static Dictionary<int, TIPartTypeFilter> partFilterTIs = new Dictionary<int, TIPartTypeFilter>();
    public static Dictionary<int, TIPartTypeFilter> PartFilterTIs { get { return partFilterTIs; } set { partFilterTIs = value; } }

    public override void OnEvent(ChangeRuleFilter evnt)
    {
        foreach (int[] gramm in GlobalReferences.RuleGroups[evnt.GroupKey])
        {
            GlobalReferences.RuleMatrix[gramm[0], gramm[1]] = evnt.State;
        }

        if (ruleFilterTIs.ContainsKey(evnt.GroupKey))
        {
            ruleFilterTIs[evnt.GroupKey].ChangeState(evnt.State);
        }

    }

    public override void OnEvent(ChangePartFilter evnt)
    {
        if (evnt.State)
        {
            GlobalReferences.TypeFilter.Add(evnt.ID, GlobalReferences.PartProbs[evnt.ID]);
        }
        else
        {
            GlobalReferences.TypeFilter.Remove(evnt.ID);
        }

        GlobalReferences.ClearAffectedList();
        if (PlacementReferences.PlacementType == PlacementReferences.PlaceChoreo.Choreo)
        {
            GlobalReferences.ChangeAffectedNumber(PlacementReferences.AffectedParts);
        }

        partFilterTIs[evnt.ID].ChangeState(evnt.State);
    }

    public override void OnEvent(SpawnFrozenBlock evnt)
    {
        IProtocolToken rawToken = evnt.token;
        Vector3 pos = evnt.Position;
        Quaternion rot = evnt.Rotation;

        NetworkPartSpawner.Data.Add(new PartSpawnData(rawToken, evnt.Position, evnt.Rotation, evnt.Owner));

    }

    public override void OnEvent(DeleteFrozenBlock evnt)
    {
        Part delPart = GlobalReferences.FrozenParts[evnt.ID].GetComponent<Part>();
        if (delPart != null)
            delPart.LocalDelete();
        else
            Debug.LogError("Delete Part: " + evnt.ID + "not present in Frozen Parts"); 
    }

    public override void OnEvent(EnableDisableFrozenBlock evnt)
    {
        Part part = GlobalReferences.FrozenParts[evnt.ID].GetComponent<Part>();

        if (part == null)
            return;

        if (evnt.Enable == true)
            part.Enable();
        else
            part.Disable();
    }

    public override void OnEvent(PlayerJoined evnt)
    {
        if (!BoltNetwork.IsServer)
            return;

        LoadAggregationContainer loadData = new LoadAggregationContainer(GlobalReferences.FrozenParts.Values.ToList<GameObject>());

        loadData.data.AddRange(NetworkPartSpawner.LoadData.data);

        loadData.StreamData(false, NetworkCallbacks.StreamAggregationChannel, evnt.RaisedBy);
    }

    public override void OnEvent(ControllerSpawned evnt)
    {
        if (BoltNetwork.IsServer && !ControllerReferences.IndependantMP)
        {
            foreach(TIRuleFilter ruleFilter in ruleFilterTIs.Values)
            {
                if (!ruleFilter.Active)
                {
                    var changeRuleFilter = ChangeRuleFilter.Create(evnt.RaisedBy);
                    changeRuleFilter.GroupKey = ruleFilter.GroupKey;
                    changeRuleFilter.State = false;
                    changeRuleFilter.Send();
                }
            }

            foreach(TIPartTypeFilter partFilter in partFilterTIs.Values)
            {
                if (!partFilter.Active)
                {
                    var changePartFilter = ChangePartFilter.Create(evnt.RaisedBy);
                    changePartFilter.State = false;
                    changePartFilter.ID = partFilter.TemplateID;
                    changePartFilter.Send();
                }
            }
        }
    }

    public override void OnEvent(BlockCtrlZ evnt)
    {
        if (BoltNetwork.IsServer)
        {
            TICtrlZ.History.Pop().CtrlZ();
        }
    }
}
