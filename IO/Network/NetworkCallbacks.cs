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
using UdpKit;
using UnityEngine;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;
using System;
using UnityEngine.SceneManagement;

/*
 * Sets up and handles streams between clients and servers
 * Starts client initialization after receiving init streams
 * 
 * Loads Multiplayer Load Files from other palyers
 */

[BoltGlobalBehaviour]
public class NetworkCallbacks : Bolt.GlobalEventListener
{
    private const string InitJsonChannelName = "InitJsonChannel";
    private const string WaspJsonChannelName = "WaspJsonChannel";
    private const string PlayerJsonChannelName = "PlayerJsonChannel";
    private const string StreamAggregationName = "StreamAggregationChannel";
    private const string UpdateAggregationName = "UpdateAggregationChannel";

    private const string playScene = "DisCo-Scene";

    public static UdpChannelName InitJsonChannel;
    public static UdpChannelName WaspJsonChannel;
    public static UdpChannelName PlayerJsonChannel;
    public static UdpChannelName StreamAggregationChannel;
    public static UdpChannelName UpdateAggregationChannel;

    bool InitJsonReceived = false;
    bool WaspJsonReceived = false;
    bool PlayerJsonReceived = false;

    byte[] waspJsonData;
    byte[] initJsonData;
    byte[] playerJsonData;


    byte[] waspData;
    byte[] initData;
    byte[] playerData;

    SliderProgress progressBars;

    InitializeLauncher initLauncher;

    GameObject playerSetup;

    GameObject changePlayer;


    public override void BoltStartBegin()
    {
        InitJsonChannel = BoltNetwork.CreateStreamChannel(InitJsonChannelName, UdpChannelMode.Reliable, 4);
        WaspJsonChannel = BoltNetwork.CreateStreamChannel(WaspJsonChannelName, UdpChannelMode.Reliable, 4);
        PlayerJsonChannel = BoltNetwork.CreateStreamChannel(PlayerJsonChannelName, UdpChannelMode.Reliable, 4);
        StreamAggregationChannel = BoltNetwork.CreateStreamChannel(StreamAggregationName, UdpChannelMode.Reliable, 4);
        UpdateAggregationChannel = BoltNetwork.CreateStreamChannel(UpdateAggregationName, UdpChannelMode.Reliable, 4);


        BoltNetwork.RegisterTokenClass<PartToken>();
        BoltNetwork.RegisterTokenClass<PartTokenParent>();
        BoltNetwork.RegisterTokenClass<PartTokenComplex>();
    }

    public override void SceneLoadLocalDone(string scene)
    {
        //GameObject.Find("GameController").GetComponent<InitializeLauncher>().Initialize((NetworkGameInfo)token);
        if (scene != playScene)
            return;

        PlacementReferences.InfiniteParts = true;

        var variables = GameObject.Find("Network").GetComponent<CallbackVariables>();

        progressBars = variables.ProgressBars;

        initLauncher = variables.InitLauncher;

        playerSetup = variables.PlayerSetup;

        changePlayer = variables.ChangePlayer;

        if (BoltNetwork.IsServer)
        {
            string path = Application.dataPath + "/Resources/config.json";

            LoadServerConfig(path, out waspData, out initData, out playerData);

            if (playerData.Length == 1)
                initLauncher.Initialize();
            else
            {
                initLauncher.Initialize(playerData);
                playerSetup.SetActive(false);
                changePlayer.SetActive(true);
            }
        }

        if (BoltNetwork.IsClient)
        {
            playerSetup.SetActive(false);
        }

        CheckStreamsReceived();
    }

    public override void Connected(BoltConnection connection)
    {
        connection.SetStreamBandwidth(1024 * 1000);

        if (BoltNetwork.IsServer)
        {
            connection.StreamBytes(WaspJsonChannel, waspData);
            connection.StreamBytes(InitJsonChannel, initData);
            connection.StreamBytes(PlayerJsonChannel, playerData);
        }
    }


    public override void StreamDataProgress(BoltConnection connection, UdpChannelName channel, ulong streamID, float progress)
    {
        if (channel.ToString() == StreamAggregationChannel.ToString())
            return;

        if (channel.ToString() == UpdateAggregationChannel.ToString())
            return;

        if (progressBars == null)
            return;

        if (!progressBars.Active)
            progressBars.ActivateSliders();


        if (channel.ToString() == InitJsonChannel.ToString())
            progressBars.ChangeInitProgress(progress);

        if (channel.ToString() == WaspJsonChannel.ToString())
            progressBars.ChangeWaspProgress(progress);

        if (channel.ToString() == PlayerJsonChannel.ToString())
            progressBars.ChangePlayerProgress(progress);
    }

    public override void StreamDataReceived(BoltConnection connection, UdpStreamData data)
    {
        Debug.Log("Received :" + data.Channel.ToString());

        if (data.Channel.ToString() == StreamAggregationChannel.ToString())
        {
            LoadAggregationContainer.LoadData(data.Data);
            return;
        }

        if (data.Channel.ToString() == UpdateAggregationChannel.ToString())
        {
            LoadAggregationContainer.UpdateData(data.Data);
            return;
        }
        
        if (data.Channel.ToString() == InitJsonChannel.ToString())
        {
            InitJsonReceived = true;
            initJsonData = data.Data;
        }
        else if (data.Channel.ToString() == WaspJsonChannel.ToString())
        {
            WaspJsonReceived = true;
            waspJsonData = data.Data;
        }
        else if (data.Channel.ToString() == PlayerJsonChannel.ToString())
        {
            PlayerJsonReceived = true;
            playerJsonData = data.Data;
        }
        else
            return;

        CheckStreamsReceived();
    }

    void CheckStreamsReceived()
    {
        if (InitJsonReceived && WaspJsonReceived && PlayerJsonReceived)
        {
            if (initLauncher == null)
                return;

            if (playerJsonData.Length == 1)
            {
                initLauncher.Initialize(initJsonData, waspJsonData);
                if (ControllerReferences.MultiPlayer == null)
                    playerSetup.SetActive(true);
            }
            else
            {
                initLauncher.Initialize(initJsonData, waspJsonData, playerJsonData);
                changePlayer.SetActive(true);
            }

            progressBars.gameObject.SetActive(false);

            if (SceneManager.GetActiveScene().name == playScene)
            {
                PlayerJoined.Create().Send();
            }
        }
    }

    public static void LoadServerConfig(string path, out byte[] waspData, out byte[] initData, out byte[] playerData)
    {
        waspData = null;
        initData = null;
        playerData = null;

        JsonInitContainer container;
        DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(JsonInitContainer));

        try
        {
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    initData = Encoding.UTF8.GetBytes(sr.ReadToEnd());
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            Debug.LogError("Could not find config file");
            return;
        }

        try
        {
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                container = (JsonInitContainer)serializer.ReadObject(stream);
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            Debug.LogError("Could not find config file");
            return;
        }


        try
        {
            using (FileStream stream = new FileStream(container.loadLoc, FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    waspData = Encoding.UTF8.GetBytes(sr.ReadToEnd());
                }
            }
        }
        catch
        {
            Debug.LogError("Could not find waspInput file");
            return;
        }

        try
        {
            using (FileStream stream = new FileStream(container.playerSettingsLoc, FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    playerData = Encoding.UTF8.GetBytes(sr.ReadToEnd());
                }
            }
        }
        catch
        {
            Debug.Log("No Player Settings File present");
            playerData = new byte[1];
            return;
        }
    }
}
