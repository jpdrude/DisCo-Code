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
using TMPro;
using UnityEditor;
using UnityEngine;

/*
 * FPS Controller for Toolset interaction
 * Input handling
 */

public class FPSController : MonoBehaviour
{
    //local variables
    #region
    Texture2D crosshairImage;

    bool toolTrackUp = false;
    bool toolTrackDown = false;
    bool toolTrackLeft = false;
    bool toolTrackRight = false;

    bool toolEnter = false;
    bool toolPressed = false;
    bool toolLeftRightDown = false;

    bool steeringTrackUp = false;
    bool steeringTrackDown = false;

    bool steeringMouseRight = false;
    bool steeringEnter = false;

    bool hidden = false;

    static bool noClip = false;
    static bool fly = false;
    bool anaglyph = false;

    [SerializeField]
    Toolset toolset;
    #endregion


    //MonoBehaviour Methods
    #region
    void Start()
    {
        PlacementReferences.PlayMode = PlacementReferences.Mode.FPS;

        crosshairImage = Resources.Load<Texture2D>("Materials/" + MaterialHolder.MatSet + "/Toolset/crosshair");

        //toolset.Initialize();

        //List<bool> toolFlags = new List<bool>() { false, true, false, true, true, false, true, true };
        //toolset.Initialize(toolFlags);
    }

    void Update()
    {
        DetectControlInputs();

        HideToolsetCheck();
    }

    private void OnGUI()
    {
        if (PlacementReferences.Aiming && !hidden)
        {
            float xMin = (Screen.width / 2) - (crosshairImage.width / 2);
            float yMin = (Screen.height / 2) - (crosshairImage.height / 2);
            GUI.DrawTexture(new Rect(xMin, yMin, crosshairImage.width, crosshairImage.height), crosshairImage);
        }
    }
    #endregion


    void DetectControlInputs()
    {
        bool up = Input.GetKeyDown(KeyCode.UpArrow);
        bool down = Input.GetKeyDown(KeyCode.DownArrow);
        bool left = Input.GetKeyDown(KeyCode.LeftArrow);
        bool right = Input.GetKeyDown(KeyCode.RightArrow);

        bool leftPressed = Input.GetKey(KeyCode.LeftArrow);
        bool rightPressed = Input.GetKey(KeyCode.RightArrow);

        bool click = Input.GetKeyDown(KeyCode.Mouse0);
        bool unclick = Input.GetKeyUp(KeyCode.Mouse0);

        bool rightClick = Input.GetKeyDown(KeyCode.Mouse1);
        bool rightClickPressed = Input.GetKey(KeyCode.Mouse1);
        bool middleClick = Input.GetKeyDown(KeyCode.Mouse2);
        bool middleClickPressed = Input.GetKey(KeyCode.Mouse2);

        bool activateItem = Input.GetKeyDown(KeyCode.Return);

        bool scrollUp = false;
        bool scrollDown = false;

        if (Input.mouseScrollDelta.y > 0)
        {
            scrollUp = true;
        }
        else if (Input.mouseScrollDelta.y < 0)
        {
            scrollDown = true;
        }

        if (up) toolset.Up();

        if (down) toolset.Down();

        if (left) toolset.Left();

        if (right) toolset.Right();

        if (leftPressed) toolset.LeftPressed();

        if (rightPressed) toolset.RightPressed();

        if (click) toolset.Click();

        if (unclick) toolset.Unclick();

        if (rightClick) toolset.NextOption();

        if (rightClickPressed) toolset.NextOptionPressed();

        if (middleClick) toolset.PrevOption();

        if (middleClickPressed) toolset.PrevOptionPressed();

        if (activateItem) toolset.ActiveteItem();

        if (scrollUp) for (int i = 0; i < 10; ++i) toolset.ScrollUp();

        if (scrollDown) for (int i = 0; i < 10; ++i) toolset.ScrollDown();
    }

    
    void HideToolsetCheck()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            hidden = !hidden;

            foreach (MeshRenderer mr in toolset.GetComponentsInChildren<MeshRenderer>(true))
            {
                mr.forceRenderingOff = hidden;
            }
        }
    }
}
