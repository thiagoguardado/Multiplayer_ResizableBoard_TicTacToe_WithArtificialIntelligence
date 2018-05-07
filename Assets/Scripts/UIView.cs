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
    public Image nextPlayerImage;
    public Image playerWinImage;

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

        if (BoardManager.Instance.Board.CurrentPlayer.playerType == PlayerType.Human)
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
            nextPlayerImage.sprite = currentPlayer.sprite;
            nextPlayerImage.color = currentPlayer.color;
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
