using UnityEngine;
using System.Collections;

public class NetworkConnect : MonoBehaviour {

    private const string serverName = "AngryBots_VR_Screener";
    private bool refreshing = false;
    private HostData[] hosts;
    public bool isServer;

    void Update()
    {
        UpdateHosts();
    }


    void StartServer()
    {
        Network.InitializeServer(32, 25001, true);
        MasterServer.RegisterHost(serverName, "AngryBotsVR Server", "This is a tutorial game");
    }

    void OnMasterServerEvent(MasterServerEvent mse)
    {
        if (mse == MasterServerEvent.RegistrationSucceeded)
        {
            Debug.Log("Registered to server");
        }
    }

    void RefreshHostList()
    {
        MasterServer.RequestHostList(serverName);
        refreshing = true;
    }

    void UpdateHosts()
    {
        if (refreshing)
        {
            if (MasterServer.PollHostList().Length > 0)
            {
                refreshing = false;
                hosts = MasterServer.PollHostList();
            }
        }
    }

    public void OnGUI()
    {
        if (Network.isClient || Network.isServer)
        {
            return;
        }

        if (isServer)
        {
            if (GUI.Button(new Rect(20, 20, 100, 60), "Start Server"))
            {
                Debug.Log("Starting server");
                StartServer();
            }
        }
        else
        {
            if (GUI.Button(new Rect(20, 20, 100, 60), "Refresh Host"))
            {
                Debug.Log("Refreshing");
                RefreshHostList();
            }

            if (hosts != null)
            {
                for (int i = 0; i < hosts.Length; i++)
                {
                    if (GUI.Button(new Rect(130, 20 * i, 120, 40), hosts[i].gameName))
                    {
                        Network.Connect(hosts[i]);
                    }
                }
            }
        }
    }
}
