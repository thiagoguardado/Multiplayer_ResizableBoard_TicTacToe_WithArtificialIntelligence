using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class IAManager: MonoBehaviour {

    public enum MinimaxType
    {
        Classic,
        AlphaBeta
    }

    private List<int> emptyPositions;
    private Board board;

    private GamePlayer m_gamePlayer;
    private MinimaxType m_minimaxType;
    private BoardManager m_boardManager;
    private int m_maxDepth;

    private float waitingTimeBeforePlay = 0.5f;
    private float waitingTime;
    private bool isWaiting;
    private bool finishedCalculation;
    private int moveIndex;
    GamePlayer m_minimaxWinningSymbol;
    Thread t;
    bool threadRunning;

    public void Initialize(BoardManager _boardManager, GamePlayer _player, MinimaxType minimaxType, int maxDepth)
    {
        m_boardManager = _boardManager;
        m_gamePlayer = _player;
        m_minimaxType = minimaxType;
        m_maxDepth = maxDepth;
    }

    private void OnDisable()
    {
        threadRunning = false;
        
    }

    private void Update()
    {

        if (isWaiting)
        {
            if (waitingTime >= waitingTimeBeforePlay)
            {
                if (finishedCalculation)
                {
                    m_boardManager.AddPlayerToBoard(moveIndex);
                    isWaiting = false;
                    return;
                }
            }
            else
            {
                waitingTime += Time.deltaTime;
                return;
            }

        }
        else
        {
            if (m_boardManager.Board.CurrentPlayer == m_gamePlayer && m_boardManager.CurrentResult == GameResult.None)
            {
                waitingTime = 0;
                isWaiting = true;
                finishedCalculation = false;
                emptyPositions = m_boardManager.Board.FindEmptyPositions();
                emptyPositions = ShuffleList<int>(emptyPositions);
                threadRunning = true;
                t = new Thread(this.FindBestMovement);
                t.Start();
            }
        }
    }

    public void FindBestMovement() {

        if (emptyPositions.Count > 0)
        {
           
            switch (m_minimaxType)
            {
                case MinimaxType.Classic:
                    moveIndex = 0;
                    board = new Board(m_boardManager.Board);
                    MinimaxClassic(true, out moveIndex, m_maxDepth);
                    break;
                case MinimaxType.AlphaBeta:
                    moveIndex = 0;
                    board = new Board(m_boardManager.Board);
                    MinimaxAlphaBeta(true, out moveIndex,-999999,999999, m_maxDepth);
                    break;
                default:
                    break;
            }
        }

        finishedCalculation = true;

    }

    private List<T> ShuffleList<T>(List<T> list)
    {
        //randomize list
        for (int i = 0; i < list.Count; i++)
        {
            int j = UnityEngine.Random.Range(0, list.Count);
            T temp = list[j];
            list[j] = list[i];
            list[i] = temp;
        }

        return list;
    }

    private int MinimaxClassic(bool isMax, out int positionIndex,int depth)
    {
        positionIndex = 0;

        if (!threadRunning)
        {
            return 0;
        }

        GameResult result = board.CheckEndGame(out m_minimaxWinningSymbol);
        if (result != GameResult.None || depth <= 0)
        {
            return CalculateScore(result, m_minimaxWinningSymbol);
        }

        GamePlayer minimaxCurrentPlayer = board.CurrentPlayer;
        int testIndex;
        int branchValue;
        int lastpositionIndex;
        int bestValue;

        if (isMax)
        {
            bestValue = -999999;

            for (int i = 0; i < emptyPositions.Count; i++)
            {
                lastpositionIndex = positionIndex;
                testIndex = emptyPositions[i];
                board.AddPlayerToBoard(testIndex, board.CurrentPlayer);
                emptyPositions.RemoveAt(i);
                branchValue = MinimaxClassic(false, out positionIndex,depth-1);
                emptyPositions.Insert(i, testIndex);
                board.RemovePlayerFromBoard(testIndex);
                board.SetPlayer(minimaxCurrentPlayer);
                positionIndex = lastpositionIndex;

                if (branchValue >= bestValue)
                {
                    positionIndex = emptyPositions[i];
                    bestValue = branchValue;
                }
            }
        }
        else
        {
            bestValue = 999999;

            for (int i = 0; i < emptyPositions.Count; i++)
            {
                lastpositionIndex = positionIndex;
                testIndex = emptyPositions[i];
                board.AddPlayerToBoard(testIndex, board.CurrentPlayer);
                emptyPositions.RemoveAt(i);
                branchValue = MinimaxClassic(true, out positionIndex,depth-1);
                emptyPositions.Insert(i, testIndex);
                board.RemovePlayerFromBoard(testIndex);
                board.SetPlayer(minimaxCurrentPlayer);
                positionIndex = lastpositionIndex;

                if (branchValue <= bestValue)
                {
                    positionIndex = testIndex;
                    bestValue = branchValue;
                }
            }

        }

        return bestValue;

    }

    private int MinimaxAlphaBeta(bool isMax, out int positionIndex, int alpha, int beta,int depth)
    {
        positionIndex = 0;

        if (!threadRunning)
        {
            return 0;
        }

        GameResult result = board.CheckEndGame(out m_minimaxWinningSymbol);
        if (result != GameResult.None || depth <=0)
        {
            return CalculateScore(result, m_minimaxWinningSymbol);
        }

        GamePlayer m_minimaxCurrentPlayer = board.CurrentPlayer;
        int testIndex;
        int branchValue;
        int lastpositionIndex;
        int bestValue;

        if (isMax)
        {

            bestValue = -999999;

            for (int i = 0; i < emptyPositions.Count; i++)
            {
                lastpositionIndex = positionIndex;
                testIndex = emptyPositions[i];
                board.AddPlayerToBoard(testIndex, board.CurrentPlayer);
                emptyPositions.RemoveAt(i);
                branchValue = MinimaxAlphaBeta(false, out positionIndex, alpha, beta,depth-1);
                emptyPositions.Insert(i, testIndex);
                board.RemovePlayerFromBoard(testIndex);
                board.SetPlayer(m_minimaxCurrentPlayer);
                positionIndex = lastpositionIndex;

                if (branchValue >= bestValue)
                {
                    positionIndex = testIndex;
                    bestValue = branchValue;


                    alpha = Mathf.Max(alpha, bestValue);
                    if (beta < alpha)
                    {
                        //positionIndex = testIndex;
                        break;
                    }

                }


            }
        }
        else
        {

            bestValue = 999999;

            for (int i = 0; i < emptyPositions.Count; i++)
            {
                lastpositionIndex = positionIndex;
                testIndex = emptyPositions[i];
                board.AddPlayerToBoard(testIndex, board.CurrentPlayer);
                emptyPositions.RemoveAt(i);
                branchValue = MinimaxAlphaBeta(true, out positionIndex, alpha, beta,depth-1);
                emptyPositions.Insert(i, testIndex);
                board.RemovePlayerFromBoard(testIndex);
                board.SetPlayer(m_minimaxCurrentPlayer);
                positionIndex = lastpositionIndex;

                if (branchValue <= bestValue)
                {
                    positionIndex = testIndex;
                    bestValue = branchValue;

                    beta = Mathf.Min(beta, bestValue);
                    if (beta < alpha)
                    {
                        //positionIndex = testIndex;
                        break;
                    }

                }



            }

        }

        return bestValue;

    }


    private int CalculateScore(GameResult gameResult, GamePlayer winningPlayer) {

        switch (gameResult)
        {
            case GameResult.PlayerWin:
                if (winningPlayer == m_gamePlayer)
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
