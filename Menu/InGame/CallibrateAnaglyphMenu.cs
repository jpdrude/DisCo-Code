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
using UnityEngine.UI;
using Valve.Newtonsoft.Json;
using System.Text;
using System.IO;

using System;

/*
 * Callibrates Anaglyph Colors and Brightness
 * Callibrates eye Distance
 * 
 * Only works with Color Anaglyph
 */

public class CallibrateAnaglyphMenu : MonoBehaviour
{
    [SerializeField]
    Slider rightColorSlider;

    [SerializeField]
    Slider leftColorSlider;

    [SerializeField]
    Slider leftLightnessSlider;

    [SerializeField]
    Slider rightLightnessSlider;

    [SerializeField]
    Slider eyeDistSlider;

    Material anaglyphMat;

    float leftCol = 0;
    float rightCol = 0;

    float leftLight = 1;
    float rightLight = 1;

    float eyeOffset = 0;

    AnaglyphizerC anaglyph = null;
    public AnaglyphizerC Anaglyph { get { return anaglyph; } set { anaglyph = value; } }

    public void Start()
    {
        InitMat();

        rightColorSlider.value = rightCol;
        leftColorSlider.value = leftCol;

        rightLightnessSlider.value = rightLight;
        leftLightnessSlider.value = leftLight;

        eyeDistSlider.value = eyeOffset;
    }

    void InitMat()
    {
        if (anaglyph == null)
            anaglyph = Camera.main.GetComponent<AnaglyphizerC>();

        if (anaglyphMat == null)
            anaglyphMat = anaglyph.anaglyphMat;
    }

    public void ChangeRightColor()
    {
        SetRightColor(rightColorSlider.value);
    }

    void SetRightColor(float val)
    {
        anaglyphMat.SetVector("_Balance_Left_B", new Vector4(val, 0, 0, 0));

        rightCol = val;
    }

    public void ChangeLeftColor()
    {
        SetLeftColor(leftColorSlider.value);
    }

    void SetLeftColor(float val)
    {
        float z = anaglyphMat.GetVector("_Balance_Right_B").z;
        anaglyphMat.SetVector("_Balance_Right_B", new Vector4(0, val, z, 0));

        leftCol = val;
    }

    public void ChangeLeftLightness()
    {
        SetLeftLightness(leftLightnessSlider.value);
    }

    void SetLeftLightness(float val)
    {
        anaglyphMat.SetVector("_Balance_Left_R", new Vector4(0, 0.7f * val, 0.3f * val, 0));

        leftLight = val;
    }

    public void ChangeRightLightness()
    {
        SetRightLightness(rightLightnessSlider.value);
    }

    void SetRightLightness(float val)
    {
        anaglyphMat.SetVector("_Balance_Right_G", new Vector4(0, val, 0, 0));

        float y = anaglyphMat.GetVector("_Balance_Right_B").y;
        anaglyphMat.SetVector("_Balance_Right_B", new Vector4(0, y, val, 0));

        rightLight = val;
    }

    public void ChangeEyeOffset()
    {
        SetEyeOffset(eyeDistSlider.value);
    }

    void SetEyeOffset(float val)
    {
        anaglyph.ChangeEyeDistance(val);
        eyeOffset = val;
    }

    void ResetColors()
    {
        anaglyphMat.SetVector("_Balance_Left_R", new Vector4(0, 0.7f, 0.3f, 0));
        anaglyphMat.SetVector("_Balance_Left_G", new Vector4(0, 0, 0, 0));
        anaglyphMat.SetVector("_Balance_Left_B", new Vector4(0, 0, 0, 0));

        anaglyphMat.SetVector("_Balance_Right_R", new Vector4(0, 0, 0, 0));
        anaglyphMat.SetVector("_Balance_Right_G", new Vector4(0, 1, 0, 0));
        anaglyphMat.SetVector("_Balance_Right_B", new Vector4(0, 0, 1, 0));

        rightColorSlider.value = 0;
        leftColorSlider.value = 0;

        rightLightnessSlider.value = 1;
        leftLightnessSlider.value = 1;

        rightCol = 0;
        leftCol = 0;

        rightLight = 1;
        leftLight = 1;

        eyeOffset = 0;

        anaglyph.ChangeEyeDistance(0);
    }

    public void ResetAndSave()
    {
        ResetColors();
        SaveSettings();
    }

    public void SaveSettings()
    {
        string path = Application.dataPath + "/Resources/colorConfig.json";

        AnaglyphColors colors = new AnaglyphColors(leftCol, rightCol, leftLight, rightLight, eyeOffset);

        StringBuilder colorsSb;
        colorsSb = new StringBuilder(JsonConvert.SerializeObject(colors, Formatting.Indented));

        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            using (StreamWriter sw = new StreamWriter(stream))
            {
                sw.Write(colorsSb);
            }
        }

        gameObject.SetActive(false);
    }

    public void LoadAnaglyph()
    {
        InitMat();
        ResetColors();

        AnaglyphColors colors = null;

        string path = Application.dataPath + "/Resources/colorConfig.json";

        try
        {
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    colors = JsonConvert.DeserializeObject<AnaglyphColors>(sr.ReadToEnd());
                }
            }
        }
        catch
        {
            colors = null;
        }

        if (colors != null)
        {
            SetRightColor(colors.rightColor);
            SetLeftColor(colors.leftColor);
            SetRightLightness(colors.rightLight);
            SetLeftLightness(colors.leftLight);
            SetEyeOffset(colors.offset);
        }
    }
}



public class AnaglyphColors
{
    public float leftColor;
    public float rightColor;

    public float leftLight;
    public float rightLight;

    public float offset;

    public AnaglyphColors(float _leftCol, float _rightCol, float _leftLight, float _rightLight, float _offset)
    {
        leftColor = _leftCol;
        rightColor = _rightCol;

        leftLight = _leftLight;
        rightLight = _rightLight;

        offset = _offset;
    }
}
