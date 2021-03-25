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
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;
using Valve.Newtonsoft.Json;

/*
 * Initializes Game from Settings
 * also deserializes and initializes streamed settings in Multiplayer
 * Contains Serialization Classes for Settings Menu
 * 
 * Initializes:
 *      - Parts Holder
 *      - Custom Player (in Multiplayer)
 *      - Connection Threshold
 *      - Visuals (Color Scheme, Fog, etc.)
 */
public class InitializeLauncher : MonoBehaviour
{

    GameObject Controller;

    [SerializeField]
    GameObject Floor;

    [SerializeField]
    GameAreaProxy gameAreaProxy;

    [SerializeField]
    GameObject customPlayerSetup;

    [SerializeField]
    GameObject customPlayerButton;

    [SerializeField]
    GameObject callibrateAnaglyph;

    Vector3[] buttonPositions;

    string LoadLoc;
    string SaveLoc;
    string PlayerSettingsLoc;

    int VrFps = 0;

    float SnapThresh = 0.1f;

    float AngleTighteningFactor = 1.15f;

    int NumParts = 100;

    float FieldRes = 0.2f;

    AnaglyphizerC.Mode anaglyph = 0;

    string colorScheme = "Candy";

    string MultiplayerName;

    bool IndependantMP;

    float fog;

    float shootDistVisibility;


    public void Initialize()
    {

        LoadJson(Application.dataPath + "/Resources/config.json");

        InitializeGame();

        GlobalReferences.PartSpawner = new PartsHolder(NumParts, LoadLoc);
    }

    
    public void Initialize(byte[] initData, byte[] waspData)
    {
        LoadJson(Application.dataPath + "/Resources/config.json");

        JsonInitContainer container = null;

        DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(JsonInitContainer));

        using (MemoryStream stream = new MemoryStream(initData))
        {
            container = serializer.ReadObject(stream) as JsonInitContainer;
        }

        if (!IndependantMP)
        {
            SnapThresh = container.snapThreshold;
            AngleTighteningFactor = container.angleTightFactor;
            NumParts = container.numberOfParts;
            //FieldRes = container.fieldResolution;
            IndependantMP = container.independantMP;

        }

        InitializeGame();

        GlobalReferences.PartSpawner = new PartsHolder(NumParts, null, waspData);
    }

    public void Initialize(byte[] initData, byte[] waspData, byte[] playerData)
    {
        Initialize(initData, waspData);

        InitializeCustomPlayers(playerData);
    }


    public void Initialize(byte[] playerData)
    {
        Initialize();

        InitializeCustomPlayers(playerData);
    }

    void InitializeCustomPlayers(byte[] playerData)
    {

        PlayerSettings playerSettings;
        string jsonString;

        using (MemoryStream stream = new MemoryStream(playerData))
        {
            using(StreamReader sr = new StreamReader(stream))
            {
                playerSettings = JsonConvert.DeserializeObject<PlayerSettings>(sr.ReadToEnd());
            }
        }

        customPlayerSetup.SetActive(true);

        buttonPositions = new Vector3[18] { new Vector3(0, 0, 0), new Vector3(0,-50,0), new Vector3(0,-100,0), new Vector3(0,50,0), new Vector3(0,100,0), new Vector3(0,-150,0),
                                            new Vector3(-200,0,0), new Vector3(-200,-50,0), new Vector3(-200,-100,0), new Vector3(-200, 50,0), new Vector3(-200,100,0), new Vector3(-200,-150,0),
                                            new Vector3(200,0,0), new Vector3(200,-50,0), new Vector3(200,-100,0), new Vector3(200,50,0), new Vector3(200,100,0), new Vector3(200,-150,0)};

        int posCount = 0;
        foreach (Player player in playerSettings.players)
        {
            player.ToUnity();
            player.BuildPlaceholders();

            Debug.Log("Player: " + player.playerName + " with ToolSettings: " + player.toolsetSettings.Count + ", Placementsettings: " + player.placementSettings.Count + ", SaveLoadSettings: " + player.saveLoadSettings.Count);
            GameObject customButtonGo = Instantiate(customPlayerButton, customPlayerSetup.transform);
            customButtonGo.transform.localPosition = buttonPositions[posCount];
            customButtonGo.GetComponent<SpawnCustomPlayer>().player = player;

            customButtonGo.SetActive(true);

            ++posCount;
            if (posCount >= buttonPositions.Length)
                return;
        }
    }

    public void InitializeGame()
    {
        MaterialHolder.MatSet = colorScheme;

        ConnectionScanning.ConnectionThreshold = SnapThresh;
        ConnectionScanning.AngleTighteningFactor = AngleTighteningFactor;
        SaveLoad.SavePath = SaveLoc;
        RenderSettings.skybox = Resources.Load<Material>("Materials/" + colorScheme + "/Sky/Sky");

        MaterialHolder.Anaglyph = anaglyph;
        if (anaglyph == AnaglyphizerC.Mode.Color)
            callibrateAnaglyph.SetActive(true);

        ControllerReferences.MultiPlayerName = MultiplayerName;

        TIExportField.Resolution = FieldRes;

        KillTimer.VisDist = shootDistVisibility;

        RenderSettings.fogDensity = fog;
    }


    private void LoadJson(string path)
    {
        JsonInitContainer container = null;

        DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(JsonInitContainer));

        using (FileStream stream = new FileStream(path, FileMode.Open))
        {
            container = serializer.ReadObject(stream) as JsonInitContainer;
        }

        if (container != null)
        {
            SaveLoc = container.saveLoc;
            LoadLoc = container.loadLoc;
            PlayerSettingsLoc = container.playerSettingsLoc;

            VrFps = container.vrFps;

            SnapThresh = container.snapThreshold;
            AngleTighteningFactor = container.angleTightFactor;

            NumParts = container.numberOfParts;

            //FieldRes = container.fieldResolution;

            colorScheme = container.colScheme;

            anaglyph = (AnaglyphizerC.Mode)container.anaglyph;

            MultiplayerName = container.multiplayerName;

            IndependantMP = container.independantMP;

            ControllerReferences.IndependantMP = IndependantMP;

            fog = container.fog;

            shootDistVisibility = container.shootDistVisibility;
        }
    }
}

    [DataContract(Name = "References")]
    public class JsonInitContainer
    {
        [DataMember(Name = "LoadLocation")]
        public string loadLoc;

        [DataMember(Name = "SaveLocation")]
        public string saveLoc;

        [DataMember(Name = "PlayerSettingsLocation")]
        public string playerSettingsLoc;

        [DataMember(Name = "VrFps")]
        public int vrFps;

        [DataMember(Name = "SnapThreshold")]
        public float snapThreshold;

        [DataMember(Name = "AngleTighteningFactor")]
        public float angleTightFactor;

        [DataMember(Name = "NumberOfParts")]
        public int numberOfParts;

        [DataMember(Name = "ShootDistVisibility")]
        public float shootDistVisibility;

        //[DataMember(Name = "FieldResolution")]
        //public float fieldResolution;

        [DataMember(Name = "ColorScheme")]
        public string colScheme;

        [DataMember(Name = "Anaglyph")]
        public int anaglyph;

        [DataMember(Name = "MultiplayerName")]
        public string multiplayerName;

        [DataMember(Name = "IndependantMP")]
        public bool independantMP;

        [DataMember(Name = "Fog")]
        public float fog;
    }

