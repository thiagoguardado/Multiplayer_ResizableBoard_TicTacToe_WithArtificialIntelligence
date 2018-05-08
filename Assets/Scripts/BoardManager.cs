using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    AI_Easy,
    AI_Medium,
    AI_Hard
}

public enum WinningCheck
{
    Full,
    MaxWidth
}

public class BoardManager : MonoBehaviour {

    public static BoardManager Instance;    // singleton

    [Header("Settings")]
    public WinningCheck winningCheck;   // type of winning condition check (look full board or lined up symbols)
    public int maxLineWidth;            // number of lined up symbols to determine win condition if winning check set to MaxWidth
    public AIManager.MinimaxType minimaxType = AIManager.MinimaxType.Classic; // minimax type
    public int maxMinimaxDepth = 8; // max depth minimax can calculate

    [Header("Board and Players")]
    public int boardSize = 3;   // current game boad size
    public List<GamePlayer> players = new List<GamePlayer>(); // list of current players
    private Board m_board; // current game board object
    public Board Board
    {
        get
        {
            return m_board;
        }
    }
    
    private GamePlayer winningPlayer; // player that won game
    private BoardView m_boardView; // reference to view
    private AIManager m_iaManager; // reference to AI manager

    private GameResult m_currentResult = GameResult.None; // current game result
    public GameResult CurrentResult
    {
        get
        {
            return m_currentResult;
        }
    }


    private void Awake() {

        //Singleton
        Instance = this;

        // load setup
        LoadSetup();

        // create board
        m_board = new Board(boardSize, players, winningCheck, maxLineWidth);

        m_boardView = GetComponent<BoardView>();

        // initialize view
        m_boardView.Initialize(this, boardSize);


        // initialize IA
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].playerType == PlayerType.AI_Easy ||
                players[i].playerType == PlayerType.AI_Medium ||
                players[i].playerType == PlayerType.AI_Hard)
            {
                AIManager ia = gameObject.AddComponent<AIManager>();
                ia.Initialize(this, players[i], minimaxType,maxMinimaxDepth);
            }
        }

    }

    /// <summary>
    /// Loads setup from setting menu
    /// </summary>
    private void LoadSetup()
    {

        if (GameManager.Instance != null) //check if gamemanager with setting exists
        {
            boardSize = GameManager.boardSize;

            players.Clear();

            for (int i = 0; i < GameManager.players.Count; i++)
            {
                Player p = GameManager.players[i];
                players.Add(new GamePlayer(p.playerSymbolAndSprite.playerSymbol, p.playerType, p.playerSymbolAndSprite.playerSprite, p.color));

            }

            maxMinimaxDepth = MaxAcceptableDepth(boardSize);

        }
    }

    /// <summary>
    /// Resets Board
    /// </summary>
    private void Reset()
    {

        // removes all players from board
        for (int i = 0; i < m_board.FullBoard.Count; i++)
        {
            m_board.RemovePlayerFromBoard(i);
        }
        m_boardView.UpdateView();

        // recreate AI
        AIManager[] ias = GetComponents<AIManager>();
        for (int i = 0; i < ias.Length; i++)
        {
            Destroy(ias[i]);
        }
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].playerType == PlayerType.AI_Easy ||
                players[i].playerType == PlayerType.AI_Medium ||
                players[i].playerType == PlayerType.AI_Hard)
            {
                AIManager ia = gameObject.AddComponent<AIManager>();
                ia.Initialize(this, players[i], minimaxType,maxMinimaxDepth);
            }
        }

        // change game result and set player one
        m_currentResult = GameResult.None;
        m_board.SetPlayer(players[0]);
    }

    /// <summary>
    /// Checks if game is concluded
    /// </summary>
    private void CheckAndResolveEndGame() {

        EndGame(m_board.CheckEndGame(out winningPlayer));

    }


    /// <summary>
    /// Add a player to board at position
    /// </summary>
    /// <param name="position"></param>
    public void AddPlayerToBoard(int position)
    {

        if (m_currentResult == GameResult.None)
        {
            if (m_board.AddPlayerToBoard(position, m_board.CurrentPlayer))
            {
                int playerIndex = 0;
                for (int i = 0; i < players.Count; i++)
                {
                    if (players[i] == m_board.CurrentPlayer)
                    {
                        playerIndex = i;
                    }
                }

                m_boardView.AddedPlayerToBoard(playerIndex);
                m_boardView.UpdateView();

                // check end of game
                CheckAndResolveEndGame();

            }


        }


    }

    /// <summary>
    /// Resolves end game according to game result
    /// </summary>
    /// <param name="result"></param>
    private void EndGame(GameResult result)
    {

        m_currentResult = result;

        switch (result)
        {
            case GameResult.None:
                break;
            case GameResult.PlayerWin:
                PlayerWin();
                break;
            case GameResult.TicTacToe:
                Tie();
                break;
            default:
                break;
        }

    }


    /// <summary>
    /// Execute actions if a player has won the game
    /// </summary>
    private void PlayerWin()
    {
        Debug.Log(winningPlayer.playerSymbol.ToString() + " wins");
        m_boardView.PlayerWin(winningPlayer);
        GameManager.Instance.FinishGame();
    }

    /// <summary>
    /// Execute actions if the game finished in a tie
    /// </summary>
    private void Tie()
    {
        m_boardView.Tie();
        Debug.Log("Tie!");
        GameManager.Instance.FinishGame();
    }

    /// <summary>
    /// Select maximum depth looked by the AI minimax, according to board size
    /// </summary>
    /// <param name="boardSize"></param>
    /// <returns></returns>
    private int MaxAcceptableDepth(int boardSize)
    {
        switch (boardSize)
        {
            case 3:
                return 8;
            case 4:
                return 6;
            case 5:
                return 5;
            case 6:
                return 3;
            case 7:
            case 8:
            case 9:
                return 2;
            default:
                return 1;
        }
    }


}

/// <summary>
/// Board object, containing positions data
/// </summary>
public struct Board
{

    private int m_size;         // board size
    public int Size
    {
        get
        {
            return m_size;
        }
    }

    private List<GamePlayer> m_fullBoard;   // all board positions
    public List<GamePlayer> FullBoard
    {
        get
        {
            return m_fullBoard;
        }
    }

    private List<GamePlayer> players;   // players playing on board
    private GamePlayer currentPlayer;   // current player playing
    public GamePlayer CurrentPlayer {
        get
        {
            return currentPlayer;
        }
    }

    private WinningCheck m_winningCheck;    // type of winning condition check
    private int m_maxWidth;                 // max lined up symbols to win if winning condition check set to MaxWidth


    /// <summary>
    /// Constructs a board object
    /// </summary>
    /// <param name="size"></param>
    /// <param name="_players"></param>
    /// <param name="_winningCheck"></param>
    /// <param name="_maxWidth"></param>
    public Board(int size, List<GamePlayer> _players, WinningCheck _winningCheck, int _maxWidth)
    {
        // create board
        m_size = size;
        m_fullBoard = new List<GamePlayer>();
        for (int i = 0; i < (m_size * m_size); i++)
        {
            m_fullBoard.Add(null);
        }

        // add players
        players = _players;
        currentPlayer = _players[0];

        // set up winning check
        m_maxWidth = _maxWidth;
        m_winningCheck = _winningCheck;
        

    }

    /// <summary>
    /// Constructs a board using another as reference
    /// </summary>
    /// <param name="referenceBoard"></param>
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
        m_maxWidth = referenceBoard.m_maxWidth;
        m_winningCheck = referenceBoard.m_winningCheck;

    }

    /// <summary>
    /// Returns a position status
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    private GamePlayer PlayerAtBoardPosition(int i)
    {
        return m_fullBoard[i];
    }

    /// <summary>
    /// Finds all empty positions remaining on board
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Checks if board is full
    /// </summary>
    /// <returns></returns>
    public bool IsFullyFilled()
    {
        for (int i = 0; i < m_fullBoard.Count; i++)
        {
            if (m_fullBoard[i] == null)
                return false;
        }
        return true;
    }

    /// <summary>
    /// Set current player
    /// </summary>
    /// <param name="newPlayer"></param>
    public void SetPlayer(GamePlayer newPlayer)
    {

        currentPlayer = newPlayer;

    }

    /// <summary>
    /// Gets next player 
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Adds a player to position on board
    /// </summary>
    /// <param name="position"></param>
    /// <param name="player"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Removes any player from position
    /// </summary>
    /// <param name="position"></param>
    public void RemovePlayerFromBoard(int position)
    {
        m_fullBoard[position] = null;
    }

    /// <summary>
    /// Check if any player has won or if a tie has reached
    /// </summary>
    /// <param name="winningPlayerSymbol"></param>
    /// <returns></returns>
    public GameResult CheckEndGame(out GamePlayer winningPlayerSymbol)
    {

        // first look for a winner
        winningPlayerSymbol = null;
        GamePlayer playerWon;

        switch (m_winningCheck)
        {
            case WinningCheck.Full:
                playerWon = CheckWinning();
                break;
            case WinningCheck.MaxWidth:
                playerWon = CheckWinningMaxWidth();
                break;
            default:
                playerWon = CheckWinning();
                break;
        }


        // check tie if no winner
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

        // if no winner and no tie, return null
        return GameResult.None;
    }

    /// <summary>
    /// Check winning condition based on full board
    /// </summary>
    /// <returns></returns>
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


    /// <summary>
    /// Check winning condition based on max streak of lined up symbols
    /// </summary>
    /// <returns></returns>
    private GamePlayer CheckWinningMaxWidth()
    {

        // check lines
        GamePlayer playerWon = CheckLinesMaxWidth(m_maxWidth);

        if (playerWon == null)
        {

            // check columns
            playerWon = CheckColumnsMaxWidth(m_maxWidth);

            if (playerWon == null)
            {

                // check diagonal 1
                playerWon = CheckDiagonal1MaxWidth(m_maxWidth);

                if (playerWon == null)
                {

                    // check diagonal 2
                    playerWon = CheckDiagonal2MaxWidth(m_maxWidth);

                }

            }


        }

        return playerWon;
    }

    /// <summary>
    /// Check if a tie has reached
    /// </summary>
    /// <returns></returns>
    private bool CheckTie()
    {
        return IsFullyFilled();
    }


    /// <summary>
    /// Check if any player has won in lines
    /// </summary>
    /// <returns></returns>
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


    /// <summary>
    /// Check if any player has won in lines using max streak of lined up symbols
    /// </summary>
    /// <returns></returns>
    private GamePlayer CheckLinesMaxWidth(int maxWidth)
    {
        GamePlayer p;
        bool different = false;

        for (int i = 0; i < m_size; i++)
        {
            for (int j = 0; j < (m_size - maxWidth + 1); j++)
            {

                different = false;

                p = m_fullBoard[(i * m_size) + j];

                if (p != null)
                {

                    for (int k = 1; k < maxWidth; k++)
                    {
                        if (m_fullBoard[(i * m_size) + j + k] == null)
                        {
                            different = true;
                            break;
                        }
                        else
                        {
                            if (m_fullBoard[(i * m_size) + j + k].playerSymbol != p.playerSymbol)
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

        }

        return null;
    }


    /// <summary>
    /// Check if any player has won in columns
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Check if any player has won in columns using max streak of lined up symbols
    /// </summary>
    /// <returns></returns>
    private GamePlayer CheckColumnsMaxWidth(int maxWidth)
    {
        GamePlayer p;
        bool different = false;

        for (int i = 0; i < m_size; i++)
        {
            for (int j = 0; j < (m_size - maxWidth + 1); j++)
            {

                different = false;

                p = m_fullBoard[i + (j * m_size)];

                if (p != null)
                {

                    for (int k = 1; k < maxWidth; k++)
                    {
                        if (m_fullBoard[i + ((j + k) * m_size)] == null)
                        {
                            different = true;
                            break;
                        }
                        else
                        {
                            if (m_fullBoard[i + ((j + k) * m_size)].playerSymbol != p.playerSymbol)
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

        }

        return null;
    }

    /// <summary>
    /// Check if any player has won in northwest-southeast diagonal
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Check if any player has won in northwest-southeast diagonals using max streak of lined up symbols
    /// </summary>
    /// <returns></returns>
    private GamePlayer CheckDiagonal1MaxWidth(int maxWidth)
    {

        GamePlayer p;
        bool diferent = false;

        for (int i = 0; i < (m_size - maxWidth + 1); i++)
        {

            for (int l = 0; l < (m_size - maxWidth + 1); l++)
            {

                p = m_fullBoard[(i * m_size) + l];

                if (p == null)
                {
                    continue;
                }
                else
                {
                    diferent = false;
                    for (int k = 1; k < maxWidth; k++)
                    {
                        GamePlayer gp = m_fullBoard[((i+k) * m_size) + (l + k)];
                        if (gp == null)
                        {
                            diferent = true;
                            break;
                        }
                        else
                        {
                            if (gp.playerSprite != p.playerSprite)
                            {
                                diferent = true;
                                break;
                            }
                        }
                    }

                    if (!diferent)
                    {
                        return p;
                    }
                    
                }

            }

        }

        return null;
    }


    /// <summary>
    /// Check if any player has won in northeast-southwest diagonal
    /// </summary>
    /// <returns></returns>
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


    /// <summary>
    /// Check if any player has won in northeast-southwest diagonals using max streak of lined up symbols
    /// </summary>
    /// <returns></returns>
    private GamePlayer CheckDiagonal2MaxWidth(int maxWidth)
    {

        GamePlayer p;
        bool diferent = false;

        for (int i = 0; i < (m_size - maxWidth + 1); i++)
        {

            for (int l = (m_size - 1); l > (maxWidth - 2); l--)
            {

                p = m_fullBoard[(i * m_size) + l];

                if (p == null)
                {
                    continue;
                }
                else
                {

                    diferent = false;
                    for (int k = 1; k < maxWidth; k++)
                    {
                        GamePlayer gp = m_fullBoard[((i + k) * m_size) + (l - k)]; ;
                        if (gp == null)
                        {
                            diferent = true;
                            break;
                        }
                        else
                        {
                            if (gp.playerSprite != p.playerSprite)
                            {
                                diferent = true;
                                break;
                            }
                        }
                    }

                    if (!diferent)
                    {
                        return p;
                    }
                }

            }

        }

        return null;
    }

}

/// <summary>
/// A player, containing symbol, type, sprite and color
/// </summary>
[System.Serializable]
public class GamePlayer
{
    public PlayerSymbol playerSymbol;
    public PlayerType playerType;
    public PlayerSprite playerSprite;

    // constructor using reference
    public GamePlayer(GamePlayer reference)
    {
        if (reference != null)
        {
            playerSymbol = reference.playerSymbol;
            playerType = reference.playerType;
            playerSprite = reference.playerSprite;
        }
    }

    // constructor
    public GamePlayer(PlayerSymbol playerSymbol, PlayerType playerType, Sprite sprite, Color color)
    {
        this.playerSymbol = playerSymbol;
        this.playerType = playerType;
        this.playerSprite = new PlayerSprite(sprite, color);
    }


}