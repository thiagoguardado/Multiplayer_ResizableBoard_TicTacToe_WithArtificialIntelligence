using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetworkMenu : MonoBehaviour {

    public PlayerSymbols possibleSymbols;
    public NetworkGameSelectionMatch matchPanelPrefab;
    public Transform matchListPanelParent;
    public PopupPanel popup;
    public Text playerNameDisplay;

    public float refreshListTime = 1f;

    private float refreshListTimer = 0f;
    private MyNetworkManager myNetwork;
    private List<NetworkMatchData> _matchesToDisplay = new List<NetworkMatchData>();
    private List<NetworkGameSelectionMatch> _currentMatchesDisplayed = new List<NetworkGameSelectionMatch>();

    private void Awake()
    {
        myNetwork = NetworkManager.singleton.gameObject.GetComponent<MyNetworkManager>();
    }

    private void Start()
    {
        myNetwork.StartLookingForMatches();
        playerNameDisplay.text = myNetwork.playerName;
    }

    private void Update()
    {

        if (!myNetwork.isConnected)
        {
            refreshListTimer -= Time.deltaTime;

            if (refreshListTimer <= 0f)
            {
                refreshListTimer = refreshListTime;
                RefreshGames();
                
            }

        }
    }

    #region Display
    private void RefreshGames()
    {
        _matchesToDisplay.Clear();

        foreach (NetworkBroadcastResult result in MyNetworkManager.Discovery.broadcastsReceived.Values)
        {
            string matchData = Encoding.Unicode.GetString(result.broadcastData);

            NetworkMatchData match;
            
            try
            {
                match = new NetworkMatchData(result, possibleSymbols,"");
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
                continue;
            }

            _matchesToDisplay.Add(match);
        }


        List<NetworkGameSelectionMatch> updatedGames = new List<NetworkGameSelectionMatch>();

        for (int i = 0; i < _matchesToDisplay.Count; i++)
        {

            bool found = false;
            for (int j = 0; j < _currentMatchesDisplayed.Count; j++)
            {
                if (_currentMatchesDisplayed[j].thisMatchData.matchName == _matchesToDisplay[i].matchName)
                {
                    _currentMatchesDisplayed[j].Setup(_matchesToDisplay[i]);
                    updatedGames.Add(_currentMatchesDisplayed[j]);
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                updatedGames.Add(AddMatchToList(_matchesToDisplay[i]));
            }
        }


        NetworkGameSelectionMatch[] gamesToDelete = _currentMatchesDisplayed.Except(updatedGames).ToArray();
        foreach (var item in gamesToDelete)
        {
            RemoveMatchFromList(item);
        }

    }

    public void BackButton()
    {
        // cancel network
        MyNetworkManager.Discovery.MyStopBroadcast();
        SceneManager.LoadScene("TitleScreen");
    }

    public void CreateNewMatch()
    {
        popup.OpenNewMatch(ConfirmNewMatchCreation);
    }


    public void ConfirmNewMatchCreation(string newMatchName)
    {
        if (ValidateMatchName(newMatchName))
        {
            popup.ClosePopup();
            myNetwork.StartBroadcastingNewMatch(newMatchName);
        }
    }


    private bool ValidateMatchName(string matchName)
    {
        return true;
    }

    private bool ValidatePlayerName(string playerName)
    {
        return playerName != "";
    }

    public void ChangePlayerName()
    {
        popup.OpenChangeName(myNetwork.playerName, ConfirmNewPlayerName);
    }

    public void ConfirmNewPlayerName(string newPlayerName)
    {
        if (ValidatePlayerName(newPlayerName))
        {
            popup.ClosePopup();
            myNetwork.playerName = newPlayerName;
            playerNameDisplay.text = myNetwork.playerName;
        }
    }

    private NetworkGameSelectionMatch AddMatchToList(NetworkMatchData matchData)
    {
        NetworkGameSelectionMatch m = Instantiate(matchPanelPrefab, matchListPanelParent);
        m.Setup(matchData);
        _currentMatchesDisplayed.Add(m);
        return m;
    }

    private void RemoveMatchFromList(NetworkGameSelectionMatch matchCard)
    {
        _currentMatchesDisplayed.Remove(matchCard);
        Destroy(matchCard);
    }

    private void ClearMatchList()
    {
        for (int i = 1; i < matchListPanelParent.childCount; i++)
        {
            Destroy(matchListPanelParent.GetChild(i).gameObject);
        }
    }

    #endregion


}

[System.Serializable]
public class NetworkMatchData
{

    public string matchName;
    [SerializeField] public NetworkPlayer[] playersOnLobby = new NetworkPlayer[0];
    public string serverAddress;
    public NetworkMatchData() { }


    public NetworkMatchData(string matchName, string serverAddress)
    {
        this.matchName = matchName;
        this.serverAddress = NetworkManager.singleton.networkAddress;
    }

    public NetworkMatchData(NetworkBroadcastResult broadcastResult, PlayerSymbols playerSymbols, string playerName) 
    {
        string[] splitted = Encoding.Unicode.GetString(broadcastResult.broadcastData).Split('_');
        matchName = splitted[0];
        string[] splittedPlayers = splitted[1].Split('|');
        List<NetworkPlayer> list = new List<NetworkPlayer>();
        for (int i = 0; i < splittedPlayers.Length; i++)
        {

            string colorHex = splittedPlayers[i].Substring(splittedPlayers[i].IndexOf("c=")+2,6);
            string symbolName = splittedPlayers[i].Substring(splittedPlayers[i].IndexOf("s=") + 2, splittedPlayers[i].IndexOf("c=") - 2);
            string name = splittedPlayers[i].Substring(splittedPlayers[i].IndexOf("n=") + 2, splittedPlayers[i].Length - splittedPlayers[i].IndexOf("n=") - 2);
            
            Color color;
            if (!ColorUtility.TryParseHtmlString("#" + colorHex, out color))
            {
                throw new System.Exception("broadcast color data error");
            }


            PlayerSymbol symbol = PlayerSymbol.Circle;
            bool findSymbol = false;
            for (int j = 0; j < playerSymbols.possiblePlayerSprites.Count; j++)
            {
                if (playerSymbols.possiblePlayerSprites[j].playerSymbol.ToString() == symbolName)
                {

                    symbol = playerSymbols.possiblePlayerSprites[j].playerSymbol;
                    findSymbol = true;
                    break;
                }
            }
            if (!findSymbol)
            {
                throw new System.Exception("broadcast sprite data error");
            }
            list.Add(new NetworkPlayer(symbol, color, name, -1));
        }
        playersOnLobby = list.ToArray();
        serverAddress = broadcastResult.serverAddress;
    }

    public static string CreateMatchBroadcastData(string matchName, Player[] players, int nextPlayerID)
    {

        string playersData = "";
        for (int i = 0; i < players.Length; i++)
        {
            if (i > 0)
                playersData += "|";

            playersData += players[i].playerSymbolAndSprite.playerSymbol + ColorUtility.ToHtmlStringRGB(players[i].color);
            playersData += "s=" + players[i].playerSymbolAndSprite.playerSymbol + "c=" + ColorUtility.ToHtmlStringRGB(players[i].color) + "n=" + players[i].playerName;

        }

        return matchName +  "_" + playersData;

    }

    public static string CreateMatchBroadcastData(NetworkMatchData matchData)
    {

        string playersData = "";
        for (int i = 0; i < matchData.playersOnLobby.Length; i++)
        {
            if (i > 0)
                playersData += "|";

            playersData += "s=" + matchData.playersOnLobby[i].playerSymbol + "c=" + ColorUtility.ToHtmlStringRGB(matchData.playersOnLobby[i].color) + "n=" + matchData.playersOnLobby[i].playerName;
        }

        return matchData.matchName + "_" + playersData;

    }

    public void AddPlayer(NetworkPlayer player)
    {
        if (playersOnLobby != null)
        {
            List<NetworkPlayer> m = playersOnLobby.ToList();
            m.Add(player);
            playersOnLobby = m.ToArray();
        }
        else {
            playersOnLobby = new NetworkPlayer[] { player };
        }
    }


    public void RemovePlayer(int playerID)
    {
        List<NetworkPlayer> m = playersOnLobby.ToList<NetworkPlayer>();
        int i = -1;
        for (i = 0; i < m.Count; i++)
        {
            if (m[i].playerID == playerID)
                break;
        }
        if (i == -1)
        {
            Debug.Log("Could not remove player from lobby");
        }
        else {
            m.RemoveAt(i);
            playersOnLobby = m.ToArray();
        }
       
    }
}

[System.Serializable]
public class NetworkPlayer
{
    public PlayerSymbol playerSymbol;
    public Color color;
    public string playerName;
    public int playerID;

    public NetworkPlayer() { }


    public NetworkPlayer(PlayerSymbol playerSymbol, Color color, string playerName, int playerID)
    {
        this.playerSymbol = playerSymbol;
        this.color = color;
        this.playerName = playerName;
        this.playerID = playerID;
    }

    public NetworkPlayer(string playerName, int playerID)
    {
        this.playerName = playerName;
        this.playerID = playerID;
    }

    public NetworkPlayer(int playerID, bool random)
    {
        if(!random) 
        {
            this.playerSymbol = PlayerSymbol.Circle;
            this.color = Color.red;

        } else 
        {
            this.playerSymbol = (PlayerSymbol)UnityEngine.Random.Range(0, Enum.GetValues(typeof(PlayerSymbol)).Length);
            this.color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, 1);
        }
        this.playerName = "";
        this.playerID = playerID;
    }

    public NetworkPlayer(PlayerSymbol playerSymbol, int playerID)
    {
        this.playerSymbol = playerSymbol;
        this.color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, 1);
        this.playerName = "";
        this.playerID = playerID;
    }
}