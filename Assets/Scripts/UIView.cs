using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays elements in game scene
/// </summary>
public class UIView : MonoBehaviour {

    private PlayerSprite currentPlayer;
    private string currentPlayerName;
    public Image nextPlayerImage;
    public Image playerWinImage;
    public Text nextPlayerName;
    public Text playerWinName;

    public GameObject nextPlayerPanel;
    public GameObject playerWinPanel;
    public GameObject tiePanel;

    private bool updatingNextPlayer;

    private Animator nextPlayerAnimator;

    public Animator userActionAnimator;

    void Awake()
    {
        updatingNextPlayer = true;
        nextPlayerPanel.SetActive(true);
        playerWinPanel.SetActive(false);
        tiePanel.SetActive(false);
        nextPlayerAnimator = nextPlayerPanel.GetComponent<Animator>();
    }

	void Update () {

        if (updatingNextPlayer)
        {
            ChangeNextPlayer();
        }

        UpdateActionPanel();

	}

    private void UpdateActionPanel()
    {
        if (GameManager.GameState == GameState.GameFinished)
        {
            userActionAnimator.SetBool("off", true);
            return;
        }
        else
        {
            userActionAnimator.SetBool("off", false);
        }

        if (BoardManager.Instance.Board.CurrentPlayer.playerType == PlayerType.Human && GameManager.playerCanInteract)
        {
            userActionAnimator.SetBool("waiting", false);
        }
        else
        {
            userActionAnimator.SetBool("waiting", true);
        }
    }

    private void ChangeNextPlayer()
    {
        if (BoardManager.Instance.Board.CurrentPlayer.playerSprite != currentPlayer)
        {
            currentPlayer = BoardManager.Instance.Board.CurrentPlayer.playerSprite;
            currentPlayerName = GameManager.GetPlayerNameBySymbol(BoardManager.Instance.Board.CurrentPlayer.playerSymbol);

            nextPlayerImage.sprite = currentPlayer.sprite;
            nextPlayerImage.color = currentPlayer.color;
            if (currentPlayerName != null) {
                nextPlayerName.gameObject.SetActive(true);
                nextPlayerName.text = currentPlayerName;
                nextPlayerName.color = currentPlayer.color;
            } else {
                nextPlayerName.gameObject.SetActive(false);
            }
            nextPlayerAnimator.SetTrigger("tilt");

        }

    }



    public void PlayerWin(GamePlayer winningPlayer) {

        nextPlayerPanel.SetActive(false);
        playerWinPanel.SetActive(true);
        tiePanel.SetActive(false);

        playerWinPanel.GetComponent<Animator>().SetBool("pulsing", true);

        playerWinImage.sprite = winningPlayer.playerSprite.sprite;
        playerWinImage.color = winningPlayer.playerSprite.color;
        string winnningPlayerName = GameManager.GetPlayerNameBySymbol(winningPlayer.playerSymbol);

        if (winnningPlayerName != null)
        {
            playerWinName.gameObject.SetActive(true);
            playerWinName.text = winnningPlayerName;
            playerWinName.color = winningPlayer.playerSprite.color;
        }
        else
        {
            playerWinName.gameObject.SetActive(false);
        }

        updatingNextPlayer = false;


    }

    public void Tie()
    {

        nextPlayerPanel.SetActive(false);
        playerWinPanel.SetActive(false);
        tiePanel.SetActive(true);

        tiePanel.GetComponent<Animator>().SetBool("pulsing", true);

        updatingNextPlayer = false;
    }

}
