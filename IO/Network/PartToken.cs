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
using Bolt.Utils;
using System.Runtime.CompilerServices;
using UnityEngine;

/*
 * Simple Part Token to send Parts through Network
 */

public class PartToken : IProtocolToken
{
    public int TemplateID { get; set; }
    public int ID { get; set; }

    public bool Disabled { get; set; }

    public PartToken()
    {
        ID = -1;
    }

    public PartToken(int templateID, int id, bool disabled)
    {
        TemplateID = templateID;
        ID = id;
        Disabled = disabled;
    }

    public PartToken(Part part)
    {
        TemplateID = part.TemplateID;
        ID = part.ID;
        Disabled = part.Disabled;
    }

    public void Read(UdpKit.UdpPacket packet)
    {
        TemplateID = packet.ReadInt();
        ID = packet.ReadInt();
        Disabled = packet.ReadBool();
    }

    public void Write(UdpKit.UdpPacket packet)
    {
        packet.WriteInt(TemplateID);
        packet.WriteInt(ID);
        packet.WriteBool(Disabled);
    }
}
