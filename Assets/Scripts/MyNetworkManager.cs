﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class MyNetworkManager : NetworkManager {


    public static MyNetworkDiscovery Discovery
    {
        get
        {
            return singleton.GetComponent<MyNetworkDiscovery>();
        }
    }

    public static bool isBroadcasting = false;

    public PlayerSymbols possibleSymbols;
    public NetworkMatchData currentMatch;
    public Dictionary<NetworkConnection,int> currentConnections = new Dictionary<NetworkConnection,int>();
    public string playerName = "Player";
    public NetworkPlayer currentPlayer;
    private int nextPlayerID = 0;

    public static bool isConnected { get; private set; }
    public bool lookingForMatches { get; private set; }

    private void Start()
    {
        isConnected = false;

        if (GameManager.networkType == NetworkType.Internet)
            StartMatchMaker();
    }

    #region Internet
    public void StartLookingForMatchesOnInternet()
    {
        lookingForMatches = true;
        StartMatchMaker();
        Debug.Log("start match maker");
    }

    public void ListMatchesOnInternet(NetworkMatch.DataResponseDelegate<List<MatchInfoSnapshot>> callback)
    {
        matchMaker.ListMatches(0, 10, "", false, 0, 0, callback);
    }

    public void CreateNewMatchOnInternet(string newMatchName, NetworkMatch.DataResponseDelegate<MatchInfo> callback)
    {
        currentMatch = new NetworkMatchData(newMatchName);
        isConnected = true;
        string broadcastData = NetworkMatchData.CreateMatchBroadcastData(currentMatch);

        matchMaker.CreateMatch(broadcastData, 4, true, "", "", "", 0, 0, callback);

        UnityEngine.SceneManagement.SceneManager.LoadScene("NetworkGameLobby");
    }

    public void ConnectToAMatchInternet(NetworkMatchData matchData)
    {
        matchMaker.JoinMatch((UnityEngine.Networking.Types.NetworkID)matchData.netID, "", "", "", 0, 0, OnMatchJoined);
        isConnected = true;
        lookingForMatches = false;
        UnityEngine.SceneManagement.SceneManager.LoadScene("NetworkGameLobby");
    }

    public override void OnMatchJoined(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        base.OnMatchJoined(success, extendedInfo, matchInfo);
    }

    #endregion

    #region LAN
    public void StartLookingForMatchesOnLAN()
    {
        lookingForMatches = true;
        Discovery.InitializeAsClient();
    }

    public void StartBroadcastingNewMatch(string newMatchName)
    {
        currentMatch = new NetworkMatchData(newMatchName);
        singleton.StartHost();
        isConnected = true;

        UnityEngine.SceneManagement.SceneManager.LoadScene("NetworkGameLobby");
    }

    public void ConnectToAMatchLAN(NetworkMatchData matchData)
    {
        singleton.StartClient();
        isConnected = true;
        lookingForMatches = false;
        UnityEngine.SceneManagement.SceneManager.LoadScene("NetworkGameLobby");
    }
    #endregion


    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        if (!NetworkServer.active)
        {
            Discovery.MyStopBroadcast();
        }

        currentPlayer = new NetworkPlayer(playerName, -1);
    }


    public override void OnClientDisconnect(NetworkConnection conn)
    {

        base.OnClientDisconnect(conn);

        Debug.Log("Server disconnected");

        currentPlayer = null;

        ClientDisconnectAll();

        GameManager.Instance.ReturnToTitleScreen();

    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        bool clientCanConnect = (GameManager.GameState == GameState.Menu);

        base.OnServerConnect(conn);

        if (clientCanConnect)
        {
            currentMatch.AddPlayer(new NetworkPlayer(ChooseRandomNewPlayerSymbol(), nextPlayerID));
            currentConnections.Add(conn, nextPlayerID);
            nextPlayerID++;

            NetworkGameLobbyView netView = GetComponent<NetworkGameLobbyView>();
            if (netView != null)
            {
                netView.UpdateView();
            }

            if (GameManager.networkType == NetworkType.LAN)
            {
                StartCoroutine(RefreshBroadcastInfoLAN());
                if (currentMatch.playersOnLobby.Length >= 4)
                {
                    //Stop broadcast
                    Discovery.MyStopBroadcast();
                }
            }
            else if (GameManager.networkType == NetworkType.Internet)
            {
                if (currentMatch.playersOnLobby.Length >= 4)
                {
                    //Stop broadcast
                    ChangeMatchVisibility(false);
                }
            }
        }
        else
        {
            var networks = FindObjectsOfType<NetworkGameLobby>();
            foreach (var net in networks)
            {
                if (net.isLocalPlayer)
                {
                    net.TargetForceDisconnect(conn);
                    break;
                }
            }
        }
    }


    public override void OnServerDisconnect(NetworkConnection conn)
    {
        if (currentConnections.ContainsKey(conn))
        {

            // if in game, remove player from current match and change to next player
            if (GameManager.GameState == GameState.GameStarted)
            {
                NetworkPlayer disconnectedPlayer = new NetworkPlayer();
                for (int i = 0; i < currentMatch.playersOnLobby.Length; i++)
                {
                    if (currentMatch.playersOnLobby[i].playerID == currentConnections[conn])
                    {
                        disconnectedPlayer = currentMatch.playersOnLobby[i];
                        break;
                    }
                }

                BoardManager.Instance.TriggerPlayerRemoval(disconnectedPlayer.playerSymbol);

                if (GameManager.networkType == NetworkType.LAN)
                {
                    if (currentMatch.playersOnLobby.Length < 4)
                    {
                        //restart broadcast
                        StartCoroutine(RefreshBroadcastInfoLAN());
                    }
                }
                else if (GameManager.networkType == NetworkType.Internet)
                {
                    if (currentMatch.playersOnLobby.Length < 4)
                    {
                        //restart broadcast
                        ChangeMatchVisibility(true);
                    }
                }

            }

            currentMatch.RemovePlayer(currentConnections[conn]);
            currentConnections.Remove(conn);

            base.OnServerDisconnect(conn);

        }

    }

    private IEnumerator RefreshBroadcastInfoLAN()
    {
        Discovery.MyStopBroadcast();
        Discovery.broadcastData = NetworkMatchData.CreateMatchBroadcastData(currentMatch);
        yield return null;
        Discovery.RestartAsServer();
    }



    public static void ClientDisconnectAll()
    {
        singleton.StopClient();
        singleton.StopHost();
        singleton.StopMatchMaker();
        Network.Disconnect();
        isConnected = false;
    }

    public static void ServerDiscconectAll()
    {
        singleton.StopClient();
        singleton.StopHost();
        singleton.StopMatchMaker();
        Discovery.MyStopBroadcast();
        Network.Disconnect();
        isConnected = false;
    }

    public PlayerSymbol ChooseRandomNewPlayerSymbol()
    {
        PlayerSymbol[] currentSymbols = new PlayerSymbol[currentMatch.playersOnLobby.Length];
        for (int i = 0; i < currentMatch.playersOnLobby.Length; i++)
        {
            currentSymbols[i] = currentMatch.playersOnLobby[i].playerSymbol;
        }

        return GameManager.FindDifferentPlayerSymbol(currentSymbols);
    }

    public void ChangeMatchVisibility(bool visible)
    {
        matchMaker.SetMatchAttributes(matchInfo.networkId, visible, 0, (success,extendedInfo) => { Debug.Log("set match attributes: " + success); });
    }


    public override void OnClientError(NetworkConnection conn, int errorCode)
    {
        base.OnClientError(conn, errorCode);

        Debug.LogError("Client error: " + errorCode);
    }

    public override void OnServerError(NetworkConnection conn, int errorCode)
    {
        base.OnServerError(conn, errorCode);

        Debug.LogError("Server error: " + errorCode);
    }
}
