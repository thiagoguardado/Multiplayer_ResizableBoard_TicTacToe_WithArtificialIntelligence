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

    private List<Player> m_board = new List<Player>();
    public List<Player> board
    {
        get
        {
            return m_board;
        }
    }
    private int m_size = 3;
    public int size
    {
        get
        {
            return m_size;
        }
    }
    private Player m_currentPlayer = Player.Circle;
    private GameResult m_currentResult = GameResult.None;
    private BoardView m_boardView;

    private void Awake() {
        InitializeBoard();
        m_boardView = GetComponent<BoardView>();
    }

    private Player PlayerAtBoardPosition(int i)
    {
        return m_board[i];
    }

    private void InitializeBoard()
    {

        // create board
        m_board.Clear();
        for (int i = 0; i < (m_size*m_size); i++)
        {
            m_board.Add(Player.None);
        }

        m_currentResult = GameResult.None;

    }

    private GameResult CheckWinningCondition()
    {
        // check lines
        Player playerWon = CheckLines();

        if (playerWon == Player.None) {

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

                    if (playerWon == Player.None)
                    {

                        // check tie
                        for (int i = 0; i < m_board.Count; i++)
                        {
                            if (m_board[i] == Player.None)
                            {
                                return GameResult.None;
                            }
                        }

                        return GameResult.TicTacToe;

                    }
                }

            }


        }

        // Resolve playerwin
        switch (playerWon)
        {
            case Player.Circle:
                return GameResult.CircleWin;
            case Player.Cross:
                return GameResult.CrossWin;
            default:
                Debug.Log("Did not find winner");
                return GameResult.None;
        }


    }

    public void AddPlayerToBoard(int position)
    {

        if (m_currentResult == GameResult.None)
        {

            if (PlayerAtBoardPosition(position) == Player.None && m_currentPlayer != Player.None)
            {
                m_board[position] = m_currentPlayer;

                m_boardView.UpdateView();

                // change current player
                if (m_currentPlayer != Player.Circle)
                {
                    m_currentPlayer = Player.Circle;
                }
                else
                {
                    m_currentPlayer = Player.Cross;
                }
            }

        }

        m_currentResult = CheckWinningCondition();


    }

    private Player CheckLines()
    {
        Player p = Player.None;
        bool different = false;
        for (int i = 0; i < m_size; i++)
        {
            p = m_board[i];
            if (p != Player.None)
            {
                for (int j = 1; j < m_size; j++)
                {
                    if (m_board[i + j] != p)
                    {
                        different = true;
                        break;
                    }
                }

                if(!different)
                    return p;
            }
            else {
                continue;
            }

        }

        return Player.None;
    }

    private Player CheckColumns()
    {
        Player p = Player.None;
        bool different = false;
        for (int i = 0; i < m_size; i++)
        {
            p = m_board[i];
            if (p != Player.None)
            {
                for (int j = 1; j < m_size; j++)
                {
                    if (m_board[i + j*m_size] != p)
                    {
                        different = true;
                        break; ;
                    }
                }

                if (!different)
                    return p;
            }
            else
            {
                continue;
            }

        }

        return Player.None;
    }

    private Player CheckDiagonal1()
    {
        Player p = Player.None;

        p = m_board[0];
        if (p != Player.None)
        {
            for (int i = 1; i < m_size; i++)
            {
                if (m_board[i * (1 + m_size)] != p)
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

        p = m_board[0];
        if (p != Player.None)
        {
            for (int i = 1; i < m_size; i++)
            {
                if (m_board[(i+1) * (m_size-1)] != p)
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
