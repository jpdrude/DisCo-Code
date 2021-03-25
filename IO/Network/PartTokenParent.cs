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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt.Utils;

/*
 * Part Token to send Parts through Network
 * Is used for parts with parents
 */

public class PartTokenParent : IProtocolToken
{
    public int TemplateID { get; set; }

    public int ID { get; set; }

    public int Parent { get; set; }

    public int ParentCon { get; set; }

    public int Con { get; set; }

    public bool Disabled { get; set; }

    public PartTokenParent()
    {

    }

    public PartTokenParent(int _templateID, int _id, int _parent, int _parentCon, int _con, bool _disabled)
    {
        TemplateID = _templateID;
        ID = _id;
        Parent = _parent;
        ParentCon = _parentCon;
        Con = _con;
        Disabled = _disabled;
    }

    public PartTokenParent(Part part, int id = -1)
    {
        TemplateID = part.TemplateID;

        if (id == -1)
            ID = part.ID;
        else
            ID = id;

        Parent = part.Parent;
        ParentCon = part.ParentCon;
        Con = part.ConToParent;
        Disabled = part.Disabled;
    }

    public void Read(UdpKit.UdpPacket packet)
    {
        TemplateID = packet.ReadInt();
        ID = packet.ReadInt();
        Parent = packet.ReadInt();
        ParentCon = packet.ReadInt();
        Con = packet.ReadInt();
        Disabled = packet.ReadBool();
        //Position = packet.ReadVector3();
        //Rotation = packet.ReadQuaternion();
    }

    public void Write(UdpKit.UdpPacket packet)
    {
        packet.WriteInt(TemplateID);
        packet.WriteInt(ID);
        packet.WriteInt(Parent);
        packet.WriteInt(ParentCon);
        packet.WriteInt(Con);
        packet.WriteBool(Disabled);
        //packet.WriteVector3(Position);
        //packet.WriteQuaternion(Rotation);
    }
}
