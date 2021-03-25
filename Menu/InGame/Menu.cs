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
using System.Diagnostics.Contracts;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;

/*
 * Ingame Main Menu
 *      - Gives Acces to more settings
 *      - Quit
 *      - End Session
 *      - Change Player (If Multiplayer with different Players)
 */

public class Menu : MonoBehaviour
{
    [SerializeField]
    GameObject menu;

    [SerializeField]
    GameObject quit;

    [SerializeField]
    GameObject reallyQuit;

    [SerializeField]
    GameObject lobby;

    [SerializeField]
    GameObject reallyLobby;

    [SerializeField]
    CallibrateAnaglyphMenu anaglyphMenu;

    [SerializeField]
    GameObject graphicsMenu;

    public FPSController myFpsController { get; set; }

    public FirstPersonController fpsController { get; set; }

    public ViveControllerInput viveController { get; set; }

    bool state = false;

    // Update is called once per frame
    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MenuToggle();
        }
    }

    public void MenuToggle()
    { 

        state = !state;

        quit.SetActive(true);
        reallyQuit.SetActive(false);

        lobby.SetActive(true);
        reallyLobby.SetActive(false);

        menu.SetActive(state);
        Cursor.visible = state;
        Cursor.lockState = CursorLockMode.None; 

        if (anaglyphMenu.gameObject.activeSelf && !state)
        {
            anaglyphMenu.SaveSettings();
            anaglyphMenu.gameObject.SetActive(state);
        }

        if (graphicsMenu.activeSelf && !state)
        {
            graphicsMenu.SetActive(state);
        }

        if (state)
        {
            if (!BoltNetwork.IsRunning)
                Time.timeScale = 0;

            if (myFpsController == null)
                myFpsController = FindObjectOfType<FPSController>();

            if (fpsController == null)
                fpsController = FindObjectOfType<FirstPersonController>();

            if (viveController == null)
                viveController = FindObjectOfType<ViveControllerInput>();

            if (viveController != null)
                viveController.enabled = false;

            if (fpsController != null)
                fpsController.enabled = false;

            if (myFpsController != null)
                myFpsController.enabled = false;

        }   
        else
        {
            if (!BoltNetwork.IsRunning)
                Time.timeScale = 1;

            if (myFpsController == null)
                myFpsController = FindObjectOfType<FPSController>();

            if (fpsController == null)
                fpsController = FindObjectOfType<FirstPersonController>();

            if (viveController == null)
                viveController = FindObjectOfType<ViveControllerInput>();

            if (viveController != null)
                viveController.enabled = true;

            if (fpsController != null)
                fpsController.enabled = true;

            if (myFpsController != null)
                myFpsController.enabled = true;
        }
    }

    public void ReallQuit()
    {
        reallyQuit.SetActive(true);
        quit.SetActive(false);
        Cursor.visible = true;
    }

    public void ReallyToLobby()
    {
        reallyLobby.SetActive(true);
        lobby.SetActive(false);
        Cursor.visible = true;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void DestroyPlayer()
    {
        if (ControllerReferences.Controller != null)
        {
            Destroy(ControllerReferences.Controller);
        }

        if (BoltNetwork.IsRunning)
            BoltNetwork.Destroy(ControllerReferences.MultiplayerCharacter);
    }

    public void EndSession()
    {
        DestroyPlayer();

        if (BoltNetwork.IsRunning)
        {
            BoltLauncher.Shutdown();
        }

        if (myFpsController != null)
            myFpsController.enabled = true;

        if (fpsController != null)
            fpsController.enabled = true;

        if (viveController != null)
            viveController.enabled = true;

        Time.timeScale = 1;

        SceneManager.LoadScene("DisCo-Lobby");
    }
}
