using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIView : MonoBehaviour {

    private PlayerSprite currentPlayer;
    public Image nextPlayerImage;
    public Image playerWinImage;

    public GameObject nextPlayerPanel;
    public GameObject playerWinPanel;
    public GameObject tiePanel;

    private bool updatingNextPlayer;


    void Awake()
    {
        updatingNextPlayer = true;
        nextPlayerPanel.SetActive(true);
        playerWinPanel.SetActive(false);
        tiePanel.SetActive(false);
    }

	void Update () {

        if (updatingNextPlayer)
        {
            ChangeNextPlayer();
        }

	}

    private void ChangeNextPlayer()
    {
        if (BoardManager.Instance.Board.CurrentPlayer.playerSprite != currentPlayer)
        {
            currentPlayer = BoardManager.Instance.Board.CurrentPlayer.playerSprite;
            nextPlayerImage.sprite = currentPlayer.sprite;
            nextPlayerImage.color = currentPlayer.color;

        }

    }



    public void PlayerWin(GamePlayer winningPlayer) {

        nextPlayerPanel.SetActive(false);
        playerWinPanel.SetActive(true);
        tiePanel.SetActive(false);

        playerWinImage.sprite = winningPlayer.playerSprite.sprite;
        playerWinImage.color = winningPlayer.playerSprite.color;

    }

    public void Tie()
    {

        nextPlayerPanel.SetActive(false);
        playerWinPanel.SetActive(false);
        tiePanel.SetActive(true);
    }

}
