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
using System;
using System.Collections.Generic;
using UnityEngine;

/*
 * Complex Part Token to send Parts through Network
 * Handles also children and active connections
 * 
 * Currently unused (too bulky)
 */

public class PartTokenComplex : IProtocolToken
{
    public int TemplateID { get; set; }

    //public bool Free { get; set; }

    //public bool Freeze { get; set; }

    public int ID { get; set; }

    public int Parent { get; set; }

    public int ParentCon { get; set; }

    //public Vector3 Position { get; set; }

    //public Quaternion Rotation { get; set; }

    //public Vector3 Scale { get; set; }

    public List<int> ActiveCons { get; set; }

    public List<int> Children { get; set; }

    public List<int> ChildCons { get; set; }


    public void Read(UdpKit.UdpPacket packet)
    {
        TemplateID = packet.ReadInt();
        //Free = packet.ReadBool();
        //Freeze = packet.ReadBool();
        ID = packet.ReadInt();
        Parent = packet.ReadInt();
        ParentCon = packet.ReadInt();
        //Position = packet.ReadVector3();
        //Rotation = packet.ReadQuaternion();
        //Scale = packet.ReadVector3();
        ActiveCons = ByteArrayToList(packet.ReadByteArray(64));
        Children = ByteArrayToList(packet.ReadByteArray(64));
        ChildCons = ByteArrayToList(packet.ReadByteArray(64));
    }

    public void Write(UdpKit.UdpPacket packet)
    {
        packet.WriteInt(TemplateID);
        //packet.WriteBool(Free);
        //packet.WriteBool(Freeze);
        packet.WriteInt(ID);
        packet.WriteInt(Parent);
        packet.WriteInt(ParentCon);
        //packet.WriteVector3(Position);
        //packet.WriteQuaternion(Rotation);
        //packet.WriteVector3(Scale);
        packet.WriteByteArray(ListToByteArray(64, ActiveCons)); //was 255
        packet.WriteByteArray(ListToByteArray(64, Children));
        packet.WriteByteArray(ListToByteArray(64, ChildCons));
    }


    byte[] ListToByteArray(int size, List<int> list)
    {
        byte[] byteArray = new byte[size];

        if (list.Count > size)
        {
            byteArray[0] = 255;
            return byteArray;
        }

        for (int i = 0; i < size; ++i)
        {
            if (list.Count > i && list[i] < 254)
            {
                byteArray[i] = BitConverter.GetBytes(list[i])[0];
            }
            else
            {
                byteArray[i] = 254;
            }
        }

        return byteArray;
    }

    List<int> ByteArrayToList(byte[] byteArray)
    {
        List<int> list = new List<int>();

        if (byteArray[0] == 255)
            return null;

        {
            for (int i = 0; i < byteArray.Length; ++i)
            {
                if (byteArray[i] != 254)
                {
                    list.Add(byteArray[i]);
                }
                else
                {
                    break;
                }
            }
        }

        return list;
    }
}
