using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkGameLobby : NetworkBehaviour {

    public MyNetworkManager mynetworkManager { get; private set; }
    float timer = 1f;
    float refreshTime = 1f;

    private void Awake()
    {
        DontDestroyOnLoad(this);

    }


    void Start()
    {
        mynetworkManager = NetworkManager.singleton.GetComponent<MyNetworkManager>();
       
    }

    void Update()
    {
        if (isLocalPlayer)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                timer = refreshTime;
                if (isServer)
                {
                    ServerUpdate();
                }
                else if (isClient)
                {
                    ClientUpdate();
                }
            }
        }

    }

    private void ClientUpdate()
    {
        UpdatePlayersOnClient();
    }


    private void ServerUpdate()
    {
        UpdatePlayersNameOnServer();

    }

    private void UpdatePlayersNameOnServer()
    {
        for (int i = 0; i < mynetworkManager.currentMatch.playersOnLobby.Length; i++)
        {
            if (mynetworkManager.currentMatch.playersOnLobby[i].playerName == "")
            {

                RpcAskPlayerForName(i);
            }
        }
    }

    private void UpdatePlayersOnClient()
    {
        if (timer <= 0f)
        {
            CmdUpdateMatchData();
        }
        timer -= Time.deltaTime;
    }


    [ClientRpc]
    private void RpcAskPlayerForName(int index)
    {
        CmdUpdatePlayerName(mynetworkManager.playerName, index);
    }

    [Command]
    private void CmdUpdatePlayerName(string playerName, int index)
    {
        mynetworkManager.currentMatch.playersOnLobby[index].playerName = playerName;
    }


    [ClientRpc]
    private void RpcSendMatchDataToClients(MatchData currentMatchData)
    {
        mynetworkManager.currentMatch = currentMatchData;
    }

    [Command]
    private void CmdUpdateMatchData()
    {
        RpcSendMatchDataToClients(mynetworkManager.currentMatch);
    }
    
}
