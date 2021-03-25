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
using System.Text;
using TMPro;
using UnityEngine;
using Valve.Newtonsoft.Json;
using System.IO;
using System;

/*
 * Contains Serialization Classes for Wasp Player Setup and Constraint Geometry
 * Deals with Movement of Player in/on constraint geometry
 * Sets up Toolset
 */

public class PlayerSettings
{
    public List<Player> players;

    public PlayerSettings()
    {
        players = new List<Player>();
    }
}

public class Player
{
    public int VrFps;

    public string playerName;

    public List<Curve> constraintCurves;

    public List<Point> constraintPoints;

    public List<Box> constraintBoxes;

    public Dictionary<string, bool> toolsetSettings;

    public Dictionary<string, bool> placementSettings;

    public Dictionary<string, bool> saveLoadSettings;

    private int currentPt = 0;
    private int currentCrv = 0;
    private int currentBox = 0;

    private int currentPtSave = 0;
    private int currentCrvSave = 0;
    private int currentBoxSave = 0;


    public Player()
    {
        VrFps = 1;
        playerName = "unknownPlayer";
        constraintCurves = new List<Curve>();
        constraintPoints = new List<Point>();
        constraintBoxes = new List<Box>();
        toolsetSettings = new Dictionary<string, bool>();
        placementSettings = new Dictionary<string, bool>();
        saveLoadSettings = new Dictionary<string, bool>();

        toolsetSettings.Add("NumParts", true);
        toolsetSettings.Add("PartFilter", true);
        toolsetSettings.Add("RuleFilter", true);
        toolsetSettings.Add("PlacementType", true);
        toolsetSettings.Add("Field", true);
        toolsetSettings.Add("SaveLoad", true);
        toolsetSettings.Add("Simulation", true);
        toolsetSettings.Add("CharacterSettings", true);
        toolsetSettings.Add("Scale", true);

        placementSettings.Add("Place", true);
        placementSettings.Add("Choreograph", true);
        placementSettings.Add("Shoot", true);
        placementSettings.Add("Grow", true);
        placementSettings.Add("PickNChose", true);
        placementSettings.Add("Delete", true);
        placementSettings.Add("DeleteRec", true);
        placementSettings.Add("DeleteSphere", true);
        placementSettings.Add("EnableDisable", true);

        saveLoadSettings.Add("SaveGame", true);
        saveLoadSettings.Add("LoadGame", true);
        saveLoadSettings.Add("NewGame", true);
        saveLoadSettings.Add("ExportField", true);
    }

    public Player(int _VrFps, string _playerName, List<Curve> _constraintCurves, List<Point> _constrainPoints, List<Box> _constrintBoxes,
        Dictionary<string, bool> _toolsetSettings, Dictionary<string, bool> _placementSettings, Dictionary<string, bool> _saveLoadSettings)
    {
        VrFps = _VrFps;
        playerName = _playerName;
        constraintCurves = _constraintCurves;
        constraintPoints = _constrainPoints;
        constraintBoxes = _constrintBoxes;
        toolsetSettings = _toolsetSettings;
        placementSettings = _placementSettings;
        saveLoadSettings = _saveLoadSettings;
    }

    public List<bool> ToolsetSettings
    {
        get
        {
            List<bool> settings = new List<bool>();

            if (placementSettings["Choreograph"] && toolsetSettings["PlacementType"])
                settings.Add(toolsetSettings["NumParts"]);
            else
                settings.Add(false);

            if (toolsetSettings["PlacementType"])
            {
                settings.Add(toolsetSettings["PartFilter"]);
                settings.Add(toolsetSettings["RuleFilter"]);
            }
            else
            {
                settings.Add(false);
                settings.Add(false);
            }

            if (!CheckAllFalse(placementSettings))
                settings.Add(false);
            else
                settings.Add(toolsetSettings["PlacementType"]);

            settings.Add(toolsetSettings["Field"]);

            if (!CheckAllFalse(saveLoadSettings))
                settings.Add(false);
            else
                settings.Add(toolsetSettings["SaveLoad"]);

            settings.Add(toolsetSettings["Simulation"]);
            settings.Add(toolsetSettings["CharacterSettings"]);
            settings.Add(toolsetSettings["Scale"]);

            return settings;
        }
    }

    public List<bool> PlacementSettings
    {
        get
        {
            List<bool> settings = new List<bool>();

            settings.Add(placementSettings["Place"]);
            settings.Add(placementSettings["Choreograph"]);
            settings.Add(placementSettings["Shoot"]);
            settings.Add(placementSettings["Grow"]);
            settings.Add(placementSettings["PickNChose"]);
            settings.Add(placementSettings["Delete"]);
            settings.Add(placementSettings["DeleteRec"]);
            settings.Add(placementSettings["DeleteSphere"]);
            settings.Add(placementSettings["EnableDisable"]);

            return settings;
        }
    }

    public List<bool> SaveLoadSettings
    {
        get
        {
            List<bool> settings = new List<bool>();

            settings.Add(saveLoadSettings["SaveGame"]);
            settings.Add(saveLoadSettings["LoadGame"]);
            settings.Add(saveLoadSettings["NewGame"]);

            if (toolsetSettings["Field"])
                settings.Add(saveLoadSettings["ExportField"]);
            else
                settings.Add(false);

            return settings;
        }
    }

    bool CheckAllFalse(Dictionary<string, bool> settings)
    {
        foreach (bool b in settings.Values)
            if (b)
                return true;

        return false;
    }

    public Point GetCurrentPoint()
    {
        if (constraintPoints == null || constraintPoints.Count == 0)
            return null;

        return constraintPoints[currentPt];
    }

    public Curve GetCurrentCurve()
    {
        if (constraintCurves == null || constraintCurves.Count == 0)
            return null;

        return constraintCurves[currentCrv];
    }

    public Box GetCurrentBox()
    {
        if (constraintBoxes == null || constraintBoxes.Count == 0)
            return null;

        return constraintBoxes[currentBox];
    }

    public Point GetNextPoint()
    {
        try
        {
            if (constraintPoints.Count > currentPt + 1)
                ++currentPt;
            else
                currentPt = 0;

            return GetCurrentPoint();
        }
        catch
        { return null; }
    }

    public Curve GetNextCurve()
    {
        try
        {
            if (constraintCurves.Count > currentCrv + 1)
                ++currentCrv;
            else
                currentCrv = 0;

            return GetCurrentCurve();
        }
        catch
        { return null; }
    }

    public Box GetNextBox()
    {
        try
        {
            if (constraintBoxes.Count > currentBox + 1)
                ++currentBox;
            else
                currentBox = 0;

            return GetCurrentBox();
        }
        catch
        { return null; }
    }

    public Point GetPrevPoint()
    {
        try
        {
            if (currentPt - 1 >= 0)
                --currentPt;
            else
                currentPt = constraintPoints.Count - 1;

            return GetCurrentPoint();
        }
        catch
        { return null; }
    }

    public Curve GetPrevCurve()
    {
        try
        {
            if (currentCrv - 1 >= 0)
                --currentCrv;
            else
                currentCrv = constraintCurves.Count - 1;

            return GetCurrentCurve();
        }
        catch
        { return null; }
    }

    public Box GetPrevBox()
    {
        try
        {
            if (currentBox - 1 >= 0)
                --currentBox;
            else
                currentBox = constraintBoxes.Count - 1;

            return GetCurrentBox();
        }
        catch
        { return null; }
    }

    public void SavePos()
    {
        currentPtSave = currentPt;
        currentCrvSave = currentCrv;
        currentBoxSave = currentBox;
    }

    public void RestorePos()
    {
        currentPt = currentPtSave;
        currentCrv = currentCrvSave;
        currentBox = currentBoxSave;
    }

    public void HighlightAllBoxes()
    {
        if (constraintBoxes == null || constraintBoxes.Count == 0)
            return;

        LineRenderer lr;
        foreach (Box box in constraintBoxes)
        {
            box.BoxGo.SetActive(true);
            lr = box.BoxGo.GetComponent<LineRenderer>();
            lr.startColor = Color.Lerp(MaterialHolder.AffectedColor, Color.black, 0.8f);
            lr.endColor = Color.Lerp(MaterialHolder.AffectedColor, Color.black, 0.8f);
            lr.rendererPriority = 1;
        }

        lr = GetCurrentBox().BoxGo.GetComponent<LineRenderer>();
        lr.startColor = MaterialHolder.AffectedColor;
        lr.endColor = MaterialHolder.AffectedColor;
        lr.rendererPriority = 2;
    }

    public void HighlightAllCurves()
    {
        if (constraintCurves == null || constraintCurves.Count == 0)
            return;

        LineRenderer lr;
        foreach (Curve crv in constraintCurves)
        {
            crv.CurveGo.SetActive(true);
            lr = crv.CurveGo.GetComponent<LineRenderer>();
            lr.startColor = Color.Lerp(MaterialHolder.AffectedColor, Color.black, 0.8f);
            lr.endColor = Color.Lerp(MaterialHolder.AffectedColor, Color.black, 0.8f);
            lr.rendererPriority = 1;
        }

        lr = GetCurrentCurve().CurveGo.GetComponent<LineRenderer>();
        lr.startColor = MaterialHolder.AffectedColor;
        lr.endColor = MaterialHolder.AffectedColor;
        lr.rendererPriority = 2;
    }

    public void HighlightAllPoints()
    {
        if (constraintPoints == null || constraintPoints.Count == 0)
            return;

        foreach (Point pt in constraintPoints)
        {
            pt.PointGo.SetActive(true);
            pt.PointGo.GetComponentInChildren<SpriteRenderer>().color = Color.Lerp(MaterialHolder.AffectedColor, Color.black, 0.8f);
        }

        GetCurrentPoint().PointGo.GetComponentInChildren<SpriteRenderer>().color = MaterialHolder.AffectedColor;
    }

    public void HideAllBoxes()
    {
        foreach (Box box in constraintBoxes)
            box.BoxGo.SetActive(false);
    }

    public void HideAllCurves()
    {
        foreach (Curve crv in constraintCurves)
            crv.CurveGo.SetActive(false);
    }

    public void HideAllPoints()
    {
        foreach (Point pt in constraintPoints)
            pt.PointGo.SetActive(false);
    }

    public void HideAllPlaceholders()
    {
        HideAllBoxes();
        HideAllCurves();
        HideAllPoints();
    }

    public void HighlightCurrentBox()
    {
        GameObject boxGo = GetCurrentBox().BoxGo;
        boxGo.SetActive(true);
        LineRenderer lr = boxGo.GetComponent<LineRenderer>();
        lr.startColor = MaterialHolder.AffectedColor;
        lr.endColor = MaterialHolder.AffectedColor;
    }

    public void HighlightCurrentCurve()
    {
        GameObject curveGo = GetCurrentCurve().CurveGo;
        curveGo.SetActive(true);
        LineRenderer lr = curveGo.GetComponent<LineRenderer>();
        lr.startColor = MaterialHolder.AffectedColor;
        lr.endColor = MaterialHolder.AffectedColor;
    }

    public void HighlightCurrentPoint()
    {
        GameObject pointGo = GetCurrentPoint().PointGo;
        pointGo.SetActive(true);
        pointGo.GetComponentInChildren<SpriteRenderer>().color = MaterialHolder.AffectedColor;
    }

    public void ToUnity()
    {
        foreach (Point p in constraintPoints)
            p.ToUnity();

        foreach (Curve c in constraintCurves)
            c.ToUnity();

        foreach (Box b in constraintBoxes)
            b.ToUnity();
    }

    public void BuildPlaceholders()
    {
        float heightOffset = -1;
        GameObject sprite = Resources.Load<GameObject>("PointPosSprite");

        foreach (Point p in constraintPoints)
            p.BuildPlaceholders(sprite, heightOffset);

        foreach (Curve c in constraintCurves)
            c.BuildPlaceholders(heightOffset);

        foreach (Box b in constraintBoxes)
            b.BuildPlaceholders();
    }
}


public class Curve
{
    public List<Point> controlPoints;

    Vector3? currentPos = null;
    int currentIdx;
    bool circular;

    GameObject curveGo;
    public GameObject CurveGo { get { return curveGo; } }

    public Curve()
    {
        controlPoints = new List<Point>();
    }

    public Curve(List<Point> _controlPoints)
    {
        controlPoints = _controlPoints;
    }

    public void BuildPlaceholders(float offset)
    {
        curveGo = new GameObject();
        curveGo.name = "curveConstraint_Object";
        var lr = curveGo.AddComponent<LineRenderer>();
        if (circular)
            lr.positionCount = controlPoints.Count + 1;
        else
            lr.positionCount = controlPoints.Count;

        Vector3[] pts;
        if (circular)
            pts = new Vector3[controlPoints.Count + 1];
        else
            pts = new Vector3[controlPoints.Count];

        for (int i = 0; i < controlPoints.Count; ++i)
            pts[i] = controlPoints[i].ToVector() + new Vector3(0, offset, 0);

        if (circular)
            pts[controlPoints.Count] = controlPoints[0].ToVector() + new Vector3(0, offset, 0);

        lr.SetPositions(pts);
        lr.startWidth = 0.01f;
        lr.endWidth = 0.01f;

        lr.material = MaterialHolder.LineMat;

        curveGo.SetActive(false);
    }

    public void ToUnity()
    {
        foreach(Point p in controlPoints)
        {
            p.ToUnity();
        }

        if (Vector3.Distance(controlPoints[0].ToVector(), controlPoints[controlPoints.Count -1].ToVector()) < 
            (Vector3.Distance(controlPoints[0].ToVector(), controlPoints[1].ToVector()) + 0.00001f) && controlPoints.Count > 2)
        {
            circular = true;
        }
    }

    public Vector3 GetCurrentPos()
    {
        try
        {
            if (currentPos == null)
            {
                currentPos = controlPoints[0].ToVector();
            }

        }
        catch
        {
            Debug.Log("No valid curve");
            return Vector3.zero;
        }
        return (Vector3)currentPos;
    }

    public Vector3 GetNextPos(float dist)
    {
        float distWalked = 0;

        if (currentPos == null)
        {
            currentPos = controlPoints[0].ToVector();
            currentIdx = 0;
            return (Vector3)currentPos;
        }

        while (true)
        {
            int nextIdx = currentIdx + 1;

            if (controlPoints.Count - 1 == currentIdx)
            {
                if (!circular)
                {
                    currentPos = controlPoints[currentIdx].ToVector();
                    return (Vector3)currentPos;
                }
                else
                {
                    nextIdx = 0;
                }
            }

            if (Vector3.Distance((Vector3)currentPos, controlPoints[nextIdx].ToVector()) > dist - distWalked)
            {
                currentPos += (controlPoints[nextIdx].ToVector() - currentPos) * (dist - distWalked);
                return (Vector3)currentPos;
            }
            else
            {
                distWalked += Vector3.Distance((Vector3)currentPos, controlPoints[nextIdx].ToVector());
                currentIdx = nextIdx;
            }
        }
    }

    public Vector3 GetPrevPos(float dist)
    {
        float distWalked = 0;

        if (currentPos == null)
        {
            currentIdx = controlPoints.Count - 1;
            currentPos = controlPoints[currentIdx].ToVector();
            return (Vector3)currentPos;
        }

        while(true)
        {
            int nextIdx = currentIdx - 1;

            if (currentIdx == 0)
            {
                if (!circular)
                {
                    currentPos = controlPoints[0].ToVector();
                    return (Vector3)currentPos;
                }
                else
                {
                    nextIdx = controlPoints.Count - 1;
                }
            }

            if(Vector3.Distance((Vector3)currentPos, controlPoints[nextIdx].ToVector()) > dist - distWalked)
            {
                currentPos += (controlPoints[nextIdx].ToVector() - currentPos) * (dist - distWalked);
                return (Vector3)currentPos;
            }
            else
            {
                distWalked += Vector3.Distance((Vector3)currentPos, controlPoints[nextIdx].ToVector());
                currentIdx = nextIdx;
            }
        }
    }
}

public class Point
{
    public float x;

    public float y;

    public float z;

    GameObject pointGo;
    public GameObject PointGo { get { return pointGo; } }

    public Point(float _x, float _y, float _z)
    {
        x = _x;
        y = _y;
        z = _z;
    }

    public Vector3 ToVector()
    {
        return new Vector3(x, y, z);
    }
    
    public void ToUnity()
    {
        float rhinoY = y;
        y = z + 1;
        z = rhinoY;
    }

    public void BuildPlaceholders(GameObject pointSprite, float offset)
    {
        pointGo = MonoBehaviour.Instantiate(pointSprite, new Vector3(x, y + offset, z), Quaternion.identity);
        pointGo.name = "pointConstraint_Object";
        pointGo.GetComponentInChildren<SpriteRenderer>().color = MaterialHolder.AffectedColor;

        pointGo.SetActive(false);
    }
}

public class Box
{
    public float minX;

    public float maxX;

    public float minY;

    public float maxY;

    public float minZ;

    public float maxZ;

    GameObject boxGo;
    public GameObject BoxGo { get { return boxGo; } }

    public Box(float _minX, float _maxX, float _minY, float _maxY, float _minZ, float _maxZ)
    {
        minX = _minX;
        maxX = _maxX;

        minY = _minY;
        maxY = _maxY;

        minZ = _minZ;
        maxZ = _maxZ;
    }

    public void BuildPlaceholders()
    {
        boxGo = new GameObject();
        boxGo.name = "boxConstraint_Object";
        var lr = boxGo.AddComponent<LineRenderer>();
        
        lr.positionCount = 16;

        Vector3[] pts = new Vector3[16];
        pts[0] = new Vector3(minX, minY, minZ);
        pts[1] = new Vector3(maxX, minY, minZ);
        pts[2] = new Vector3(maxX, minY, maxZ);
        pts[3] = new Vector3(minX, minY, maxZ);
        pts[4] = new Vector3(minX, minY, minZ);
        pts[5] = new Vector3(minX, maxY, minZ);

        pts[6] = new Vector3(minX, maxY, maxZ);
        pts[7] = new Vector3(minX, minY, maxZ);
        pts[8] = new Vector3(minX, maxY, maxZ);

        pts[9] = new Vector3(maxX, maxY, maxZ);
        pts[10] = new Vector3(maxX, minY, maxZ);
        pts[11] = new Vector3(maxX, maxY, maxZ);

        pts[12] = new Vector3(maxX, maxY, minZ);
        pts[13] = new Vector3(maxX, minY, minZ);
        pts[14] = new Vector3(maxX, maxY, minZ);

        pts[15] = new Vector3(minX, maxY, minZ);

        lr.SetPositions(pts);
        lr.startWidth = 0.01f;
        lr.endWidth = 0.01f;

        lr.material = MaterialHolder.LineMat;

        boxGo.SetActive(false);
    }

    public void ToUnity()
    {
        float rhinoMinY = minY;
        float rhinoMaxY = maxY;
        minY = minZ;
        maxY = maxZ;
        minZ = rhinoMinY;
        maxZ = rhinoMaxY;
    }
}
