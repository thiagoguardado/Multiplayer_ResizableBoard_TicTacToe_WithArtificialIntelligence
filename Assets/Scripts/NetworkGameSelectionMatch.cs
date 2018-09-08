using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkGameSelectionMatch : MonoBehaviour {

    public MatchData thisMatchData;
    public Text matchName;
    public int maxCharsInName = 15;
    public List<Image> playerSymbols = new List<Image>();

    public void Setup(MatchData _matchData)
    {
        thisMatchData = _matchData;

        SetName(thisMatchData.matchName);
        SetPlayers(thisMatchData.playersOnLobby);

    }

    private void SetName(string newName)
    {
        matchName.text = newName.Substring(0, Mathf.Min(newName.Length,maxCharsInName));
    }

    private void SetPlayers(List<MatchData.SpriteAndColor> players)
    {
        foreach (Image image in playerSymbols)
        {
            image.gameObject.SetActive(false);
        }

        for (int i = 0; i < players.Count; i++)
        {
            playerSymbols[i].gameObject.SetActive(true);
            playerSymbols[i].sprite = players[i].symbolAndSprite.playerSprite;
            playerSymbols[i].color = players[i].color;


        }
    }

    public void OnClick()
    {
        GameObject.FindObjectOfType<MyNetworkManager>().ConnectToAMatch(thisMatchData);

    }

}
