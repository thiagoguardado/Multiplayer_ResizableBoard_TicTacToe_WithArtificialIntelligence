using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAManager : MonoBehaviour {
    
    private Player m_IAPlayer = Player.Cross;
    public BoardManager boardManager;
    private int bestIndex;

    public void FindBestMovement() {

        List<int> empty = boardManager.Board.FindEmptyPositions();

        if (empty.Count > 0)
        {

            // clone board
            Board clonedBoard = new Board(boardManager.Board);

            bestIndex = -1;

            MiniMax(clonedBoard, m_IAPlayer);

            Debug.Log("Best IA Movement: " + bestIndex.ToString());

            if (bestIndex == -1)
            {
                boardManager.AddPlayerToBoard(empty[UnityEngine.Random.Range(0, empty.Count)]);
            }
            else
            {
                boardManager.AddPlayerToBoard(bestIndex);
            }

        }

    }

    private int MiniMax(Board board, Player currentPlayer)
    {

        GameResult gameResult = board.CheckEndGame();

        if (gameResult != GameResult.None)
        {
            return CalculateScore(gameResult);
        }
        else
        {
            
            int val = 0;
            Player nextplayer = currentPlayer == Player.Circle ? Player.Cross : Player.Circle;

            if (currentPlayer != m_IAPlayer)
            {
                val = 999999;
                foreach (int index in board.FindEmptyPositions())
                {
                    Board nextBoard = new Board(board);
                    nextBoard.AddPlayerToBoard(index,currentPlayer);

                    int indexval = MiniMax(nextBoard, nextplayer);
                    if (indexval < val || (indexval == val && (UnityEngine.Random.value > 0.5f)))
                    {
                        val = indexval;
                        bestIndex = index;
                    }
                    

                }
            }
            else
            {
                val = -999999;
                foreach (int index in board.FindEmptyPositions())
                {
                    Board nextBoard = new Board(board);
                    nextBoard.AddPlayerToBoard(index, currentPlayer);
                    int indexval = MiniMax(nextBoard, nextplayer);
                    if (indexval > val || (indexval == val && (UnityEngine.Random.value > 0.5f)))
                    {
                        val = indexval;
                        bestIndex = index;
                    }

                }

            }
            return val;
        }

    }

    private void LogMinimaxCalc(Board nextBoard)
    {
        string s = "";
        foreach (var item in nextBoard.FullBoard.ToArray())
        {
            s += item.ToString() + " ";
        }
        Debug.Log(s);
        Debug.Log("Best index in depth " + (nextBoard.FindEmptyPositions().Count + 1).ToString() + ": " + bestIndex.ToString());

    }

    private int CalculateScore(GameResult gameResult) {

        switch (gameResult)
        {
            case GameResult.CircleWin:
                if (m_IAPlayer == Player.Circle)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            case GameResult.CrossWin:
                if (m_IAPlayer == Player.Cross)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            case GameResult.TicTacToe:
                return 0;
            default:
                return 0;
        }
       
    }

}
