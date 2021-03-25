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
using TMPro;
using UnityEngine;

/*
 * Tool parent class
 * 
 * Gives Interaction Input to Tools
 * 
 * Holds Active Toolitem and switsches through tools
 */

public class Tool : MonoBehaviour
{
    [SerializeField]
    string toolName = "";

    [SerializeField]
    bool activeTool = false;
    public bool ActiveTool { get { return activeTool; } }

    [SerializeField]
    float activeFontSize = 0.22f;
    public float ActiveFontSize { get { return activeFontSize; } }

    [SerializeField]
    float inactiveFontSize = 0.1f;
    public float InactiveFontSize { get { return inactiveFontSize; } }

    [SerializeField]
    Vector3 basePos;
    public Vector3 BasePos { get { return basePos; } }

    [SerializeField]
    Vector3 unfocusPos;

    [SerializeField]
    bool HighlightActivated = true;

    [SerializeField]
    bool resetFocusPosition = true;

    public Vector3 UnfocusPos { get { return unfocusPos; } }

    public string ToolName { get { return toolName; } }

    [SerializeField]
    List<ToolItem> toolItems = new List<ToolItem>();

    [SerializeField]
    bool activeHighlight = false;

    [SerializeField]
    GameObject largeToolPart;

    [SerializeField]
    GameObject smallToolPart;

    [SerializeField]
    bool highlightActiveItem = true;

    [SerializeField]
    Vector3 filterOffset;
    public Vector3 FilterOffset { get { return filterOffset; } }

    int activeIndex = 0;
    ToolItem activeItem;
    public ToolItem ActiveItem { get { return activeItem; } }

    int focusIndex = 0;
    ToolItem focusItem;

    Toolset toolSet;
    public Toolset ToolSet { get { return toolSet; } }

    public virtual void Initialize(Toolset _toolSet) 
    {
        toolSet = _toolSet;

        for (int i = 0; i < toolItems.Count; ++i)
        {
            if (toolItems[i] != null)
                toolItems[i].Initialize(toolSet);
            else
                Debug.LogError("NullItem in ToolItems in Tool " + ToolName);
        }

        activeItem = toolItems[0];
        focusItem = activeItem;

        InitColors();

    }

    public virtual void Reinitialize(Toolset _toolSet, List<bool> activityFlags, bool focus, bool activate)
    {

        if (activityFlags.Count != toolItems.Count)
        {
            Debug.Log("Wrong number of activity flags for " + toolName + ". Will not reinitialize");
            return;
        }

        List<ToolItem> allItems = new List<ToolItem>();

        foreach (ToolItem item in toolItems)
            allItems.Add(item);

        toolItems.Clear();

        for (int i = 0; i < allItems.Count; ++i)
        {
            if (activityFlags[i])
                toolItems.Add(allItems[i]);
            else
            {
                allItems[i].DeactivateItem();
                allItems[i].Disable();
                allItems[i].gameObject.SetActive(false);
            }
        }

        if (toolItems.Count > 0)
            activeItem = toolItems[0];
        else
            activeItem = null;

        focusItem = activeItem;

        RearrangeToolItems();

        if (activate && activeItem != null)
        {
            activeItem.ActivateItem();

            if (highlightActiveItem)
                activeItem.Highlight();
        }
        if (focus && activeItem != null && toolSet.Tools[0] == this)
            FocusTool();
    }

    public void Click()
    {
        if (activeItem != null)
            activeItem.Click();
    }

    public void Unclick()
    {
        if (activeItem != null)
            activeItem.Unclick();
    }

    public virtual void ActivateItem()
    {
 
        if (focusItem != null)
        {
            if (focusItem != activeItem)
            {
                activeItem.DeactivateItem();
                activeItem = focusItem;
                activeIndex = focusIndex;
            }

            activeItem.ActivateItem();

            if (HighlightActivated)
            {
                LowlightAll();
                activeItem.Highlight();
            }
        }
    }

    public void DeactivateItem()
    {

        if (activeItem != null)
            activeItem.DeactivateItem();
    }

    public virtual void NextOption()
    {
        if (activeItem != null)
            activeItem.NextOption();
    }

    public void NextOptionPressed() 
    {
        if (activeItem != null)
            activeItem.NextOptionPressed();
    }

    public virtual void PrevOption()
    {
        if (activeItem != null)
            activeItem.PrevOption();
    }

    public void PrevOptionPressed()
    {
        if (activeItem != null)
            activeItem.PrevOptionPressed();
    }

    public void ScrollUp() 
    {
        if (activeItem != null)
            activeItem.ScrollUp();
    }

    public void ScrollDown() 
    {
        if (activeItem != null)
            activeItem.ScrollDown();
    }

    void NextItem()
    {
        ++focusIndex;
        if (focusIndex >= toolItems.Count)
            focusIndex = 0;

        focusItem.UnfocusItem();
        focusItem = toolItems[focusIndex];

        ShiftItems();

        focusItem.FocusItem();

        if (activeHighlight)
        {
            LowlightAll();
            focusItem.Highlight();
        }
    }

    void PrevItem()
    {
        --focusIndex;
        if (focusIndex < 0)
            focusIndex = toolItems.Count - 1;

        focusItem.UnfocusItem();
        focusItem = toolItems[focusIndex];

        ShiftItems();

        focusItem.FocusItem();

        if (activeHighlight)
        {
            LowlightAll();
            focusItem.Highlight();
        }
    }

    void ShiftItems()
    {
        for (int i = 0; i < toolItems.Count; ++i)
        {
            toolItems[i].gameObject.transform.localPosition = basePos + filterOffset * (i - focusIndex);
        }
    }

    void LowlightAll()
    {
        foreach (ToolItem item in toolItems)
        {
            item.Lowlight();
        }
    }

    public void UnfocusTool()
    {
        if (resetFocusPosition)
        {
            focusItem.UnfocusItem();
            focusIndex = activeIndex;
            focusItem = activeItem;

            ShiftItems();
        }
        else
        {
            focusItem.UnfocusItem();
        }

        foreach(ToolItem item in toolItems)
        {
            item.Hide();
        }

        focusItem.Unhide();

        focusItem.SetFocus(unfocusPos, inactiveFontSize, false);

        largeToolPart.SetActive(false);
        smallToolPart.SetActive(true);
    }

    public void FocusTool()
    {
        foreach (ToolItem item in toolItems)
        {
            item.Unhide();
        }

        focusItem.SetFocus(basePos, activeFontSize, true);
        focusItem.FocusItem();

        largeToolPart.SetActive(true);
        smallToolPart.SetActive(false);
    }

    public virtual void Left()
    {
        PrevItem();
    }

    public virtual void Right()
    {
        NextItem();
    }

    public virtual void LeftPressed()
    { }

    public virtual void RightPressed()
    { }

    void InitColors()
    {
        if (highlightActiveItem)
            activeItem.Highlight();

        InitToolMaterials();
    }

    void InitToolMaterials()
    {
        largeToolPart.GetComponent<MeshRenderer>().material = MaterialHolder.ToolsetMaterial;
        largeToolPart.GetComponentInChildren<TextMeshPro>().faceColor = MaterialHolder.AffectedColor;

        smallToolPart.GetComponent<MeshRenderer>().material = MaterialHolder.ToolsetMaterial;
        smallToolPart.GetComponentInChildren<TextMeshPro>().faceColor = MaterialHolder.UnaffectedColor;
    }

    public void ChangeToolItemList(List<ToolItem> newItems)
    {
        toolItems = newItems;
        activeIndex = 0;
        focusIndex = 0;
        activeItem = toolItems[0];
        focusItem = toolItems[0];
    }

    public void RemoveToolItemAt(int i)
    {
        toolItems.RemoveAt(i);
    }

    public void RemoveToolItem(ToolItem item)
    {
        if (toolItems.Contains(item))
            toolItems.Remove(item);
    }

    void RearrangeToolItems()
    {
        for (int i = 0; i < toolItems.Count; ++i)
        {
            toolItems[i].gameObject.transform.localPosition = basePos + (i * filterOffset);
        }
        
        toolItems[0].UnfocusItem();

        UnfocusTool();
    }

    public void AddToolItems(List<ToolItem> items)
    {
        toolItems.AddRange(items);

        foreach (ToolItem item in items)
        {
            item.Initialize(toolSet);
        }

        if (toolItems.Count > 0)
            activeItem = toolItems[0];
        else
            activeItem = null;

        focusItem = activeItem;

        RearrangeToolItems();
    }

    public void Disable()
    {
        for (int i = toolItems.Count - 1; i >= 0; --i)
        {
            toolItems[i].Disable();
            toolItems[i].gameObject.SetActive(false);
        }
    }
}
