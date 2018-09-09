using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetworkMenu : MonoBehaviour {

    public PlayerSymbols possibleSymbols;
    public NetworkGameSelectionMatch matchPanelPrefab;
    public Transform matchListPanelParent;
    public GameObject popupMenu;
    public InputField matchNameInput;

    public float refreshListTime = 1f;

    private float refreshListTimer = 0f;
    private MyNetworkManager myNetwork;
    private List<MatchData> _matchesToDisplay = new List<MatchData>();
    private List<NetworkGameSelectionMatch> _currentMatchesDisplayed = new List<NetworkGameSelectionMatch>();

    private void Awake()
    {
        myNetwork = NetworkManager.singleton.gameObject.GetComponent<MyNetworkManager>();
    }

    private void Start()
    {
        myNetwork.StartLookingForMatches();
    }

    private void Update()
    {

        if (!myNetwork.isConnected)
        {
            refreshListTimer -= Time.deltaTime;

            if (refreshListTimer <= 0f)
            {
                RefreshGames();
                refreshListTimer = refreshListTime;
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
            MatchData match;
            
            try
            {
                match = new MatchData(result, possibleSymbols);
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


    public void CreateNewMatch()
    {
        popupMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(matchNameInput.gameObject);
    }


    public void ConfirmNewMatchCreation()
    {
        if (ValidateMatchName(matchNameInput.text))
        {
            popupMenu.SetActive(false);
            myNetwork.StartBroadcastingNewMatch(matchNameInput.text);
        }
    }


    private bool ValidateMatchName(string matchName)
    {
        return true;
    }

    private NetworkGameSelectionMatch AddMatchToList(MatchData matchData)
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


public class MatchData
{

    public class SpriteAndColor
    {
        public SymbolAndSprite symbolAndSprite;
        public Color color;

        public SpriteAndColor(SymbolAndSprite symbolSprite, Color color)
        {
            this.symbolAndSprite = symbolSprite;
            this.color = color;
        }
    }

    public string matchName;
    public List<SpriteAndColor> playersOnLobby = new List<SpriteAndColor>();
    public string serverAddress;



    public MatchData(string matchName, string serverAddress)
    {
        this.matchName = matchName;
        this.serverAddress = NetworkManager.singleton.networkAddress;
    }

    public MatchData(NetworkBroadcastResult broadcastResult, PlayerSymbols playerSymbols) 
    {
        string[] splitted = Encoding.Unicode.GetString(broadcastResult.broadcastData).Split('_');
        matchName = splitted[0];
        string[] splittedPlayers = splitted[1].Split('|');
        for (int i = 0; i < splittedPlayers.Length; i++)
        {
            string colorHex = splittedPlayers[i].Substring(splittedPlayers[i].Length - 6, 6);
            string symbolName = splittedPlayers[i].Substring(0,  splittedPlayers[i].Length - 6);

            Color color;
            if (!ColorUtility.TryParseHtmlString("#" + colorHex, out color))
            {
                throw new System.Exception("broadcast color data error");
            }


            SymbolAndSprite symbolSprite = null;
            for (int j = 0; j < playerSymbols.possiblePlayerSprites.Count; j++)
            {
                if (playerSymbols.possiblePlayerSprites[j].playerSymbol.ToString() == symbolName)
                {

                    symbolSprite = playerSymbols.possiblePlayerSprites[j];
                    break;
                }
            }
            if (symbolSprite == null)
            {
                throw new System.Exception("broadcast sprite data error");
            }

            playersOnLobby.Add(new SpriteAndColor(symbolSprite, color));
        }

        serverAddress = broadcastResult.serverAddress;
    }

    public static string CreateMatchBroadcastData(string matchName, Player[] players)
    {

        string playersData = "";
        for (int i = 0; i < players.Length; i++)
        {
            if (i > 0)
                playersData += "|";

            playersData += players[i].playerSymbolAndSprite.playerSymbol + ColorUtility.ToHtmlStringRGB(players[i].color);
        }

        return matchName + "_" + playersData;

    }

    public static string CreateMatchBroadcastData(MatchData matchData)
    {

        string playersData = "";
        for (int i = 0; i < matchData.playersOnLobby.Count; i++)
        {
            if (i > 0)
                playersData += "|";

            playersData += matchData.playersOnLobby[i].symbolAndSprite.playerSymbol + ColorUtility.ToHtmlStringRGB(matchData.playersOnLobby[i].color);
        }

        return matchData.matchName + "_" + playersData;

    }

    public void AddPlayer(SpriteAndColor spriteAndColor)
    {
        playersOnLobby.Add(spriteAndColor);
    }


    public void RemovePlayer()
    {
        playersOnLobby.RemoveAt(playersOnLobby.Count);
    }
}