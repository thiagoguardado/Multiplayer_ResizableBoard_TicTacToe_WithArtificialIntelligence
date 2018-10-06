using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Menu,
    GameStarted,
    GameFinished
}

public enum NetworkType
{
    Local,
    LAN,
    Internet
}


public class GameManager : MonoBehaviour {

    public static GameManager Instance;                     //  singleton

    private static GameState gameState = GameState.Menu;    // stores current game state
    public static GameState GameState
    {
        get
        {
            return gameState;
        }
    }


    public static NetworkType networkType = NetworkType.Local;

    public static bool playerCanInteract = true;            // determine if player can interact with game
    public static int numberOfPlayers;                      // current number of players
    public static int boardSize;                            // current board size
    public static List<Player> players = new List<Player>();// current list of players

    public static bool initialized = false; // if game manager has been initialized

    public PlayerSymbols possiblePlayerSymbols;

    public delegate void GameAction();
    public static event GameAction GameStarted;
    public static event GameAction GameEnded;

    private void Awake()
    {
        // singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }

    }

    /// <summary>
    /// Starts game
    /// </summary>
    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
        AudioManager.Instance.ChangeToInGameMusic();
        gameState = GameState.GameStarted;
        if (GameStarted != null) GameStarted.Invoke();

    }

    /// <summary>
    /// finishes a game
    /// </summary>
    public void FinishGame()
    {
        gameState = GameState.GameFinished;
    }

    /// <summary>
    /// Return to menu after finished game
    /// </summary>
    public void ReturnToMenu()
    {
        SceneManager.LoadScene("LocalGameSetup");
        AudioManager.Instance.ChangeToMenuMusic();
        gameState = GameState.Menu;
    }


    /// <summary>
    /// Return to menu after finished game on lan
    /// </summary>
    public void ReturnToNetworkMenu()
    {
        MyNetworkManager.ClientDisconnectAll();
        SceneManager.LoadScene("NetworkGameSetup");
        AudioManager.Instance.ChangeToMenuMusic();
        gameState = GameState.Menu;
    }



    /// <summary>
    /// Returns to title screen
    /// </summary>
    public void ReturnToTitleScreen()
    {
        SceneManager.LoadScene("TitleScreen");
        gameState = GameState.Menu;
        GameManager.networkType = NetworkType.Local;
    }

    /// <summary>
    /// Initialize with default values
    /// </summary>
    public void Initialize()
    {

        if (!initialized)
        {

            // setup board
            boardSize = 3;

            // setup players
            numberOfPlayers = 2;
            players.Add(new Player(MenuManager.Instance.playerSymbols.possiblePlayerSprites[0]));
            players.Add(new Player(MenuManager.Instance.playerSymbols.possiblePlayerSprites[1]));

            initialized = true;

        }
    }


    public void NetworkInitialize()
    {
        if (!initialized)
        {
            // setup board
            boardSize = 3;
            numberOfPlayers = 0;

            initialized = true;
        }
    }

    public static string GetPlayerNameBySymbol(PlayerSymbol symbol)
    {
        foreach (var player in players)
        {
            if (player.playerSymbolAndSprite.playerSymbol == symbol) return player.playerName;
        }

        return null;
    }


    /// <summary>
    /// Change number of players
    /// </summary>
    /// <param name="possibleNumberOfPlayers"></param>
    /// <param name="greater"></param>
    /// <returns></returns>
    public static bool ChangeNumberOfPlayers(int[] possibleNumberOfPlayers, bool greater)
    {
        if(ChangeNumber(ref numberOfPlayers, possibleNumberOfPlayers, greater))
        {
            if (greater)
            {
                players.Add(new Player(MenuManager.Instance.playerSymbols.possiblePlayerSprites[players.Count]));
                return true;
            }
            else
            {
                players.RemoveAt(players.Count - 1);
                return true;
            }
            
        }
        return false;
    }

    /// <summary>
    /// Change board size
    /// </summary>
    /// <param name="possibleBoardSize"></param>
    /// <param name="greater"></param>
    /// <returns></returns>
    public static bool ChangeBoardSize(int[] possibleBoardSize, bool greater)
    {
        return ChangeNumber(ref boardSize, possibleBoardSize, greater);
    }

    private static bool ChangeNumber(ref int staticNumber, int[] possibleNumbers, bool greater)
    {
        if (greater)
        {
            if (staticNumber < possibleNumbers[possibleNumbers.Length - 1])
            {
                staticNumber += 1;
                return true;
            }

        }
        else
        {
            if (staticNumber > possibleNumbers[0])
            {
                staticNumber -= 1;
                return true;
            }
        }
        return false;
    }

    public void SetNetworkPlayers(NetworkPlayer[] netPlayers){

        players.Clear();
        for (int i = 0; i < netPlayers.Length; i++)
        {
            players.Add(new Player(new SymbolAndSprite(netPlayers[i].playerSymbol, possiblePlayerSymbols.GetSprite(netPlayers[i].playerSymbol)),
                                               netPlayers[i].color, netPlayers[i].playerName,netPlayers[i].playerID));
        }
        numberOfPlayers = netPlayers.Length;
    }


    public static PlayerSymbol FindDifferentPlayerSymbol(PlayerSymbol[] currentSymbols)
    {
        PlayerSymbol[] possibleSymbols = (PlayerSymbol[])Enum.GetValues(typeof(PlayerSymbol));
        PlayerSymbol[] eligibleSymbols = possibleSymbols.Except(currentSymbols).ToArray<PlayerSymbol>();

        return eligibleSymbols[UnityEngine.Random.Range(0, eligibleSymbols.Length)];

    }


    public void EndGameClickAction()
    {
        switch (networkType)
        {
            case NetworkType.Local:
                AudioManager.Instance.PlayReturnSFX();
                Instance.ReturnToMenu();
                break;
            case NetworkType.LAN:
            case NetworkType.Internet:
                Instance.ReturnToNetworkMenu();
                break;
            default:
                break;
        }
    }

    public void EscapeFromGameScene()
    {
        Instance.FinishGame();
        AudioManager.Instance.PlayReturnSFX();

        switch (networkType)
        {
            case NetworkType.Local:
                Instance.ReturnToMenu();
                break;
            case NetworkType.LAN:
            case NetworkType.Internet:
                Instance.ReturnToNetworkMenu();
                break;
            default:
                Instance.ReturnToNetworkMenu();
                break;
        }

    }
}