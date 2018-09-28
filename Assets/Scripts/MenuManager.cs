using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;


/// <summary>
/// Controls game configuration menu
/// </summary>
public class MenuManager : MonoBehaviour {

    public static MenuManager Instance;

    public PlayerSymbols playerSymbols;
    public int[] PossibleNumberOfPlayers = { 2, 3, 4 };
    public int[] PossibleBoardSize = { 3, 4, 5, 6, 7, 8, 9 };

    public Animator numberOfPlayersAnimator;
    public Animator boardSizeAnimator;

    private MenuView menuView;


    private void Awake()
    {
        Instance = this;
        menuView = GetComponent<MenuView>();

        if (!NetworkServer.active)
        {
            GameManager.Instance.Initialize();
        } else {
            GameManager.Instance.NetworkInitialize();
        }

    }


    private void Start()
    {
        //menuView.UpdateView();
    }

    public void EffectivateBoardSizeChange()
    {
        boardSizeAnimator.SetTrigger("change");
        menuView.UpdateView();
        AudioManager.Instance.PlayOptionSelectSFX();
    }

    public void ChangeBoardSize(bool greater)
    {
        if (GameManager.ChangeBoardSize(PossibleBoardSize, greater))
        {
            // do changes
            EffectivateBoardSizeChange();

            // update networked itens
            NetworkGameLobby[] clientsScript = FindObjectsOfType<NetworkGameLobby>();
            foreach (var script in clientsScript)
            {
                if(script.isLocalPlayer) script.RpcChangeBoardSize(GameManager.boardSize);
                break;
            }
        }
    }

    public void ChangeNumberOfPlayers(bool greater)
    {
        if (GameManager.ChangeNumberOfPlayers(PossibleNumberOfPlayers, greater))
        {
            numberOfPlayersAnimator.SetTrigger("change");
            menuView.UpdateView();
            AudioManager.Instance.PlayOptionSelectSFX();

        }
    }



}

[System.Serializable]
public class Player
{
    public SymbolAndSprite playerSymbolAndSprite;
    public Color color;
    public PlayerType playerType;
    public string playerName;
    public int networkConnectionID;

    public Player(SymbolAndSprite _playerSymbolAndSprite, string playerName = "")
    {
        this.playerSymbolAndSprite = _playerSymbolAndSprite;
        this.color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
        this.playerType = PlayerType.Human;
        this.playerName = playerName;
    }

    public Player(SymbolAndSprite _playerSymbolAndSprite, Color color, string playerName = "", int connectionID = 0)
    {
        this.playerSymbolAndSprite = _playerSymbolAndSprite;
        this.color = color;
        this.playerName = playerName;
        this.networkConnectionID = connectionID;
        this.playerType = PlayerType.Human;
    }

}



