using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary>
/// Controls game configuration menu
/// </summary>
public class MenuManager : MonoBehaviour {

    public static MenuManager Instance;

    public List<SymbolAndSprite> possiblePlayerSprites;

    public int[] PossibleNumberOfPlayers = { 2, 3, 4 };
    public int[] PossibleBoardSize = { 3, 4, 5, 6, 7, 8, 9 };

    public Animator numberOfPlayersAnimator;
    public Animator boardSizeAnimator;

    private MenuView menuView;


    private void Awake()
    {
        Instance = this;
        menuView = GetComponent<MenuView>();

        GameManager.Instance.Initialize();

    }


    private void Start()
    {
        menuView.UpdateView();
    }


    public void ChangeBoardSize(bool greater)
    {
        if (GameManager.ChangeBoardSize(PossibleBoardSize, greater))
        {
            boardSizeAnimator.SetTrigger("change");
            menuView.UpdateView();
            AudioManager.Instance.PlayOptionSelectSFX();

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

    public Player(SymbolAndSprite _playerSymbolAndSprite)
    {
        playerSymbolAndSprite = _playerSymbolAndSprite;
        color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
        playerType = PlayerType.Human;
    }
}

[System.Serializable]
public class SymbolAndSprite
{
    public PlayerSymbol playerSymbol;
    public Sprite playerSprite;
}

