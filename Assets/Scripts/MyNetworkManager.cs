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
    public MatchData currentMatch { get; private set; }
    public List<int> currentConnectionsIDs = new List<int>();
    public string playerName = "Player";

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
        currentConnectionsIDs.Add(conn.connectionId);
        currentMatch.AddPlayer(new MatchData.MatchPlayer(possibleSymbols.possiblePlayerSprites[0], Color.red, ""));
        RefreshBroadcastInfo();

        NetworkGameLobbyView netView = GetComponent<NetworkGameLobbyView>();
        if (netView != null)
        {
            netView.UpdateView();
        }

    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        currentConnectionsIDs.Remove(conn.connectionId);
        currentMatch.RemovePlayer();
        RefreshBroadcastInfo();
    }

    private void RefreshBroadcastInfo()
    {
        Discovery.StopBroadcast();
        Discovery.broadcastData = MatchData.CreateMatchBroadcastData(currentMatch);
        Discovery.StartAsServer();
    }


}
