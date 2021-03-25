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
 * Toolitem: Place
 * 
 * holds instances of template Parts
 * Places an active template Part at position
 * 
 * Can switch between template Parts
 */

public class TIPlace : ToolItem
{
    private static GameObject[] carryGo;

    GameObject target = null;

    public static GameObject[] CarryGo
    {
        get { return carryGo; }
    }

    private static int currentTempID = 0;

    public override void Initialize(Toolset _toolSet)
    {
        if (target != null)
            ResetTool();

        base.Initialize(_toolSet);

        target = Instantiate(ToolSet.ControllerTarget, ToolSet.ControllerTarget.transform.position, ToolSet.ControllerTarget.transform.rotation,
                     ToolSet.ControllerTarget.transform.parent);
        target.name = "PlaceTarget";

        ScaleTool.KeepLarge.Add(target);

        carryGo = new GameObject[GlobalReferences.TemplateParts.Count];

        for (int i = 0; i < GlobalReferences.TemplateParts.Count; ++i)
        {
            SpawnObject(i);
        }

        carryGo[currentTempID].SetActive(true);
    }

    private void FreezeObject()
    {
        if (BoltNetwork.IsRunning)
        {
            Vector3 pos = carryGo[currentTempID].transform.position;
            Quaternion rot = carryGo[currentTempID].transform.rotation;
            int id = Random.Range(int.MinValue, int.MaxValue);
            while (GlobalReferences.FrozenParts.ContainsKey(id) && id == -1)
                id = Random.Range(int.MinValue, int.MaxValue);

            PartToken token = new PartToken(currentTempID, id, false);

            var spawn = SpawnFrozenBlock.Create();
            spawn.token = token;
            spawn.Position = pos;
            spawn.Rotation = rot;
            if (BoltNetwork.IsServer)
                spawn.Owner = 0;
            else
                spawn.Owner = (int)BoltNetwork.Server.ConnectionId;
            spawn.Send();

            //Debug.Log("#43 Spawning Block: " + token.ID + ", Position = " + pos.ToString("F2") + ", Rotation = " + rot.eulerAngles.ToString("F2"));
        }
        else
        {
            Vector3 pos = carryGo[currentTempID].transform.localPosition;
            carryGo[currentTempID].transform.parent = null;
            carryGo[currentTempID].GetComponent<Part>().FreezePart();
            carryGo[currentTempID].GetComponent<Rigidbody>().isKinematic = false;

            SpawnObject(currentTempID);
            carryGo[currentTempID].transform.localPosition = pos;
            carryGo[currentTempID].SetActive(true);
        }
    }

    private void SpawnObject(int id)
    {
        GameObject go = Instantiate(GlobalReferences.TemplateParts[id]);
        PartsHolder.ResetPart(go, GlobalReferences.TemplateParts[id], -1);

        carryGo[id] = go;
        go.GetComponent<MeshRenderer>().material = MaterialHolder.FrozenMat;
        go.GetComponent<Rigidbody>().isKinematic = true;
        go.transform.parent = target.transform;
        go.SetActive(false);
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
    }

    public override void ActivateItem()
    {
        PlacementReferences.PlacementType = PlacementReferences.PlaceChoreo.Place;
        carryGo[currentTempID].SetActive(true);
    }

    public override void DeactivateItem()
    {
        base.DeactivateItem();
        
        carryGo[currentTempID].SetActive(false);
    }

    public override void Click()
    {
        FreezeObject();
    }

    public override void NextOption()
    {
        carryGo[currentTempID].SetActive(false);

        ++currentTempID;

        if (currentTempID >= carryGo.Length)
            currentTempID = 0;

        carryGo[currentTempID].SetActive(true);
    }

    public override void PrevOption()
    {
        carryGo[currentTempID].SetActive(false);

        --currentTempID;

        if (currentTempID < 0)
            currentTempID = carryGo.Length - 1;

        carryGo[currentTempID].SetActive(true);
    }

    public override void ScrollUp()
    {
       target.transform.localPosition += Vector3.forward * Time.deltaTime;
    }

    public override void ScrollDown()
    {
        Vector3 newPos = target.transform.localPosition + Vector3.back * Time.deltaTime;
        if (newPos.z > 0.05f)
        {
            target.transform.localPosition = newPos;
        }
    }

    void ResetTool()
    {
        ScaleTool.KeepLarge.Remove(target);

        Destroy(target);

        for (int i = carryGo.Length - 1; i >= 0; --i)
        {
            Destroy(carryGo[i]);
        }
    }
}
