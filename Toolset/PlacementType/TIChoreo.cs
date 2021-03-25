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
 * Toolitem: Choreography
 * 
 * makes free parts responsive to controller position/rotation
 * choreographed parts will look for open connections and aggregate
 */

public class TIChoreo : ToolItem
{
    public override void ActivateItem()
    {
        PlacementReferences.PlacementType = PlacementReferences.PlaceChoreo.Choreo;
        GlobalReferences.ChangeAffectedNumber(PlacementReferences.AffectedParts);
    }

    public override void DeactivateItem()
    {
        base.DeactivateItem();
        GlobalReferences.ClearAffectedList();
    }

    public override void Click()
    {
        GlobalReferences.DriftApartFac = 1;
    }

    public override void Unclick()
    {
        GlobalReferences.DriftApartFac = 0;
    }

    public override void ScrollUp()
    {
        ToolSet.ControllerTarget.transform.localPosition += Vector3.forward * Time.deltaTime;
        ToolSet.ControllerTarget.GetComponent<ControllerTarget>().Highlight();
    }

    public override void ScrollDown()
    {
        Vector3 newPos = ToolSet.ControllerTarget.transform.localPosition - Vector3.forward * Time.deltaTime;
        if (newPos.z > 0.05f)
        {
            ToolSet.ControllerTarget.transform.localPosition = newPos;
        }

        ToolSet.ControllerTarget.GetComponent<ControllerTarget>().Highlight();
    }

    public override void Disable()
    {
        //DeleteFreeParts();
    }

    
    void DeleteFreeParts()
    {
        //GlobalReferences.NumOfParts = 0;

        for (int i = GlobalReferences.FreeParts.Count - 1; i >= 0; --i)
        {
            GameObject go = GlobalReferences.FreeParts[i];

            if (BoltNetwork.IsRunning)
            {
                BlockDestroy destroy = BlockDestroy.Create(go.GetComponent<NetworkBlockBehaviour>().entity);
                destroy.destroyWithoutRespawn = true;
                destroy.Send();
            }
            else
            {
                if (GlobalReferences.Parts.Contains(go))
                    GlobalReferences.Parts.Remove(go);

                if (GlobalReferences.FreeParts.Contains(go))
                    GlobalReferences.FreeParts.Remove(go);
                Destroy(go);
            }
        }
    }
}
