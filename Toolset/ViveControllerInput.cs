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
using Valve.VR;

/*
 * VR Controller for toolset interaction
 * Input handling
 */

public class ViveControllerInput : MonoBehaviour
{
    [SerializeField]
    Toolset toolset;

    [SerializeField]
    LineRenderer lineRenderer;

    bool aiming = false;

    //local variables
    #region
    //string trackPadClick_left = "TrackPadClick_Left";
    //string trackPadClick_right = "TrackPadClick_Right";

    //string trackPadPos_left = "TrackPadPos_Left";
    //string trackPadPos_right = "TrackPadPos_Right";

    //string triggerClick_left = "TriggerClick_Left";
    //string triggerClick_right = "TriggerClick_Right";

    SteamVR_Action_Boolean trackPadRight = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("TrackPadClick_Right", true);
    SteamVR_Action_Boolean trackPadLeft = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("TrackPadClick_Left", true);

    SteamVR_Action_Vector2 trackPadRightPos = SteamVR_Input.GetAction<SteamVR_Action_Vector2>("TrackPadPos_Right", true);
    SteamVR_Action_Vector2 trackPadLeftPos = SteamVR_Input.GetAction<SteamVR_Action_Vector2>("TrackPadPos_Left", true);

    SteamVR_Action_Boolean triggerRight = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("TriggerClick_Right", true);
    SteamVR_Action_Boolean triggerLeft = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("TriggerClick_Left", true);

    SteamVR_Input_Sources sources = SteamVR_Input_Sources.Any;
    #endregion


    //MonoBehaviour methods
    #region
    void Start()
    {
        PlacementReferences.PlayMode = PlacementReferences.Mode.VR;
        KillTimer.VisDist = 0.1f;

        var particleRender = GameObject.Find("ParticleSystem").GetComponent<ParticleSystemRenderer>();
        particleRender.allowRoll = false;
        particleRender.gameObject.AddComponent<DisableParticleRoll>();
    }
    

    void Update()
    {
        DetectControlInputs();

        if (PlacementReferences.Aiming && !aiming)
        {
            lineRenderer.enabled = true;
            aiming = true;
        }
        else if (!PlacementReferences.Aiming && aiming)
        {
            lineRenderer.enabled = false;
            aiming = false;
        }
    }
    #endregion

    void DetectControlInputs()
    {
        //bool leftPadDown = SteamVR_Input.GetStateDown(trackPadClick_left, SteamVR_Input_Sources.Any, true);
        //bool leftPadPressed = SteamVR_Input.GetState(trackPadClick_left, SteamVR_Input_Sources.Any, true);
        //Vector2 leftPadPos = SteamVR_Input.GetVector2(trackPadPos_left, SteamVR_Input_Sources.Any, true);

        bool leftPadDown = trackPadLeft.GetStateDown(sources);
        bool leftPadPressed = trackPadLeft.GetState(sources);
        Vector2 leftPadPos = trackPadLeftPos.GetAxis(sources);


        bool left_up = false;
        bool left_down = false;
        bool left_left = false;
        bool left_right = false;

        if (leftPadDown && leftPadPos.y > 0.5f) left_up = true;
        if (leftPadDown && leftPadPos.y < -0.5f) left_down = true;
        if (leftPadDown && leftPadPos.x < -0.5f) left_left = true;
        if (leftPadDown && leftPadPos.x > 0.5f) left_right = true;


        bool left_leftPressed = false;
        bool left_rightPressed = false;

        if (leftPadPressed && leftPadPos.x < -0.5f) left_leftPressed = true;
        if (leftPadPressed && leftPadPos.x > 0.5f) left_rightPressed = true;




        //bool left_triggerDown = SteamVR_Input.GetStateDown(triggerClick_left, SteamVR_Input_Sources.Any, true);
        //bool left_triggerUp = SteamVR_Input.GetStateUp(triggerClick_left, SteamVR_Input_Sources.Any, true);

        bool left_triggerDown = triggerLeft.GetStateDown(sources);
        bool left_triggerUp = triggerLeft.GetStateUp(sources);


        //bool rightPadDown = SteamVR_Input.GetStateDown(trackPadClick_right, SteamVR_Input_Sources.Any, true);
        //bool rightPadPressed = SteamVR_Input.GetState(trackPadClick_right, SteamVR_Input_Sources.Any, true);
        //Vector2 rightPadPos = SteamVR_Input.GetVector2(trackPadPos_right, SteamVR_Input_Sources.Any, true);

        bool rightPadDown = trackPadRight.GetStateDown(sources);
        bool rightPadPressed = trackPadRight.GetState(sources);
        Vector2 rightPadPos = trackPadRightPos.GetAxis(sources);


        bool right_left = false;
        bool right_right = false;

        if (rightPadDown && rightPadPos.x < -0.5f) right_left = true;
        if (rightPadDown && rightPadPos.x > 0.5f) right_right = true;


        bool right_upPressed = false;
        bool right_downPressed = false;
        bool right_leftPressed = false;
        bool right_rightPressed = false;

        if (rightPadPressed && rightPadPos.y > 0.5f) right_upPressed = true;
        if (rightPadPressed && rightPadPos.y < -0.5f) right_downPressed = true;
        if (rightPadPressed && rightPadPos.x < -0.5f) right_leftPressed = true;
        if (rightPadPressed && rightPadPos.x > 0.5f) right_rightPressed = true;


        //bool right_triggerDown = SteamVR_Input.GetStateDown(triggerClick_right, SteamVR_Input_Sources.Any, true);
        //bool right_triggerUp = SteamVR_Input.GetStateUp(triggerClick_right, SteamVR_Input_Sources.Any, true);

        bool right_triggerDown = triggerRight.GetStateDown(sources);
        bool right_triggerUp = triggerRight.GetStateUp(sources);




        if (left_up) toolset.Up();

        if (left_down) toolset.Down();

        if (left_left) toolset.Left();

        if (left_right) toolset.Right();

        if (left_leftPressed) toolset.LeftPressed();

        if (left_rightPressed) toolset.RightPressed();

        if (left_triggerDown) toolset.ActiveteItem();


        if (right_triggerDown) toolset.Click();

        if (right_triggerUp) toolset.Unclick();

        if (right_right) toolset.NextOption();

        if (right_rightPressed) toolset.NextOptionPressed();

        if (right_left) toolset.PrevOption();

        if (right_leftPressed) toolset.PrevOptionPressed();

        if (right_upPressed) toolset.ScrollUp();

        if (right_downPressed) toolset.ScrollDown();
    }

    //Controller actions
    #region
    /*
void ToolControllerAction()
{
    toolTrackPos = SteamVR_Input.GetVector2(trackPadPos_tool, SteamVR_Input_Sources.Any, true);
    toolTrackDown = SteamVR_Input.GetStateDown(trackPadClick_tool, SteamVR_Input_Sources.Any, true);
    toolTrack = SteamVR_Input.GetState(trackPadClick_tool, SteamVR_Input_Sources.Any, true);

    if (SteamVR_Input.GetStateDown(triggerClick_tool, SteamVR_Input_Sources.Any, true))
    {
        toolSet.TriggerDown();
    }
    else if (toolTrackDown && toolTrackPos.y < 0.5f && toolTrackPos.y > -0.5f)
    {
        if (toolTrackPos.x < -0.5f)
        {
            toolSet.PadRight(Button.release);
            toolSet.PadLeft(Button.press);
            toolPressed = true;
        }
        else if (toolTrackPos.x > 0.5f)
        {
            toolSet.PadLeft(Button.release);
            toolSet.PadRight(Button.press);
            toolPressed = true;
        }
    }
    else if (toolTrackDown && toolTrackPos.x < 0.5f && toolTrackPos.x > -0.5f)
    {
        if (toolTrackPos.y < -0.5f)
        {
            toolSet.PadDown();
        }
        else if (toolTrackPos.y > 0.5f)
        {
            toolSet.PadUp();
        }
    }

    if (!toolTrack && toolPressed)
    {
        toolSet.PadLeft(Button.release);
        toolSet.PadRight(Button.release);
        toolPressed = false;
    }
}


void SteeringControllerAction()
{
    steeringTrackPos = SteamVR_Input.GetVector2(trackPadPos_steering, SteamVR_Input_Sources.Any, true);
    steeringTrackDown = SteamVR_Input.GetStateDown(trackPadClick_steering, SteamVR_Input_Sources.Any, true);
    steeringTrack = SteamVR_Input.GetState(trackPadClick_steering, SteamVR_Input_Sources.Any, true);

    if (SteamVR_Input.GetStateDown(triggerClick_steering, SteamVR_Input_Sources.Any, true))
    {
        //behaviourTool.Engage();
        steeringTriggerDown = true;
    }
    else if (steeringTrackDown && steeringTrackPos.y < 0.5f && steeringTrackPos.y > -0.5f)
    {
        if (steeringTrackPos.x < -0.5f)
        {
            //behaviourTool.ChangePart(PlacementBehaviour.ToolsetControls.Left);
        }
        else if (steeringTrackPos.x > 0.5f)
        {
            //behaviourTool.ChangePart(PlacementBehaviour.ToolsetControls.Right);
        }
    }
    else if (steeringTrack && steeringTrackPos.x < 0.5f && steeringTrackPos.x > -0.5f)
    {
        if (steeringTrackPos.y < -0.5f)
        {
            //behaviourTool.ScrollPart(PlacementBehaviour.ToolsetControls.Down);
        }
        else if (steeringTrackPos.y > 0.5f)
        {
            //behaviourTool.ScrollPart(PlacementBehaviour.ToolsetControls.Up);
        }
    }


    if (!SteamVR_Input.GetState(triggerClick_steering, SteamVR_Input_Sources.Any, true) && steeringTriggerDown)
    {
        //behaviourTool.DisEngage();
        steeringTriggerDown = false;
    }
}
*/
    #endregion
}
