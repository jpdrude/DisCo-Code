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
using System.Data;
using UnityEngine;
using UnityEngine.UI;

/*
 * Controller Settings Menu sets up Toolset Options in Game
 */

public class ControllerSettings : MonoBehaviour
{
    List<bool> toolsetSettings;

    List<bool> placementSettings;
    public List<bool> PlacementSettings { get { return placementSettings; } }

    List<bool> saveLoadSettings;
    public List<bool> SaveLoadSettings { get { return saveLoadSettings; } }

    [SerializeField]
    NetworkPlayerMenu playerMenu;

    [SerializeField]
    GameObject placementTools;

    [SerializeField]
    GameObject saveLoadTools;

    [SerializeField]
    Toggle numPartsToggle;

    [SerializeField]
    Toggle ruleFilterToggle;

    [SerializeField]
    Toggle partFilterToggle;

    bool numParts = true;
    bool partFilter = true;
    bool ruleFilter = true;
    bool placementType = true;
    bool field = true;
    bool saveLoad = true;
    bool simulation = true;
    bool characterSettings = true;
    bool scale = true;

    bool place = true;
    bool choreograph = true;
    bool shoot = true;
    bool grow = true;
    bool pickNChose = true;
    bool delete = true;
    bool deleteRec = true;
    bool deleteSphere = true;
    bool enableDisable = true;

    bool saveGame = true;
    bool loadGame = true;
    bool newGame = true;
    bool exportField = true;

    public void ChangeNumParts() { numParts = !numParts; }
    public void ChangePartFilter() { partFilter = !partFilter; }
    public void ChangeRuleFilter() { ruleFilter = !ruleFilter; }
    public void ChangeSimulation() { simulation = !simulation; }
    public void ChangeCharacterSettings() { characterSettings = !characterSettings; }
    public void ChangeScale() { scale = !scale; }
    public void ChangeField() { field = !field; }

    public void ChangePlacementType()
    {
        placementType = !placementType;
        CheckNumParts(placementType);
        CheckFilter(placementType);

        if (!placementType)
            SetTogglesActive(placementTools, false);
        else
            SetTogglesActive(placementTools, true);
    }

    public void ChangeSaveLoad()
    {
        saveLoad = !saveLoad;

        if (!saveLoad)
            SetTogglesActive(saveLoadTools, false);
        else
            SetTogglesActive(saveLoadTools, true);
    }


    public void ChangePlace() { place = !place; }
    public void ChangeChoreo() 
    { 
        choreograph = !choreograph;
        CheckNumParts(choreograph);
    }
    public void ChangeShoot() { shoot = !shoot; }
    public void ChangeGrow() { grow = !grow; }
    public void ChangePickNChose() { pickNChose = !pickNChose; }
    public void ChangeDelete() { delete = !delete; }
    public void ChangeDeleteRec() { deleteRec = !deleteRec; }
    public void ChangeDeleteSphere() { deleteSphere = !deleteSphere; }
    public void ChangeEnableDisable() { enableDisable = !enableDisable; }


    public void ChangeSave() { saveGame = !saveGame; }
    public void ChangeLoad() { loadGame = !loadGame; }
    public void ChangeNew() { newGame = !newGame; }
    public void ChangeExportField() { exportField = !exportField; }


    void SetTogglesActive(GameObject go, bool active)
    {
        Toggle[] toggles = go.GetComponentsInChildren<Toggle>();

        foreach (Toggle toggle in toggles)
            toggle.interactable = active;
    }

    void CheckNumParts(bool choreograph)
    {
        if (!choreograph)
        {
            numPartsToggle.interactable = false;
            numParts = false;
        }
        else
        {
            numPartsToggle.interactable = true;
            numParts = numPartsToggle.isOn;
        }
    }

    void CheckFilter(bool placement)
    {
        if (!placement)
        {
            partFilterToggle.interactable = false;
            ruleFilterToggle.interactable = false;
            partFilter = false;
            ruleFilter = false;
        }
        else
        {
            partFilterToggle.interactable = true;
            ruleFilterToggle.interactable = true;
            partFilter = partFilterToggle.isOn;
            ruleFilter = ruleFilterToggle.isOn;
        }
    }

    public void SpawnFPS()
    {
        gameObject.SetActive(false);

        placementSettings = new List<bool>();
        if (placementType)
        {  
            placementSettings.Add(place);
            placementSettings.Add(choreograph);
            placementSettings.Add(shoot);
            placementSettings.Add(grow);
            placementSettings.Add(pickNChose);
            placementSettings.Add(delete);
            placementSettings.Add(deleteRec);
            placementSettings.Add(deleteSphere);
            placementSettings.Add(enableDisable);
        }

        saveLoadSettings = new List<bool>();
        if (saveLoad)
        {
            saveLoadSettings.Add(saveGame);
            saveLoadSettings.Add(loadGame);
            saveLoadSettings.Add(newGame);
            saveLoadSettings.Add(exportField);
        }


        placementType = CheckAllFalse(placementSettings);
        saveLoad = CheckAllFalse(saveLoadSettings);

        toolsetSettings = new List<bool>();
        toolsetSettings.Add(numParts);
        toolsetSettings.Add(partFilter);
        toolsetSettings.Add(ruleFilter);
        toolsetSettings.Add(placementType);
        toolsetSettings.Add(field);
        toolsetSettings.Add(saveLoad);
        toolsetSettings.Add(simulation);
        toolsetSettings.Add(characterSettings);
        toolsetSettings.Add(scale);

        playerMenu.SpawnFPSController(toolsetSettings, placementSettings, saveLoadSettings);
    }

    public void SpawnVR()
    {
        gameObject.SetActive(false);

        placementSettings = new List<bool>();
        if (placementType)
        {
            placementSettings.Add(place);
            placementSettings.Add(choreograph);
            placementSettings.Add(shoot);
            placementSettings.Add(grow);
            placementSettings.Add(pickNChose);
            placementSettings.Add(delete);
            placementSettings.Add(deleteRec);
            placementSettings.Add(deleteSphere);
            placementSettings.Add(enableDisable);
        }

        saveLoadSettings = new List<bool>();
        if (saveLoad)
        {
            saveLoadSettings.Add(saveGame);
            saveLoadSettings.Add(loadGame);
            saveLoadSettings.Add(newGame);
            saveLoadSettings.Add(exportField);
        }   

        placementType = CheckAllFalse(placementSettings);
        saveLoad = CheckAllFalse(saveLoadSettings);

        toolsetSettings = new List<bool>();
        toolsetSettings.Add(numParts);
        toolsetSettings.Add(partFilter);
        toolsetSettings.Add(ruleFilter);
        toolsetSettings.Add(placementType);
        toolsetSettings.Add(field);
        toolsetSettings.Add(saveLoad);
        toolsetSettings.Add(simulation);
        toolsetSettings.Add(characterSettings);
        toolsetSettings.Add(scale);

        playerMenu.SpawnVRController(toolsetSettings, placementSettings, saveLoadSettings);
    }

    bool CheckAllFalse(List<bool> checkList)
    {
        foreach (bool b in checkList)
        {
            if (b) return true;
        }

        return false;
    }
}
