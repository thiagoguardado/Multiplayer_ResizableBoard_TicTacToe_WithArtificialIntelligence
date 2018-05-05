using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Menu,
    GameStarted,
    GameFinished
}

public class GameManager : MonoBehaviour {

    public static GameManager Instance;

    private static GameState gameState = GameState.Menu;
    public static GameState GameState
    {
        get
        {
            return gameState;
        }
    }

    public static bool playerCanInteract = true;

    public static int numberOfPlayers;
    public static int boardSize;

    public static List<Player> players = new List<Player>();


    
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

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        // populate players
        boardSize = 3;
        numberOfPlayers = 2;

        players.Add(new Player(MenuManager.Instance.possiblePlayerSprites[0]));
        players.Add(new Player(MenuManager.Instance.possiblePlayerSprites[1]));

    }

    public static bool ChangeNumberOfPlayers(int[] possibleNumberOfPlayers, bool greater)
    {
        if(ChangeNumber(ref numberOfPlayers, possibleNumberOfPlayers, greater))
        {
            if (greater)
            {
                players.Add(new Player(MenuManager.Instance.possiblePlayerSprites[players.Count]));
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


}
