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
using System.Linq;

/*
 * Toolitem: Simulate
 * 
 * performs simulation of aggregation
 * (lets it fall to the ground)
 */

public class TISimulate : ToolItem
{
    bool simulationHappened = false;

    TextMeshPro textMesh;
    string caption;

    HashSet<int> idLedger = new HashSet<int>();

    public override void Initialize(Toolset _toolSet)
    {
        base.Initialize(_toolSet);

        textMesh = GetComponent<TextMeshPro>();

        caption = textMesh.text;
    }

    public override void ActivateItem()
    {
        if (!SimulateStatics.SimulationInProgress && !SimulateStatics.SimulationDone)
        {
            idLedger.Clear();
            foreach (int id in GlobalReferences.FrozenParts.Keys)
                idLedger.Add(id);

            simulationHappened = true;

            textMesh.text = "Stop!";

            SimulateStatics.ParentAndSimulate();
            SimulateStatics.SimulationInProgress = true;
        }
        else if (SimulateStatics.SimulationInProgress && !SimulateStatics.SimulationDone)
        {
            textMesh.text = "Reset";
            SimulateStatics.AfterSimulation();
            SimulateStatics.SimulationDone = true;
        }
        else
        {
            SimulateStatics.ResetSimulation();
            ResetTool();
        }
    }

    public override void UnfocusItem()
    {
        if (simulationHappened && BoltNetwork.IsRunning)
        {
            foreach (KeyValuePair<int, GameObject> frozen in GlobalReferences.FrozenParts)
            {
                if (!idLedger.Contains(frozen.Key))
                    frozen.Value.GetComponent<Part>().Delete();
            }

            new LoadAggregationContainer(GlobalReferences.FrozenParts.Values.ToList<GameObject>()).StreamData(false, NetworkCallbacks.UpdateAggregationChannel);
        }
        ResetTool();
    }

    public override void DeactivateItem()
    {
        UnfocusItem();
    }

    void ResetTool()
    {
        SimulateStatics.AfterSimulation();
        textMesh.text = caption;
        simulationHappened = false;
        idLedger.Clear();
    }
}
