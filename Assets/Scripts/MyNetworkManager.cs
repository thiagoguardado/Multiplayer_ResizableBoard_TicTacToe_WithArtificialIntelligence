using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public enum NetworkType {LAN, Internet}

public class MyNetworkManager : NetworkManager {


    public static MyNetworkDiscovery Discovery
    {
        get
        {
            return singleton.GetComponent<MyNetworkDiscovery>();
        }
    }

    public static NetworkType NetworkType { get; set; }


    public PlayerSymbols possibleSymbols;
    private MatchData currentMatch;

    public bool isConnected { get; private set; }

    
    private void Start()
    {
        isConnected = false;
    }

    private void Update()
    {

    }

    public void StartLookingForMatches()
    {
        Discovery.Initialize();
        Discovery.StartAsClient();
    }

    public void StartBroadcastingNewMatch(string newMatchName)
    {
        currentMatch = new MatchData(newMatchName, singleton.networkAddress);
        singleton.StartHost();
        isConnected = true;
        UnityEngine.SceneManagement.SceneManager.LoadScene("NetworkGameLobby");

    }

    public void ConnectToAMatch(MatchData matchData)
    {
        singleton.networkAddress = matchData.serverAddress;
        singleton.StartClient();
        isConnected = true;

        UnityEngine.SceneManagement.SceneManager.LoadScene("NetworkGameLobby");
    }


    public override void OnServerConnect(NetworkConnection conn)
    {
        currentMatch.AddPlayer(new MatchData.SpriteAndColor(possibleSymbols.possiblePlayerSprites[2], Color.yellow));

        Discovery.StopBroadcast();
        Discovery.broadcastData = MatchData.CreateMatchBroadcastData(currentMatch);
        Discovery.StartAsServer();

    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {

        currentMatch.RemovePlayer();

        Discovery.StopBroadcast();
        Discovery.broadcastData = MatchData.CreateMatchBroadcastData(currentMatch);
        Discovery.StartAsServer();
    }
    


}
