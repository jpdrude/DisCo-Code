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

using Valve.Newtonsoft.Json;
using System.IO;
using System.Text;

/*
 * Toolitem: Export Field
 * Exports wasp field for wasp field aggregations
 * 
 * uses left and right to set field range
 */

public class TIExportField : ToolItem
{
    [SerializeField]
    TIDrawField drawField;

    [SerializeField]
    GameObject pointSprite;

    Region gameArea;

    int xRange;
    int yRange;
    int zRange;

    static float resolution = 0.2f;
    public static float Resolution { get { return resolution; } set { resolution = value; } }

    FieldPoint[,,] fieldContainer;

    bool showField = false;
    public bool ShowField { get { return showField; } }

    public override void Initialize(Toolset _toolSet)
    {
        base.Initialize(_toolSet);

        gameArea = InitializeGameArea.GameArea;

        xRange = (int)(System.Math.Floor((gameArea.MaxX - gameArea.MinX) / resolution));
        yRange = (int)(System.Math.Floor((gameArea.MaxY - gameArea.MinY) / resolution));
        zRange = (int)(System.Math.Floor((gameArea.MaxZ - gameArea.MinZ) / resolution));

        fieldContainer = new FieldPoint[xRange, yRange, zRange];

        GameObject spriteParent = new GameObject();
        spriteParent.name = "FieldSprites";

        for (int x = 0; x < xRange; ++x)
        {
            for (int y = 0; y < yRange; ++y)
            {
                for (int z = 0; z < zRange; ++z)
                {
                    FieldPoint fp = new FieldPoint(new Vector3(resolution * x + gameArea.MinX + resolution / 2, resolution * y + gameArea.MinY + resolution / 2,
                                                               resolution * z + gameArea.MinZ + resolution / 2));

                    fieldContainer[x, y, z] = fp;
                    fp.sprite = Instantiate(pointSprite, fp.pos, Quaternion.identity, spriteParent.transform);
                    fp.sprite.SetActive(false);
                }
            }
        }

        FieldPoint.Range = resolution * 3;
    }

    public override void ActivateItem()
    {
        if (!showField)
        {
            ToContainer();
            ShowPoints();
            showField = true;
        }
        else
        {
            ExportField();
            ResetTool();
        }
    }

    public override void DeactivateItem()
    {
        base.DeactivateItem();

        ResetTool();
    }

    public void RightPressed()
    {
        ScaleRange(Time.deltaTime);
    }

    public void LeftPressed()
    {
        ScaleRange(-Time.deltaTime);
    }

    void ResetTool()
    {
        HidePoints();

        showField = false;
    }

    void ScaleRange(float value)
    {
        FieldPoint.Range += value;

        if (FieldPoint.Range <= 0)
        {
            FieldPoint.Range -= value;
            return;
        }

        for (int x = 0; x < xRange; ++x)
            for (int y = 0; y < yRange; ++y)
                for (int z = 0; z < zRange; ++z)
                    fieldContainer[x, y, z].ChangeColor();
    }

    public void PutInGrid(Vector3 cloudPoint)
    {
        int _minX = (int)(System.Math.Floor((cloudPoint.x - gameArea.MinX - FieldPoint.Range) / resolution));
        int _maxX = (int)(System.Math.Floor((cloudPoint.x - gameArea.MinX + FieldPoint.Range) / resolution));

        int _minY = (int)(System.Math.Floor((cloudPoint.y - gameArea.MinY - FieldPoint.Range) / resolution));
        int _maxY = (int)(System.Math.Floor((cloudPoint.y - gameArea.MinY + FieldPoint.Range) / resolution));

        int _minZ = (int)(System.Math.Floor((cloudPoint.z - gameArea.MinZ - FieldPoint.Range) / resolution));
        int _maxZ = (int)(System.Math.Floor((cloudPoint.z - gameArea.MinZ + FieldPoint.Range) / resolution));

        for (int x = _minX; x <= _maxX; ++x)
            for (int y = _minY; y <= _maxY; ++y)
                for (int z = _minZ; z <= _maxZ; ++z)
                    if (x >= 0 && x < xRange && y >= 0 && y < yRange && z >= 0 && z < zRange)
                    {
                        FieldPoint fp = fieldContainer[x, y, z];

                        if (fp.GetValue(cloudPoint) > fp.Value)
                            fp.closestPt = cloudPoint;
                    }

    }

    void HidePoints()
    {
        for (int x = 0; x < xRange; ++x)
            for (int y = 0; y < yRange; ++y)
                for (int z = 0; z < zRange; ++z)
                    fieldContainer[x, y, z].sprite.SetActive(false);
    }

    void ToContainer()
    {
        List<Vector3> particles = drawField.ParticleContainer;

        foreach (Vector3 pt in particles)
        {
            PutInGrid(pt);
        }
    }

    void ShowPoints()
    {
        int count = 0;

        for (int x = 0; x < xRange; ++x)
        {
            for (int y = 0; y < yRange; ++y)
            {
                for (int z = 0; z < zRange; ++z)
                {
                    FieldPoint fp = fieldContainer[x, y, z];
                    ++count;
                    float value = fp.Value;
                    fp.ChangeColor();
                }
            }
        }
    }

    void ExportField()
    {
        List<float> values = new List<float>();
        List<Vector3> points = new List<Vector3>();

        for (int x = 0; x < xRange; ++x)
            for (int y = 0; y < yRange; ++y)
                for (int z = 0; z < zRange; ++z)
                {
                    FieldPoint fp = fieldContainer[x, y, z];
                    values.Add(fp.Value);
                    points.Add(fp.pos);
                }

        WaspField waspField = new WaspField(resolution, fieldContainer.GetLength(0), fieldContainer.GetLength(1), fieldContainer.GetLength(2), values, points);

        StringBuilder sb = new StringBuilder(JsonConvert.SerializeObject(waspField, Formatting.Indented));

        string path = SaveLoad.SavePath + "/Fields/Field_" + SaveLoad.BuildDateString() + ".json";

        Directory.CreateDirectory(Path.GetDirectoryName(path));

        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            using (StreamWriter sw = new StreamWriter(stream))
            {
                sw.Write(sb);
            }
        }
    }
}

public class FieldPoint
{
    static float range;
    public static float Range
    {
        get { return range; }
        set { range = value; }
    }

    public Vector3 pos;

    public Vector3? closestPt = null;

    public GameObject sprite = null;

    public float Value
    {
        get
        {
            if (closestPt == null)
                return 0;

            float val = Vector3.Distance((Vector3)closestPt, pos) / range;

            return 1 - val;
        }
    }

    public FieldPoint(Vector3 _pos)
    {
        pos = _pos;
    }

    public float GetValue(Vector3 pt)
    {
        float val = Vector3.Distance(pt, pos) / range;

        return 1 - val;
    }

    public void ChangeColor()
    {
        if (sprite != null)
        {
            sprite.GetComponent<SpriteRenderer>().color = Color.HSVToRGB(Value / 3, 1, 1);

            if (Value < 0.001f)
                sprite.SetActive(false);
            else
                sprite.SetActive(true);
        }
    }
}

public class WaspField
{
    public float resolution;

    public string name;

    public int[] count;

    public List<float> values;

    public List<float[]> pts = new List<float[]>();

    public List<int> boundaries = new List<int>();

    public WaspField(float res, int countX, int countY, int countZ, List<float> vals, List<Vector3> points)
    {
        resolution = res;
        name = "DisCoField_" + SaveLoad.BuildDateString();
        count = new int[3] { countX, countZ, countY };
        values = vals;

        foreach (Vector3 pt in points)
        {
            pts.Add(new float[3] { pt.x, pt.z, pt.y });
        }
    }
}
