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

using Bolt.Matchmaking;
using System;
using System.Collections.Generic;
using UdpKit;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;

/*
 * Network Menu
 *      - Hosting
 *      - Joining
 *      - Session
 *      - etc.
 */

public class NetworkMenu : Bolt.GlobalEventListener
{
    
    [SerializeField]
    Button joinGameButtonPrefab;

    [SerializeField]
    GameObject serverListPanel;

    [SerializeField]
    float buttonSpacing;

    List<Button> joinButtons = new List<Button>();

    private static string serverName = "unknownGame";
    public static string ServerName
    {
        get { return serverName; }
        set { serverName = value; }
    }

    public void HostServer()
    {
        BoltLauncher.StartServer();
    }

    public void StartClient()
    {
        BoltLauncher.StartClient();
    }

    public void AbortClient()
    {
        BoltLauncher.Shutdown();
    }

    public void StartSinglePlayer()
    {
        SceneManager.LoadScene("DisCo-Scene");
    }

    public override void BoltStartDone()
    {
        if (BoltNetwork.IsServer)
        {
            BoltMatchmaking.CreateSession(serverName);
            BoltNetwork.LoadScene("DisCo-Scene");
        }
    }

    public override void SessionListUpdated(Map<Guid, UdpSession> sessionList)
    {
        ClearSessions();

        foreach(var session in sessionList)
        {
            UdpSession photonSession = session.Value as UdpSession;

            Button joinGameBtn = Instantiate(joinGameButtonPrefab);
            joinGameBtn.transform.SetParent(serverListPanel.transform);
            joinGameBtn.transform.localPosition = new Vector3(0, 0, 0);
            joinGameBtn.gameObject.SetActive(true);

            joinGameBtn.gameObject.GetComponentInChildren<Text>().text = photonSession.HostName;

            joinGameBtn.onClick.AddListener(() => JoinGame(photonSession));

            joinButtons.Add(joinGameBtn);
        }

        float startPos = (float)Math.Floor(joinButtons.Count / 2f) * buttonSpacing;
        for (int i = 0; i < joinButtons.Count; ++i)
        {
            joinButtons[i].transform.localPosition = new Vector3(0, startPos - (i * buttonSpacing), 0);
        }
    }

    private void JoinGame(UdpSession photonSession)
    {
        BoltMatchmaking.JoinSession(photonSession);
    }

    void ClearSessions()
    {
        foreach (Button btn in joinButtons)
        {
            Destroy(btn.gameObject);
        }

        joinButtons.Clear();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
