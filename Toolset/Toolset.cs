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

/*
 * Toolset as Container for different tools
 * 
 * Gives Interaction Input to Tools
 * 
 * Holds Active Tool and switsches through tools
 *      - Highlights Active and Keeps Size
 */

public class Toolset : MonoBehaviour
{
    [SerializeField]
    List<Tool> allTools = new List<Tool>();

    [SerializeField]
    Vector3 firstToolPos;

    [SerializeField]
    Vector3 largeToolStep;

    [SerializeField]
    Vector3 smallToolStep;

    [SerializeField]
    GameObject toolTop;

    [SerializeField]
    GameObject toolBottom;

    List<Tool> tools = new List<Tool>();
    public List<Tool> Tools { get { return tools; } }

    [SerializeField]
    Tool activeTool;
    public Tool ActiveTool { get { return activeTool; } }

    [SerializeField]
    GameObject controllerTarget;
    public GameObject ControllerTarget { get { return controllerTarget; } }

    private Vector3 toolTopPos;
    public Vector3 ToolTopPos { get { return toolTopPos; } }

    Tool focusTool;
    int focusIndex = 0;


    public void Initialize()
    {
        foreach (Tool tool in allTools)
        {
            tools.Add(tool);
        }

        CalculateTop();

        foreach (Tool tool in tools)
            tool.Initialize(this);

        focusTool = tools[0];

        toolTop.GetComponent<MeshRenderer>().material = MaterialHolder.ToolsetMaterial;
        toolBottom.GetComponent<MeshRenderer>().material = MaterialHolder.ToolsetMaterial;

        if (activeTool != null)
            activeTool.ActivateItem();

        if (BoltNetwork.IsRunning && BoltNetwork.IsClient)
            ControllerSpawned.Create().Send();
    }

    public void Initialize(List<bool> activityFlags)
    {
        if (allTools.Count != activityFlags.Count)
        {
            Debug.LogError("Wrong number of activity flags for Toolset!");
            Initialize();
            return;
        }


        for (int i = 0; i < activityFlags.Count; ++i)
        {
            if (activityFlags[i])
                tools.Add(allTools[i]);
            else
            {
                allTools[i].gameObject.SetActive(false);
                allTools[i].Disable();
            }
        }

        Vector3 toolPos = firstToolPos;

        CalculateTop();

        for (int i = tools.Count - 1; i >= 0; --i)
        {
            tools[i].Initialize(this);
        }

        if (activeTool != null && tools.Contains(activeTool))
        {
            activeTool.ActivateItem();
        }
        else
        {
            foreach (Tool tool in tools)
                if (tool.ActiveTool)
                {
                    activeTool = tool;
                    tool.ActivateItem();
                    break;
                }

            if (!tools.Contains(activeTool))
                activeTool = null;
        }

        focusTool = tools[0];

        RearrangeToolset();

        focusTool.FocusTool();

        if (BoltNetwork.IsRunning && BoltNetwork.IsClient)
            ControllerSpawned.Create().Send();
    }

    void CalculateTop()
    {
        toolTopPos = largeToolStep + (tools.Count - 2) * smallToolStep + new Vector3(-0.05f, 0, -0.05f);
    }


    void NextTool()
    {
        focusTool.UnfocusTool();

        ++focusIndex;
        if (focusIndex >= tools.Count)
            focusIndex = 0;

        focusTool = tools[focusIndex];

        focusTool.FocusTool();

        RearrangeToolset();
    }

    void PrevTool()
    {
        focusTool.UnfocusTool();

        --focusIndex;
        if (focusIndex < 0)
            focusIndex = tools.Count - 1;

        focusTool = tools[focusIndex];
        
        focusTool.FocusTool();

        RearrangeToolset();
    }

    void RearrangeToolset()
    {
        Vector3 pos = firstToolPos;

        for (int i = tools.Count - 1; i >= 0; --i)
        {
            tools[i].transform.localPosition = pos;

            if (i == focusIndex)
                pos += largeToolStep;
            else
                pos += smallToolStep;
        }

        toolTop.transform.localPosition = pos;
    }

    void UnfocusAllTools()
    {
        foreach (Tool tool in tools)
        {
            tool.UnfocusTool();
        }
    }

    //ControllerInputs
    #region
    public void Up()
    {
        PrevTool();
    }

    public void Down()
    {
        NextTool();
    }

    public void Left()
    {
        focusTool.Left();
    }

    public void Right()
    {
        focusTool.Right();
    }

    public void LeftPressed()
    {
        focusTool.LeftPressed();
    }

    public void RightPressed()
    {
        focusTool.RightPressed();
    }

    public void Click()
    {
        if (activeTool == null) return;

        activeTool.Click();
    }

    public void Unclick()
    {
        if (activeTool == null) return;

        activeTool.Unclick();
    }

    public void NextOption()
    {
        if (activeTool == null) return;

        activeTool.NextOption();
    }

    public void NextOptionPressed()
    {
        if (activeTool == null) return;

        activeTool.NextOptionPressed();
    }

    public void PrevOption()
    {
        if (activeTool == null) return;

        activeTool.PrevOption();
    }

    public void PrevOptionPressed()
    {
        if (activeTool == null) return;

        activeTool.PrevOptionPressed();
    }

    public void ScrollUp()
    {
        if (activeTool == null) return;

        activeTool.ScrollUp();
    }

    public void ScrollDown()
    {
        if (activeTool == null) return;

        activeTool.ScrollDown();
    }

    public void ActiveteItem()
    {
        if (focusTool.ActiveTool)
        {
            if (activeTool != null && focusTool != activeTool)
                activeTool.DeactivateItem();

            activeTool = focusTool;
        }

        focusTool.ActivateItem();
    }

    public void DeactivateItem()
    {
        if (activeTool != null)
        {
            activeTool.DeactivateItem();
            activeTool = null;
        }
    }
    #endregion
}
