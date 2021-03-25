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
using UnityEditor;
using UnityEngine.UI;

/*
 * Class is used to rebuild UI Elements, that lost their Attributes
 * Came up twice
 */

public class ReplaceUI : MonoBehaviour
{
    static Sprite UISprite;

    static Sprite Background;

    static Sprite Knob;

    static Sprite InputFieldBackground;

    static Sprite UIMask;

    static Sprite dropdownArrow;

    static Sprite check;

    static Font font;

    [MenuItem("GameObject/UI/ReplaceUIElements")]
    public static void ReplaceUIElements()
    {
        LoadResources();


        List<Image> images = new List<Image>();
        List<Text> text = new List<Text>();
        List<Dropdown> dropdown = new List<Dropdown>();
        List<InputField> inputFields = new List<InputField>();
        List<Slider> sliders = new List<Slider>();
        List<Toggle> toggles = new List<Toggle>();

        GameObject[] selection = Selection.gameObjects;

        foreach (GameObject selected in selection)
        {
            images.AddRange(selected.GetComponentsInChildren<Image>(true));
            text.AddRange(selected.GetComponentsInChildren<Text>(true));
            dropdown.AddRange(selected.GetComponentsInChildren<Dropdown>(true));
            inputFields.AddRange(selected.GetComponentsInChildren<InputField>(true));
            sliders.AddRange(selected.GetComponentsInChildren<Slider>(true));
            toggles.AddRange(selected.GetComponentsInChildren<Toggle>(true));
        }

        foreach (Image img in images)
            img.sprite = UISprite;

        foreach (Text t in text)
            t.font = font;

        foreach (InputField input in inputFields)
            input.GetComponent<Image>().sprite = InputFieldBackground;

        foreach (Toggle toggle in toggles)
            try 
            {
                foreach(Transform t in toggle.GetComponentsInChildren<Transform>(true))
                    if (t.gameObject.name == "Checkmark")
                        t.GetComponent<Image>().sprite = check; 
            }
            catch { Debug.Log("no Checkmark found for Toggle: " + toggle.gameObject.name); }

        foreach (Dropdown drop in dropdown)
        {
            try 
            { 
                foreach(Transform t in drop.GetComponentsInChildren<Transform>(true))
                    if (t.gameObject.name == "Arrow")
                        t.GetComponent<Image>().sprite = dropdownArrow; 
                    else if (t.gameObject.name == "Viewport")
                        t.GetComponent<Image>().sprite = UIMask;
                    else if (t.gameObject.name == "Scrollbar")
                        t.GetComponent<Image>().sprite = Background;
                    else if (t.gameObject.name == "Item Background")
                        t.GetComponent<Image>().sprite = null;
                    else if (t.gameObject.name == "Item Checkmark")
                        t.GetComponent<Image>().sprite = check;
            }
            catch { Debug.Log("Error in DropDown: " + drop.gameObject.name); }

            /*
            try { drop.transform.Find("Viewport").GetComponent<Image>().sprite = UIMask; }
            catch { Debug.Log("No Viewport found in DropDown: " + drop.gameObject.name) ; }

            try { drop.transform.Find("Scrollbar").GetComponent<Image>().sprite = Background; }
            catch { Debug.Log("No Scrollbar found in DropDown: " + drop.gameObject.name); }

            try { drop.transform.Find("Item Background").GetComponent<Image>().sprite = null; }
            catch { Debug.Log("No Item Background found in DropDown: " + drop.gameObject.name); }

            try { drop.transform.Find("Item Checkmark").GetComponent<Image>().sprite = check; }
            catch { Debug.Log("No Item Checkmark found in Dropdown: " + drop.gameObject.name); }
            */
        }

        foreach (Slider slider in sliders)
        {
            foreach(Transform t in slider.GetComponentsInChildren<Transform>(true))
            {
                if (t.gameObject.name == "Background")
                    t.GetComponent<Image>().sprite = Background;
                else if (t.gameObject.name == "Handle")
                    t.GetComponent<Image>().sprite = Knob;
            }

            /*
            try { slider.transform.Find("Background").GetComponent<Image>().sprite = Background; }
            catch { Debug.Log("No Background found in Slider: " + slider.gameObject.name); }

            try { slider.transform.Find("Handle").GetComponent<Image>().sprite = Knob; }
            catch { Debug.Log("No Handle found in Slider: " + slider.gameObject.name); }
            */
        }
    }

    static void LoadResources()
    {
        UISprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        Background = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
        UIMask = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UIMask.psd");
        InputFieldBackground = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/InputFieldBackground.psd");
        Knob = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
        dropdownArrow = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/DropdownArrow.psd");
        check = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Checkmark.psd");
        font = Resources.GetBuiltinResource<Font>("Arial.ttf");
    }
}
