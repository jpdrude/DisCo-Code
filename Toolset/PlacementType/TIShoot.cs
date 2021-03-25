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
 * Toolitem: Shoot
 * 
 * Shoots parts from Controller
 * Parts snap when in proximity to aggregation
 * Parts get deleted after a short time when not aggregated
 */

public class TIShoot : ToolItem
{
    float counter;

    [SerializeField]
    float freq = 0.05f;

    [SerializeField]
    GameObject shootSpawner;

    [SerializeField]
    float speed = 20;


    bool shoot = false;

    public override void Initialize(Toolset _toolSet)
    {
        base.Initialize(_toolSet);

        counter = freq;
    }

    public override void ActivateItem()
    {
        PlacementReferences.PlacementType = PlacementReferences.PlaceChoreo.Shoot;

        if (PlacementReferences.PlayMode == PlacementReferences.Mode.FPS)
            PlacementReferences.Aiming = true;
        
        this.enabled = true;
    }

    public override void DeactivateItem()
    {
        base.DeactivateItem();

        shoot = false;
        PlacementReferences.Aiming = false;
        this.enabled = false;
    }

    public override void Click()
    {
        shoot = true;
    }

    public override void Unclick()
    {
        shoot = false;
    }

    public override void ScrollUp()
    {
        if (freq >= 0.015f)
            freq -= 0.005f;
    }

    public override void ScrollDown()
    {
        freq += 0.005f;
    }


    void Update()
    {
        KillTimer.camPos = shootSpawner.transform.position;

        counter += Time.deltaTime;

        if (shoot)
        {
            if (counter > freq)
            {

                int spawnIdx = GlobalReferences.PartProb;

                if (spawnIdx == -1)
                    return;

                GameObject go;

                if (BoltNetwork.IsRunning)
                    go =GlobalReferences.PartSpawner.SpawnNetworkPart(spawnIdx);
                else
                    go = GlobalReferences.PartSpawner.SpawnGhostPart(spawnIdx);

                go.transform.position = shootSpawner.transform.position;

                go.SetActive(true);

                go.GetComponent<ConstantForce>().force = shootSpawner.transform.forward.normalized * speed;

                go.GetComponent<ConnectionScanning>().enabled = true;

                go.AddComponent<KillTimer>();

                go.GetComponent<MeshRenderer>().enabled = false;

                Destroy(go.GetComponent<PartBehaviour>());

                go.layer = 15;

                counter = 0;

            }
        }
    }
}
