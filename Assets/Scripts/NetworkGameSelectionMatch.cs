using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkGameSelectionMatch : MonoBehaviour {

    public Text matchName;
    public int maxCharsInName = 15;
    public List<Image> playerSymbols = new List<Image>();
    

    public void SetName(string newName)
    {
        matchName.text = newName.Substring(0, maxCharsInName);
    }

    public void SetPlayers(List<Player> players)
    {
        foreach (Image image in playerSymbols)
        {
            image.enabled = false;
        }

        for (int i = 0; i < players.Count; i++)
        {
            playerSymbols[i].enabled = false;
            playerSymbols[i].sprite = players[i].playerSymbolAndSprite.playerSprite;
            playerSymbols[i].color = players[i].color;


        }
    }


}
