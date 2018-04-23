using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerSymbol
{
    Circle,
    Cross,
    Triangle,
    Square
}
public enum GameResult
{
    None,
    PlayerWin,
    TicTacToe
}
public enum PlayerType
{
    Human,
    AI
}


public class BoardManager : MonoBehaviour {

    public int boardSize = 3;

    private Board m_board;
    public Board Board
    {
        get
        {
            return m_board;
        }
    }



    public List<GamePlayer> players = new List<GamePlayer>();

    public IAManager.MinimaxType minimaxType = IAManager.MinimaxType.Classic;
    public int maxMinimaxDepth = 8;

    private GamePlayer winningPlayer;

    private BoardView m_boardView;
    private IAManager m_iaManager;

    private GameResult m_currentResult = GameResult.None;
    public GameResult CurrentResult
    {
        get
        {
            return m_currentResult;
        }
    }


    private void Awake() {

        // create board
        m_board = new Board(boardSize, players);

        m_boardView = GetComponent<BoardView>();

        // initialize view
        m_boardView.Initialize(this, boardSize);

        // initialize IA
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].playerType == PlayerType.AI)
            {
                IAManager ia = gameObject.AddComponent<IAManager>();
                ia.Initialize(this, players[i], minimaxType,maxMinimaxDepth);
            }
        }
    }

    private void Update()
    {
        if (m_currentResult != GameResult.None) {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Reset();
            }

        }
    }

    private void Reset()
    {
        for (int i = 0; i < m_board.FullBoard.Count; i++)
        {
            m_board.RemovePlayerFromBoard(i);
        }

        m_boardView.UpdateView();

        IAManager[] ias = GetComponents<IAManager>();
        for (int i = 0; i < ias.Length; i++)
        {
            Destroy(ias[i]);
        }

        // initialize IA
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].playerType == PlayerType.AI)
            {
                IAManager ia = gameObject.AddComponent<IAManager>();
                ia.Initialize(this, players[i], minimaxType,maxMinimaxDepth);
            }
        }

        m_currentResult = GameResult.None;
        m_board.SetPlayer(players[0]);
    }

    private void CheckAndResolveEndGame() {

        EndGame(m_board.CheckEndGame(out winningPlayer));

    }

    public void AddPlayerToBoard(int position)
    {

        if (m_currentResult == GameResult.None)
        {
            if (m_board.AddPlayerToBoard(position, m_board.CurrentPlayer))
            {
                m_boardView.UpdateView();

                // check end of game
                CheckAndResolveEndGame();

            }


        }


    }


    private void EndGame(GameResult result)
    {

        m_currentResult = result;

        switch (result)
        {
            case GameResult.None:
                break;
            case GameResult.PlayerWin:
                Debug.Log(winningPlayer.playerName + " wins");
                break;
            case GameResult.TicTacToe:
                Debug.Log("Tie");
                break;
            default:
                break;
        }

    }





}

public struct Board
{
    private int m_size;
    public int Size
    {
        get
        {
            return m_size;
        }
    }

    private List<GamePlayer> m_fullBoard;
    public List<GamePlayer> FullBoard
    {
        get
        {
            return m_fullBoard;
        }
    }

    private List<GamePlayer> players;
    private GamePlayer currentPlayer;
    public GamePlayer CurrentPlayer {
        get
        {
            return currentPlayer;
        }
    }

    // constructor
    public Board(int size, List<GamePlayer> _players)
    {
        // create board
        m_size = size;
        m_fullBoard = new List<GamePlayer>();
        for (int i = 0; i < (m_size * m_size); i++)
        {
            m_fullBoard.Add(null);
        }

        players = _players;
        currentPlayer = _players[0];

    }

    public Board(Board referenceBoard)
    {
        m_size = referenceBoard.Size;
        m_fullBoard = new List<GamePlayer>();
        for (int i = 0; i < (referenceBoard.m_fullBoard.Count); i++)
        {
            m_fullBoard.Add(referenceBoard.m_fullBoard[i]);
        }
        players = referenceBoard.players;
        currentPlayer = referenceBoard.currentPlayer;
    }

    private GamePlayer PlayerAtBoardPosition(int i)
    {
        return m_fullBoard[i];
    }

    public List<int> FindEmptyPositions()
    {
        List<int> emptyPositions = new List<int>();
        for (int i = 0; i < m_fullBoard.Count; i++)
        {
            if (m_fullBoard[i] == null)
                emptyPositions.Add(i);
        }
        return emptyPositions;
    }

    public void SetPlayer(GamePlayer newPlayer)
    {

        currentPlayer = newPlayer;

    }

    public GamePlayer NextPlayer()
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i] == currentPlayer)
            {
                return players[(i + 1) % players.Count];

            }
        }

        Debug.Log("Could not find next player");
        return null;
    }


    public bool AddPlayerToBoard(int position,GamePlayer player)
    {
        GamePlayer gp = PlayerAtBoardPosition(position);
        if (gp == null)
        {
            m_fullBoard[position] = player;

            // change current player
            currentPlayer = NextPlayer();
            return true;

        }

        return false;
    }

    public void RemovePlayerFromBoard(int position)
    {
        m_fullBoard[position] = null;
    }

    public GameResult CheckEndGame(out GamePlayer winningPlayerSymbol)
    {
        winningPlayerSymbol = null;

        GamePlayer playerWon = CheckWinning();

        // check tie
        if (playerWon == null)
        {
            if (CheckTie())
            {
                return GameResult.TicTacToe;
            }
        }
        else
        {
            winningPlayerSymbol = playerWon;
            return GameResult.PlayerWin;
        }
        return GameResult.None;
    }

    private GamePlayer CheckWinning()
    {

        // check lines
        GamePlayer playerWon = CheckLines();

        if (playerWon == null)
        {

            // check columns
            playerWon = CheckColumns();

            if (playerWon == null)
            {

                // check diagonal 1
                playerWon = CheckDiagonal1();

                if (playerWon == null)
                {

                    // check diagonal 2
                    playerWon = CheckDiagonal2();

                }

            }


        }

        return playerWon;
    }

    private bool CheckTie()
    {

        List<int> empty = FindEmptyPositions();

        if (empty.Count == 0)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    private GamePlayer CheckLines()
    {
        GamePlayer p;
        bool different;
        for (int i = 0; i < m_size; i++)
        {
            p = m_fullBoard[i * m_size];

            if (p != null)
            {
                different = false;

                for (int j = 1; j < m_size; j++)
                {
                    if (m_fullBoard[(i * m_size) + j] == null)
                    {
                        different = true;
                        break;
                    }
                    else
                    {
                        if (m_fullBoard[(i * m_size) + j].playerSymbol != p.playerSymbol)
                        {
                            different = true;
                            break;
                        }
                    }
                }

                if (!different)
                {
                    return p;
                }
            }

        }

        return null;
    }

    private GamePlayer CheckColumns()
    {
        GamePlayer p;
        bool different;
        for (int i = 0; i < m_size; i++)
        {
            p = m_fullBoard[i];
            if (p != null)
            {
                different = false;

                for (int j = 1; j < m_size; j++)
                {
                    if (m_fullBoard[i + (j * m_size)] == null)
                    {
                        different = true;
                        break;
                    }
                    else
                    {
                        if (m_fullBoard[i + (j * m_size)].playerSymbol != p.playerSymbol)
                        {
                            different = true;
                            break;
                        }
                    }
                }

                if (!different)
                {
                    return p;
                }
            }


        }

        return null;
    }

    private GamePlayer CheckDiagonal1()
    {
        GamePlayer p;

        p = m_fullBoard[0];
        if (p != null)
        {
            for (int i = 1; i < m_size; i++)
            {
                if (m_fullBoard[i * (1 + m_size)] == null)
                {
                    return null;
                }
                else
                {
                    if (m_fullBoard[i * (1 + m_size)].playerSymbol != p.playerSymbol)
                    {
                        return null;
                    }
                }
            }
        }
        else
        {
            return null;
        }

        return p;
    }

    private GamePlayer CheckDiagonal2()
    {
        GamePlayer p;

        p = m_fullBoard[m_size - 1];
        if (p != null)
        {
            for (int i = 1; i < m_size; i++)
            {
                if (m_fullBoard[(i + 1) * (m_size - 1)] == null)
                {
                    return null;
                }
                else
                {
                    if (m_fullBoard[(i + 1) * (m_size - 1)].playerSymbol != p.playerSymbol)
                    {
                        return null;
                    }
                }
            }
        }
        else
        {
            return null;
        }

        return p;
    }



}

[System.Serializable]
public class GamePlayer
{
    public string playerName;
    public PlayerSymbol playerSymbol;
    public PlayerType playerType;
    public PlayerSprite playerSprite;

    public GamePlayer(GamePlayer reference)
    {
        if (reference != null)
        {
            playerName = reference.playerName;
            playerSymbol = reference.playerSymbol;
            playerType = reference.playerType;
            playerSprite = reference.playerSprite;
        }
    }
}