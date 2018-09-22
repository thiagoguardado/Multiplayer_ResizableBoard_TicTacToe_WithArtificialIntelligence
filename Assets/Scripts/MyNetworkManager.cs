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
    public MatchData currentMatch;
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
        base.OnServerConnect(conn);

        currentConnectionsIDs.Add(conn.connectionId);
        currentMatch.AddPlayer(new MatchPlayer(possibleSymbols.possiblePlayerSprites[0].playerSymbol, Color.red, "", conn.connectionId));
        StartCoroutine(RefreshBroadcastInfo());

        NetworkGameLobbyView netView = GetComponent<NetworkGameLobbyView>();
        if (netView != null)
        {
            netView.UpdateView();
        }

        if (currentMatch.playersOnLobby.Length >= 4)
        {
            //Stop broadcast
            Discovery.StopBroadcast();
        }

    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        if (!NetworkServer.active)
        {
            Discovery.StopBroadcast();
        }
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        Debug.Log("Server disconnected");
        DiscconectAll();
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        currentConnectionsIDs.Remove(conn.connectionId);
        currentMatch.RemovePlayer(conn.connectionId);

        if (currentMatch.playersOnLobby.Length <4)
        {
            //restart broadcast
            StartCoroutine(RefreshBroadcastInfo());
        }

    }

    private IEnumerator RefreshBroadcastInfo()
    {
        Discovery.StopBroadcast();
        Discovery.broadcastData = MatchData.CreateMatchBroadcastData(currentMatch);
        yield return null;
        Discovery.StartAsServer();
    }


    public static void DiscconectAll()
    {
        singleton.StopClient();
        singleton.StopHost();
        singleton.StopMatchMaker();
        Discovery.StopBroadcast();
        Network.Disconnect();
    }
}
