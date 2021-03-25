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

using UnityEngine;
using TMPro;

/*
 * Abstract class for specific ToolItems to inherit from
 * 
 * Those should:
 *      - Hold tool functionality
 *      - interact with game
 *      - Execute  commands based on inputs
 */

public abstract class ToolItem : MonoBehaviour
{
    [SerializeField]
    string itemName;

    Toolset toolSet;
    public Toolset ToolSet { get { return toolSet; } }

    public string ItemName { get { return itemName; } }

    public virtual void Initialize(Toolset _toolSet)
    {
        toolSet = _toolSet;
        Lowlight();
    }

    public virtual void Click() {}

    public virtual void Unclick() { }

    public virtual void ActivateItem()
    {
        Debug.Log(itemName + " has been clicked.");
    }

    public virtual void DeactivateItem() { Lowlight(); }

    public virtual void FocusItem() { }

    public virtual void UnfocusItem() { }

    public virtual void NextOption() { }

    public virtual void NextOptionPressed() { }

    public virtual void PrevOption() { }

    public virtual void PrevOptionPressed() { }

    public virtual void ScrollUp() { }

    public virtual void ScrollDown() { }

    public virtual void Highlight()
    {
        
        try
        {
            GetComponent<TextMeshPro>().faceColor = MaterialHolder.AffectedColor;
        }
        catch
        {
            if (ItemName != "Rule")
                Debug.Log("Item: " + ItemName + " doesn't have TMPro attached");
        }
    }

    public virtual void Lowlight()
    {
        
        try
        {
            GetComponent<TextMeshPro>().faceColor = MaterialHolder.UnaffectedColor;
        }
        catch
        {
            if (ItemName != "Rule")
                Debug.Log("Item: " + ItemName + " doesn't have TMPro attached");
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public virtual void Unhide()
    {
        gameObject.SetActive(true);
    }

    public virtual void SetFocus(Vector3 pos, float size, bool focus)
    {
        TextMeshPro tmPro = GetComponent<TextMeshPro>();
        if (tmPro != null)
            tmPro.fontSize = size;

        gameObject.transform.localPosition = pos;
    }

    public virtual void Disable() { }
}
