using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

/// <summary>
/// Artificial Intelligence to play on tictactoe game
/// </summary>
public class IAManager: MonoBehaviour {

    public enum MinimaxType
    {
        Classic,
        AlphaBeta
    }

    public enum Difficulty
    {
        None,
        Easy,
        Medium,
        Hard
    }

    private Board board;                    // reference to board the AI is playing
    private List<int> emptyPositions;       // empty positions on board
    private GamePlayer m_gamePlayer;        // reference to board game player representing this AI
    private BoardManager m_boardManager;    // reference to board manager controlling game
    private MinimaxType m_minimaxType;      // type of minimax
    private int m_maxDepth;                 // max depth in minimax the AI can look
    private Difficulty m_difficulty;        // AI difficulty

    private float waitingTimeBeforePlay = 1.25f; // minimum time waiting before making move


    // Minimax variables
    GamePlayer m_minimaxWinningSymbol;
    Thread t;
    bool threadRunning;
    private float waitingTime;
    private bool isWaiting;
    private bool finishedCalculation;
    private int moveIndex;

    /// <summary>
    /// Initialize tha AI
    /// </summary>
    /// <param name="_boardManager"></param>
    /// <param name="_player"></param>
    /// <param name="minimaxType"></param>
    /// <param name="maxDepth"></param>
    public void Initialize(BoardManager _boardManager, GamePlayer _player, MinimaxType minimaxType, int maxDepth)
    {
        m_boardManager = _boardManager;
        m_gamePlayer = _player;
        m_minimaxType = minimaxType;
        m_maxDepth = maxDepth;
        switch (_player.playerType)
        {
            case PlayerType.AI_Easy:
                m_difficulty = Difficulty.Easy;
                break;
            case PlayerType.AI_Medium:
                m_difficulty = Difficulty.Medium;
                break;
            case PlayerType.AI_Hard:
                m_difficulty = Difficulty.Hard;
                break;
            default:
                m_difficulty = Difficulty.None;
                break;
        }
        
    }

    /// <summary>
    /// sets thread runiing bool to false so thread stops after the game is closed
    /// </summary>
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

                    // if finished calculation and finished waiting, apply movement to board
                    moveIndex = AdjustMovementByDifficulty(moveIndex);
                    m_boardManager.AddPlayerToBoard(moveIndex);
                    isWaiting = false;
                    return;
                }
            }
            else
            {
                // if not finished calculation, increase timer
                waitingTime += Time.deltaTime;
                return;
            }

        }
        else
        {
            // if AI turn and game not finished, start minimax calculation
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

    /// <summary>
    /// Depending on AI difficulty, consider minimax movement or random movement before applying to board
    /// </summary>
    /// <param name="moveIndex"></param>
    /// <returns></returns>
    private int AdjustMovementByDifficulty(int moveIndex)
    {
        switch (m_difficulty)
        {
            case Difficulty.Easy:

                if (UnityEngine.Random.value > 0.33f)
                {
                    return emptyPositions[UnityEngine.Random.Range(0, emptyPositions.Count)];
                }
                break;
            case Difficulty.Medium:
                if (UnityEngine.Random.value > 0.66f)
                {
                    return emptyPositions[UnityEngine.Random.Range(0, emptyPositions.Count)];
                }
                break;
            case Difficulty.Hard:
            default:
                break;
        }

        return moveIndex;

    }


    /// <summary>
    /// Finds best movement to make, using manimax funtion
    /// </summary>
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


    /// <summary>
    /// Shuffles a list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
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


    /// <summary>
    /// Classic minimax
    /// </summary>
    /// <param name="isMax"></param>
    /// <param name="positionIndex"></param>
    /// <param name="depth"></param>
    /// <returns></returns>
    private int MinimaxClassic(bool isMax, out int positionIndex,int depth)
    {
        positionIndex = 0;

        // stops if thread aborted
        if (!threadRunning)
        {
            return 0;
        }

        // check if game has reached end
        GameResult result = board.CheckEndGame(out m_minimaxWinningSymbol);
        if (result != GameResult.None || depth <= 0)
        {
            return CalculateScore(result, m_minimaxWinningSymbol);
        }


        // recursion
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


    /// <summary>
    /// Minimax funtion with alpha-beta prunning
    /// </summary>
    /// <param name="isMax"></param>
    /// <param name="positionIndex"></param>
    /// <param name="alpha"></param>
    /// <param name="beta"></param>
    /// <param name="depth"></param>
    /// <returns></returns>
    private int MinimaxAlphaBeta(bool isMax, out int positionIndex, int alpha, int beta,int depth)
    {
        positionIndex = 0;

        // finished if thread aborted
        if (!threadRunning)
        {
            return 0;
        }

        // check if game has reached end
        GameResult result = board.CheckEndGame(out m_minimaxWinningSymbol);
        if (result != GameResult.None || depth <=0)
        {
            return CalculateScore(result, m_minimaxWinningSymbol);
        }

        // recursion
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

    /// <summary>
    /// Heuristic funtion to calculate score depending on board 
    /// </summary>
    /// <param name="gameResult"></param>
    /// <param name="winningPlayer"></param>
    /// <returns></returns>
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
