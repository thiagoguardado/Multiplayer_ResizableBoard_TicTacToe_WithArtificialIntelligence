using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIView : MonoBehaviour {

    private PlayerSprite currentPlayer;
    public Image nextPlayerImage;


	// Update is called once per frame
	void Update () {

        ChangeNextPlayer();

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
}
