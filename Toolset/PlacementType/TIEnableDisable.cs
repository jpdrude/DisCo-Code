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
 * Toolitem: Enable/Disable
 * 
 * uses DisableSphere to enable/disable Parts for further aggregation
 */

public class TIEnableDisable : ToolItem
{
    [SerializeField]
    GameObject sphere;

    [SerializeField]
    GameObject spherePos = null;

    DisableSphere disableSphere = null;

    public override void Initialize(Toolset _toolSet)
    {
        if (disableSphere != null)
            Destroy(disableSphere.gameObject);

        base.Initialize(_toolSet);

        GameObject go;
        if (spherePos == null)
        {
            go = Instantiate(sphere, ToolSet.transform);
            disableSphere = go.AddComponent<DisableSphere>();
            disableSphere.Initialize(0.5f, MaterialHolder.DisableSphereMat, ToolSet.ToolTopPos, new Vector3(0, 0, -1));
        }
        else
        {
            go = Instantiate(sphere, spherePos.transform);
            disableSphere = go.AddComponent<DisableSphere>();
            disableSphere.Initialize(0.5f, MaterialHolder.DisableSphereMat, Vector3.forward * 0.05f, Vector3.forward);
        }

        go.name = "DisableSphere";

        disableSphere.gameObject.SetActive(false);
    }

    public override void ActivateItem()
    {
        disableSphere.gameObject.SetActive(true);
    }

    public override void DeactivateItem()
    {
        base.DeactivateItem();

        disableSphere.gameObject.SetActive(false);
    }

    public override void Click()
    {
        disableSphere.Click();
    }

    public override void Unclick()
    {
        disableSphere.Unclick();
    }

    public override void NextOption()
    {
        disableSphere.ChangeMode();
    }

    public override void PrevOption()
    {
        disableSphere.ChangeMode();
    }

    public override void ScrollUp()
    {
        disableSphere.ScrollUp();
    }

    public override void ScrollDown()
    {
        disableSphere.ScrollDown();
    }
}
