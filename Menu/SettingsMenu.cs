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
using SimpleFileBrowser;
using System.IO;
using UnityEngine.UI;
using System.Runtime.Serialization.Json;
using UnityEditor;

/*
 * Settings Menu
 * 
 * Sets up important variables for game
 *      - Wasp Input file location
 *      - save location
 *      - player setup location
 *      
 *      - thresholds
 *      
 *      - visuals
 *      - etc...
 */

public class SettingsMenu : MonoBehaviour
{
    [SerializeField]
    Text lblInputLocation;
    string inputLocation = null;

    [SerializeField]
    Text lblSaveLocation;
    string saveLocation = null;

    [SerializeField]
    Text lblPlayerSettingsLocation;
    string playerSettingsLocation = null;

    [SerializeField]
    Dropdown colorSchemeDropdown;
    string colorScheme = "Candy";

    [SerializeField]
    Dropdown anaglyphDropdown;
    AnaglyphizerC.Mode anaglyphMode = AnaglyphizerC.Mode.None;

    [SerializeField]
    InputField multiplayerNameInput;
    string multiplayerName = null;

    [SerializeField]
    Toggle independantMPToggle;
    bool independantMP = false;

    [SerializeField]
    InputField snapThreshInput;
    [SerializeField]
    Slider snapThreshSlider;
    float snapThreshold = 0.2f;
    float SnapThreshold
    {
        get { return snapThreshold; }
        set
        {
            snapThreshold = value;
            if (snapThreshold <= 1 && snapThreshold > 0)
                snapThreshSlider.value = snapThreshold;
            else if (snapThreshold > 0)
                snapThreshSlider.value = snapThreshSlider.maxValue;
            snapThreshInput.text = snapThreshold.ToString();
        }
    }

    [SerializeField]
    Text angleToleranceLabel;
    [SerializeField]
    Slider angleToleranceSlider;
    float angleTolerance = 1.15f;
    float AngleTolerance
    {
        get { return angleTolerance; }
        set
        {
            angleTolerance = value;
            if (angleTolerance < 1.08f)
                angleToleranceLabel.text = "lose snap";
            else if (angleTolerance < 1.13f)
                angleToleranceLabel.text = "medium snap";
            else if (angleTolerance < 1.17f)
                angleToleranceLabel.text = "tight snap";
            else
                angleToleranceLabel.text = "super tight snap";

            if (angleTolerance >= angleToleranceSlider.minValue && angleTolerance <= angleToleranceSlider.maxValue)
                angleToleranceSlider.value = angleTolerance;
        }
    }

    [SerializeField]
    InputField numPartsInput;
    [SerializeField]
    Slider numPartsSlider;
    int numParts = 200;
    int NumParts
    {
        get { return numParts; }
        set 
        { 
            numParts = value;
            if (numParts <= 1000 && numParts >= 0)
                numPartsSlider.value = numParts;
            else if (numParts >= 0)
                numPartsSlider.value = numPartsSlider.maxValue;
            numPartsInput.text = numParts.ToString();
        }
    }

    [SerializeField]
    InputField shootDistInput;
    [SerializeField]
    Slider shootDistSlider;
    float shootDist = 1;
    float ShootDist
    {
        get { return shootDist; }
        set
        {
            shootDist = value;
            if (shootDist <= 10 && shootDist >= 0)
                shootDistSlider.value = shootDist;
            else if (shootDist >= 0)
                shootDistSlider.value = shootDistSlider.maxValue;
            shootDistInput.text = shootDist.ToString();
        }
    }

    [SerializeField]
    InputField fogInput;
    [SerializeField]
    Slider fogSlider;
    float fog = 100;
    float Fog
    {
        get { return fog; }
        set
        {
            fog = value / 1000;
            if (value <= 100 && value >= 0)
                fogSlider.value = value;
            else if (value >= 0)
                fogSlider.value = fogSlider.maxValue;
            fogInput.text = value.ToString();
        }
    }

    /*
    [SerializeField]
    InputField fieldResInput;
    [SerializeField]
    Slider fieldResSlider;
    float fieldRes = 0.5f;
    float FieldRes
    {
        get { return fieldRes; }
        set
        {
            fieldRes = value;
            if (fieldRes <= 1 && fieldRes >= fieldResSlider.minValue)
                fieldResSlider.value = fieldRes;
            else if (fieldRes >= fieldResSlider.minValue)
                fieldResSlider.value = fieldResSlider.maxValue;
            fieldResInput.text = fieldRes.ToString();
        }
    }
    */

    /*
    [SerializeField]
    Toggle groundPlaneToggle;
    bool groundPlane = true;

    [SerializeField]
    InputField minXField;
    float minX = -4;

    [SerializeField]
    InputField maxXField;
    float maxX = 4;

    [SerializeField]
    InputField minYField;
    float minY = 0;

    [SerializeField]
    InputField maxYField;
    float maxY = 4;

    [SerializeField]
    InputField minZField;
    float minZ = -4;

    [SerializeField]
    InputField maxZField;
    float maxZ = 4;
    */


    // Start is called before the first frame update
    void Start()
    {
        FileBrowser.SetFilters(false, new FileBrowser.Filter("JSON", ".json"));
        FileBrowser.SetDefaultFilter(".json");
        FileBrowser.AddQuickLink("Users", "C:\\Users", null);

        LoadConfigFile(Application.dataPath + "/Resources/config.json");

        ResetFields();
    }


    void ResetFields()
    {
        if (inputLocation != null)
            lblInputLocation.text = inputLocation;

        if (saveLocation != null)
            lblSaveLocation.text = saveLocation;

        if (playerSettingsLocation != null)
            lblPlayerSettingsLocation.text = playerSettingsLocation;

        switch (colorScheme)
        {
            case "Candy":
                colorSchemeDropdown.value = 0;
                break;
            case "BlackWhite":
                colorSchemeDropdown.value = 1;
                break;
            case "DroppingSun":
                colorSchemeDropdown.value = 2;
                break;
            case "Space":
                colorSchemeDropdown.value = 3;
                break;
        }

        switch (anaglyphMode)
        {
            case AnaglyphizerC.Mode.None:
                anaglyphDropdown.value = 0;
                break;
            case AnaglyphizerC.Mode.Color:
                anaglyphDropdown.value = 1;
                break;
            case AnaglyphizerC.Mode.True:
                anaglyphDropdown.value = 2;
                break;
        }

        if (multiplayerName != null)
            multiplayerNameInput.text = multiplayerName;

        independantMPToggle.isOn = independantMP;

        SnapThreshold = snapThreshold;

        AngleTolerance = angleTolerance;

        NumParts = numParts;

        ShootDist = shootDist;

        Fog = fog * 1000;

        /*
        FieldRes = fieldRes;
        groundPlaneToggle.isOn = groundPlane;

        minXField.text = minX.ToString();
        maxXField.text = maxX.ToString();
        minYField.text = minY.ToString();
        maxYField.text = maxY.ToString();
        minZField.text = minZ.ToString();
        maxZField.text = maxZ.ToString();
        */
    }

    public void ChangeInputLocation()
    {
        StartCoroutine(ShowLoadInputFileDialogCoroutine());
    }

    public void ChangeSaveLocation()
    {
        StartCoroutine(ShowLoadSavePathDialogCoroutine());
    }

    public void ChangePlayerSettingsLocation()
    {
        StartCoroutine(ShowLoadPlayerSettingsFileDialogCoroutine());
    }

    public void ChangeColorScheme(int idx)
    {
        switch (colorSchemeDropdown.value)
        {
            case 0:
                colorScheme = "Candy";
                break;
            case 1:
                colorScheme = "BlackWhite";
                break;
            case 2:
                colorScheme = "DroppingSun";
                break;
            case 3:
                colorScheme = "Space";
                break;
        }
    }

    public void ChangeAnaglyphMode(int idx)
    {
        anaglyphMode = (AnaglyphizerC.Mode)anaglyphDropdown.value;
    }

    public void ChangeMultiplayerName()
    {
        multiplayerName = multiplayerNameInput.text;
    }

    public void ChangeIndependantMP()
    {
        independantMP = !independantMP;
    }

    public void ClearPlayerSettingsLocation()
    {
        playerSettingsLocation = null;
        lblPlayerSettingsLocation.text = "no path provided";
    }

    public void LoadPreviousLaunchFile()
    {
        StartCoroutine(ShowLoadPrevLaunchFileDialogCoroutine());
    }

    public void WriteSettings()
    {
        SaveConfigFile(Application.dataPath + "/Resources/config.json");
    }

    public void ChangeSnapThreshInput()
    {
        try
        {
            float thresh = float.Parse(snapThreshInput.text);

            if (thresh > 0)
                SnapThreshold = thresh;
            else
                throw new System.Exception("No negative values allowed");
        }
        catch
        {
            Debug.Log("Input is not a Number");
            snapThreshInput.text = snapThreshold.ToString();
        }
    }

    public void ChangeSnapThreshSlider()
    {
        SnapThreshold = snapThreshSlider.value;
    }

    public void ChangeNumPartsInput()
    {
        try
        {
            int num = int.Parse(numPartsInput.text);
            if (num >= 0)
                NumParts = num;
            else
                throw new System.Exception("No negative values allowed");
        }
        catch
        {
            Debug.Log("Input is not a Number");
            numPartsInput.text = numParts.ToString();
        }
    }

    public void ChangeShootDistInput()
    {
        try
        {
            float dist = float.Parse(shootDistInput.text);
            if (dist >= 0)
                ShootDist = dist;
            else
                throw new System.Exception("No negative values allowed");
        }
        catch
        {
            Debug.Log("Input is not a Number");
            shootDistInput.text = shootDist.ToString();
        }
    }

    public void ChangeFogInput()
    {
        try
        {
            float f = float.Parse(fogInput.text);
            if (f >= 0)
                Fog = f;
            else
                throw new System.Exception("No negative values allowed");
        }
        catch
        {
            Debug.Log("Input is not a Number");
            fogInput.text = (fog * 1000).ToString();
        }
    }

    public void ChangeNumPartsSlider()
    {
        NumParts = (int)numPartsSlider.value;
    }

    public void ChangeShootDistSlider()
    {
        ShootDist = shootDistSlider.value;
    }

    public void ChangeFogSlider()
    {
        Fog = fogSlider.value;
    }

    public void ChangeAngleToleranceSlider()
    {
        AngleTolerance = angleToleranceSlider.value;
    }


    IEnumerator ShowLoadInputFileDialogCoroutine()
	{
		yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, false, inputLocation, null, "Load Input File", "Load");

		// Dialog is closed
		// Print whether the user has selected some files/folders or cancelled the operation (FileBrowser.Success)
		Debug.Log(FileBrowser.Success);

		if (FileBrowser.Success)
		{
            inputLocation = FileBrowser.Result[0];
            lblInputLocation.text = inputLocation;
		}
	}

    IEnumerator ShowLoadPlayerSettingsFileDialogCoroutine()
    {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, false, inputLocation, null, "Load Input File", "Load");

        // Dialog is closed
        // Print whether the user has selected some files/folders or cancelled the operation (FileBrowser.Success)
        Debug.Log(FileBrowser.Success);

        if (FileBrowser.Success)
        {
            playerSettingsLocation = FileBrowser.Result[0];
            lblPlayerSettingsLocation.text = playerSettingsLocation;
        }
    }

    IEnumerator ShowLoadSavePathDialogCoroutine()
    {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Folders, false, saveLocation, null, "Set Save Location", "Set");

        // Dialog is closed
        // Print whether the user has selected some files/folders or cancelled the operation (FileBrowser.Success)
        Debug.Log(FileBrowser.Success);

        if (FileBrowser.Success)
        {
            saveLocation = FileBrowser.Result[0];
            lblSaveLocation.text = saveLocation;
        }
    }

    IEnumerator ShowLoadPrevLaunchFileDialogCoroutine()
    {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, false, null, null, "Launch File", "Load");

        // Dialog is closed
        // Print whether the user has selected some files/folders or cancelled the operation (FileBrowser.Success)
        Debug.Log(FileBrowser.Success);

        if (FileBrowser.Success)
        {
            LoadConfigFile(FileBrowser.Result[0]);
            ResetFields();
        }
    }

    bool LoadConfigFile(string path)
    {
        JsonInitContainer container = null;

        DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(JsonInitContainer));

        try
        {
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                container = serializer.ReadObject(stream) as JsonInitContainer;
            }
        }
        catch
        {
            Debug.LogError("Could not find config file");
        }

        if (container != null)
        {
            saveLocation = container.saveLoc;
            inputLocation = container.loadLoc;
            playerSettingsLocation = container.playerSettingsLoc;

            SnapThreshold = container.snapThreshold;
            AngleTolerance = container.angleTightFactor;

            NumParts = container.numberOfParts;

            colorScheme = container.colScheme;

            anaglyphMode = (AnaglyphizerC.Mode)container.anaglyph;

            multiplayerName = container.multiplayerName;

            independantMP = container.independantMP;

            fog = container.fog;

            shootDist = container.shootDistVisibility;
        }

        return container != null;
    }

    public void SaveConfigFile(string path)
    {
        JsonInitContainer container = new JsonInitContainer();

        container.loadLoc = inputLocation;
        Debug.Log("Input Loc: " + inputLocation);
        container.saveLoc = saveLocation;
        Debug.Log("Save Loc: " + saveLocation);
        container.playerSettingsLoc = playerSettingsLocation;
        Debug.Log("Player Settings Loc: " + playerSettingsLocation);
        container.numberOfParts = numParts;
        Debug.Log("NumParts: " + numParts);
        container.snapThreshold = snapThreshold;
        Debug.Log("Snap Threshold: " + snapThreshold);
        container.colScheme = colorScheme;
        Debug.Log("ColorScheme: " + colorScheme);
        container.angleTightFactor = angleTolerance;
        Debug.Log("Angle Tolerance: " + angleTolerance);
        container.anaglyph = (int)anaglyphMode;
        Debug.Log("Anaglyph: " + (int)anaglyphMode);
        container.multiplayerName = multiplayerName;
        Debug.Log("Multiplayer Name: " + multiplayerName);
        container.independantMP = independantMP;
        Debug.Log("IndependantMP: " + independantMP);
        container.fog = fog;
        Debug.Log("Fog: " + fog);
        container.shootDistVisibility = shootDist;
        Debug.Log("Shoot Visibilizty: " + shootDist);

        DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(JsonInitContainer));
        Debug.Log("Container: " + container.ToString());

        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            serializer.WriteObject(stream, container);
        }
    }
}
