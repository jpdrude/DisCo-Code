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

using System.Collections.Generic;
using UnityEngine;
using System.IO;

/*
 * Performs Save/Load/New Functionality
 */

public class SaveLoad
{
    private static string savePath = "";
    public static string SavePath
    {
        get { return savePath; }
        set { savePath = value; }
    }

    private static bool ContainsFile(string timeCode, string dirPath)
    {
        foreach (string s in GetStoredFiles(dirPath))
        {
            if (string.Compare(s, timeCode) == 0)
            {
                return true;
            }
        }
        return false;
    }


    public static string[] GetStoredFiles(string dirPath)
    {
        DirectoryInfo info = new DirectoryInfo(dirPath);
        FileInfo[] fileinfo = info.GetFiles("*.json");

        string[] filenames = new string[fileinfo.Length];
        int count = 0;

        foreach (FileInfo _fileInf in fileinfo)
        {
            filenames[count] = _fileInf.Name;
            count += 1;
        }

        return filenames;
    }


    //Save, Load and New methods
    #region
    public static string SaveGame(string prefix, bool export = false, string subFolder = "")
    {
        string timeCode = prefix + BuildDateString() + ".json";
        string path = SavePath + "/" + subFolder;

        try
        {
            Directory.CreateDirectory(path + "/");
        }
        catch { }

        int i = 2;
        while (ContainsFile(timeCode, path))
        {
            timeCode = prefix + BuildDateString() + "_" + i.ToString() + ".json";
            ++i;
        }

        if (!export)
        {
            path = path + timeCode;
        }
        else
        {
            path = path + prefix + "export.json";

        }

        AssemblyIO.Save(GlobalReferences.FrozenParts, 10, path);

        return timeCode;
    }


    public static void LoadGame(string path)
    {
        Debug.Log(SavePath + "/" + path);
        AssemblyIO.Load(SavePath + "/" + path);
        GlobalReferences.RebuildIndices();
    }

    private static void DeleteExtras()
    {
        List<int> tempIDs = new List<int>();

        Dictionary<int, List<GameObject>> freeSorted = new Dictionary<int, List<GameObject>>();
        Dictionary<int, List<GameObject>> frozenSorted = new Dictionary<int, List<GameObject>>();

        foreach (GameObject go in GlobalReferences.TemplateParts)
        {
            tempIDs.Add(go.GetComponent<Part>().TemplateID);
        }

        foreach (int i in tempIDs)
        {
            freeSorted.Add(i, new List<GameObject>());
            frozenSorted.Add(i, new List<GameObject>());
        }

        foreach (GameObject go in GlobalReferences.FreeParts)
        {
            freeSorted[go.GetComponent<Part>().TemplateID].Add(go);
        }

        foreach (GameObject go in GlobalReferences.FrozenParts.Values)
        {
            frozenSorted[go.GetComponent<Part>().TemplateID].Add(go);
        }

        foreach (int key in freeSorted.Keys)
        {
            int spawnNumber = GlobalReferences.TemplateParts[key].GetComponent<Part>().SpawnNumber;
            while (freeSorted[key].Count + frozenSorted[key].Count > spawnNumber && freeSorted[key].Count > 0)
            {
                GameObject toDel = freeSorted[key][0];
                freeSorted[key].Remove(toDel);
                GlobalReferences.FreeParts.Remove(toDel);
                GameObject.Destroy(toDel);
            }
        }

    }

    public static void NewGame()
    {
        if (BoltNetwork.IsRunning)
        {
            var newGame = NewGameEvent.Create();
            newGame.Send();
            return;
        }

        List<GameObject> tempGos = new List<GameObject>();
        tempGos.AddRange(GlobalReferences.FreeParts);
        tempGos.AddRange(GlobalReferences.AffectedParts);
        
        foreach(GameObject go in GlobalReferences.FrozenParts.Values)
        {
            tempGos.Add(go);
        }

        for (int i = tempGos.Count - 1; i >= 0; --i)
        {
            if (tempGos[i].GetComponent<Part>().ID != -1)
                GlobalReferences.DeletePart((int)tempGos[i].GetComponent<Part>().ID);
            else
                MonoBehaviour.Destroy(tempGos[i]);
        }

        GlobalReferences.FreeParts.Clear();
        GlobalReferences.FrozenParts.Clear();
        GlobalReferences.AffectedParts.Clear();
        GlobalReferences.NumOfParts = 0;
        GlobalReferences.Parts.Clear();

        GlobalReferences.PartSpawner.SpawnMultiple(PartsHolder.NumParts);
    }

    public static string BuildDateString()
    {
        string year = (System.DateTime.Now.Year - 2000).ToString("D2");
        string month = System.DateTime.Now.Month.ToString("D2");
        string day = System.DateTime.Now.Day.ToString("D2");
        string hour = System.DateTime.Now.TimeOfDay.Hours.ToString("D2");
        string minute = System.DateTime.Now.TimeOfDay.Minutes.ToString("D2");

        return year + month + day + "_" + hour + "-" + minute;
    }
    #endregion
}
