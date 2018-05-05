using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuView : MonoBehaviour {

    public MenuPlayerBox playerBoxPrefab;
    public Text numberOfPlayers;
    public Text boardSize;
    public Transform menuPlayerBoxesCenter;
    private List<MenuPlayerBox> menuPlayerBoxes = new List<MenuPlayerBox>();


    public void UpdateView()
    {
        UpdateBoardSize();
        UpdateNumberOfPlayers();
        UpdateMenuPlayerBoxes();

    }

    private void UpdateBoardSize()
    {
        boardSize.text = GameManager.boardSize.ToString();

    }

    private void UpdateNumberOfPlayers()
    {
        numberOfPlayers.text = GameManager.numberOfPlayers.ToString();
    }

    private void UpdateMenuPlayerBoxes()
    {
        // check if number of player boxes is equal to players
        if (menuPlayerBoxes.Count != GameManager.numberOfPlayers)
        {
            CreateNewPlayerBoxes();
        }

        for (int i = 0; i < GameManager.numberOfPlayers; i++)
        {
            Player p = GameManager.players[i];
            menuPlayerBoxes[i].Setup(p.playerSymbolAndSprite.playerSprite, p.color, p.playerType,i,this);

        }
    }

    private void CreateNewPlayerBoxes()
    {
        for (int i = 0; i < menuPlayerBoxes.Count; i++)
        {
            Destroy(menuPlayerBoxes[i].gameObject);
        }

        menuPlayerBoxes.Clear();

        for (int i = 0; i < GameManager.numberOfPlayers; i++)
        {
            MenuPlayerBox go = Instantiate(playerBoxPrefab, menuPlayerBoxesCenter.position,Quaternion.identity,menuPlayerBoxesCenter);
            go.transform.localPosition = new Vector3((i - ((float)GameManager.numberOfPlayers-1) / 2f) * 450f, 0, 0);
            menuPlayerBoxes.Add(go);
        }

    }
}
