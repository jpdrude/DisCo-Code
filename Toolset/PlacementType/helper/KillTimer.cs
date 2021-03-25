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
 * Deletes Shoot Parts after certain time.
 * Sets visibility of Shoot parts after certain distance to player
 */

public class KillTimer : MonoBehaviour
{
    public static float killTime = 2;

    float livedTime;

    public static Vector3 camPos;

    private static float visDist = 1;
    public static float VisDist { get { return visDist; } set { visDist = value; } }


    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(camPos, transform.position) > visDist)
        {
            GetComponent<MeshRenderer>().enabled = true;
        }

        if (livedTime > killTime)
        {
            Kill();
        }
        livedTime += Time.deltaTime;
    }

    public void Kill()
    {
        if (BoltNetwork.IsRunning)
        {
            BoltEntity entity = GetComponent<NetworkBlockBehaviour>().entity;

            if (entity != null && entity.IsOwner)
                BoltNetwork.Destroy(entity);

            return;
        }
        else
        {

            try
            {
                GlobalReferences.Parts.Remove(gameObject);
                GlobalReferences.FreeParts.Remove(gameObject);
            }
            catch
            {
                throw new System.Exception("Part not Containes in global Collections");
            }

            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        GetComponent<MeshRenderer>().enabled = true;
    }
}
