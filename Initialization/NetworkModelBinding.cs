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
using TMPro;

/*
 * Binding for Network Player Model
 */

public class NetworkModelBinding : Bolt.EntityBehaviour<IPlayerModelState>
{
    [SerializeField]
    List<TextMeshPro> nameFields;

    private GameObject controller;
    public GameObject Controller
    {
        get { return controller; }
        set { controller = value; }
    }

    public string MultiplayerName
    {
        get
        {
            return state.Name;
        }
        set
        {
            state.Name = value;
        }
    }

    private void Start()
    {
        state.AddCallback("Name", OnNameChanged);
        OnNameChanged();
    }

    public override void SimulateOwner()
    {
        transform.position = controller.transform.position;
        transform.rotation = controller.transform.rotation;
        state.PlayerScale = ControllerReferences.Controller.transform.localScale;
    }

    private void Update()
    {
        transform.localScale = state.PlayerScale;
    }

    public void OnNameChanged()
    {
        foreach (TextMeshPro name in nameFields)
        {
            name.text = MultiplayerName;
            name.faceColor = MaterialHolder.AffectedColor;
        }
    }
}
