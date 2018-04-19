using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Player
{
    None,
    Circle,
    Cross
}
public enum GameResult
{
    None,
    CircleWin,
    CrossWin,
    TicTacToe
}


public class BoardManager : MonoBehaviour {

    private Board m_board;
    public Board Board
    {
        get
        {
            return m_board;
        }
    }

    private Player m_controlledPlayer = Player.Circle;
    private Player m_currentPlayer;
    public Player CurrentPlayer
    {
        get
        {
            return m_currentPlayer;
        }
    }
    
    private BoardView m_boardView;
    public IAManager iaManager;

    private GameResult m_currentResult = GameResult.None;

    private void Awake() {

        m_board = new Board(3);
        m_currentPlayer = m_controlledPlayer;
        m_boardView = GetComponent<BoardView>();
    }

 
    private void CheckAndResolveEndGame() {

        EndGame(m_board.CheckEndGame());

    }

    public void AddPlayerToBoard(int position)
    {

        if (m_currentResult == GameResult.None)
        {
            if (m_board.AddPlayerToBoard(position, m_currentPlayer))
            {
                m_boardView.UpdateView();

                // change current player
                if (m_currentPlayer != Player.Circle)
                {
                    ChangeCurrentPlayer(Player.Circle);
                }
                else
                {
                    ChangeCurrentPlayer(Player.Cross);
                }

                // check end of game
                CheckAndResolveEndGame();

                if (m_currentResult == GameResult.None)
                {
                    if (m_currentPlayer != m_controlledPlayer)
                    {
                        GameManager.playerCanInteract = false;
                        iaManager.FindBestMovement();
                    }
                    else
                    {
                        GameManager.playerCanInteract = true;
                    }
                }

            }


        }


    }


    private void ChangeCurrentPlayer(Player newPlayer)
    {
        m_currentPlayer = newPlayer;
    }


 

    private void EndGame(GameResult result)
    {

        m_currentResult = result;

        switch (result)
        {
            case GameResult.None:
                break;
            case GameResult.CircleWin:
                Debug.Log("Circle wins");
                break;
            case GameResult.CrossWin:
                Debug.Log("Cross wins");
                break;
            case GameResult.TicTacToe:
                Debug.Log("Tie");
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

    private List<Player> m_fullBoard;
    public List<Player> FullBoard
    {
        get
        {
            return m_fullBoard;
        }
    }


    // constructor
    public Board(int size)
    {
        // create board
        m_size = size;
        m_fullBoard = new List<Player>();
        for (int i = 0; i < (m_size * m_size); i++)
        {
            m_fullBoard.Add(Player.None);
        }

    }

    public Board(Board referenceBoard)
    {
        m_size = referenceBoard.Size;
        m_fullBoard = new List<Player>();
        for (int i = 0; i < (referenceBoard.m_fullBoard.Count); i++)
        {
            m_fullBoard.Add(referenceBoard.m_fullBoard[i]);
        }
    }

    private Player PlayerAtBoardPosition(int i)
    {
        return m_fullBoard[i];
    }

    public List<int> FindEmptyPositions()
    {
        List<int> emptyPositions = new List<int>();
        for (int i = 0; i < m_fullBoard.Count; i++)
        {
            if (m_fullBoard[i] == Player.None)
                emptyPositions.Add(i);
        }
        return emptyPositions;
    }

    public bool AddPlayerToBoard(int position,Player player)
    {
        if (PlayerAtBoardPosition(position) == Player.None)
        {
            m_fullBoard[position] = player;
            return true;

        }

        return false;
    }

    public GameResult CheckEndGame()
    {

        Player playerWon = CheckWinning();

        // check tie
        if (playerWon == Player.None)
        {
            if (CheckTie())
            {
                return GameResult.TicTacToe;
            }
        }
        else
        {
            // Resolve playerwin
            switch (playerWon)
            {
                case Player.Circle:
                    return GameResult.CircleWin;
                case Player.Cross:
                    return GameResult.CrossWin;
                default:
                    break;
            }
        }
        return GameResult.None;
    }

    private Player CheckWinning()
    {

        // check lines
        Player playerWon = CheckLines();

        if (playerWon == Player.None)
        {

            // check columns
            playerWon = CheckColumns();

            if (playerWon == Player.None)
            {

                // check diagonal 1
                playerWon = CheckDiagonal1();

                if (playerWon == Player.None)
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

    private Player CheckLines()
    {
        Player p = Player.None;
        bool different;
        for (int i = 0; i < m_size; i++)
        {
            p = m_fullBoard[i * m_size];

            if (p != Player.None)
            {
                different = false;

                for (int j = 1; j < m_size; j++)
                {
                    if (m_fullBoard[i * m_size + j] != p)
                    {
                        different = true;
                        break;
                    }
                }

                if (!different)
                {
                    return p;
                }
            }

        }

        return Player.None;
    }

    private Player CheckColumns()
    {
        Player p = Player.None;
        bool different;
        for (int i = 0; i < m_size; i++)
        {
            p = m_fullBoard[i];
            if (p != Player.None)
            {
                different = false;

                for (int j = 1; j < m_size; j++)
                {
                    if (m_fullBoard[i + j * m_size] != p)
                    {
                        different = true;
                        break; ;
                    }
                }

                if (!different)
                {
                    return p;
                }
            }


        }

        return Player.None;
    }

    private Player CheckDiagonal1()
    {
        Player p = Player.None;

        p = m_fullBoard[0];
        if (p != Player.None)
        {
            for (int i = 1; i < m_size; i++)
            {
                if (m_fullBoard[i * (1 + m_size)] != p)
                {
                    return Player.None;
                }
            }
        }
        else
        {
            return Player.None;
        }

        return p;
    }

    private Player CheckDiagonal2()
    {
        Player p = Player.None;

        p = m_fullBoard[m_size - 1];
        if (p != Player.None)
        {
            for (int i = 1; i < m_size; i++)
            {
                if (m_fullBoard[(i + 1) * (m_size - 1)] != p)
                {
                    return Player.None;
                }
            }
        }
        else
        {
            return Player.None;
        }

        return p;
    }

}
