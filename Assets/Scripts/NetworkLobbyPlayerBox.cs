using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections.Generic;
using System;

public class NetworkLobbyPlayerBox : MenuPlayerBox
{
    private int connectionID;

    new public void Setup(Sprite sprite, Color color, string playerName, int _connectionID, MenuView _menuView)
    {
        image.sprite = sprite;
        image.color = color;
        label.text = playerName;
        menuView = _menuView;
        connectionID = _connectionID;
    }


    new public void ChangeColor()
    {
        if (NetworkServer.active)
        {
            NetworkPlayer[] netPlayers = ((NetworkGameLobbyView)menuView).myNetworkManager.currentMatch.playersOnLobby;

            for (int i = 0; i < netPlayers.Length; i++)
            {
                if (netPlayers[i].connectionID == connectionID)
                {
                    ((NetworkGameLobbyView)menuView).myNetworkManager.currentMatch.playersOnLobby[i].color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
                    menuView.UpdateView();
                    imageAnimator.SetTrigger("change");
                    AudioManager.Instance.PlayOptionSelectSFX();
                    break;
                }
            }

        }

    }


    public void ChangeSymbol()
    {
        if (NetworkServer.active)
        {
            NetworkPlayer[] netPlayers = ((NetworkGameLobbyView)menuView).myNetworkManager.currentMatch.playersOnLobby;

            for (int i = 0; i < netPlayers.Length; i++)
            {
                if (netPlayers[i].connectionID == connectionID)
                {
                    PlayerSymbol currentSymbol = netPlayers[i].playerSymbol;
                    PlayerSymbol newSymbol = currentSymbol;
                    while(newSymbol == currentSymbol)
                        newSymbol = (PlayerSymbol)UnityEngine.Random.Range(0, Enum.GetValues(typeof(PlayerSymbol)).Length);

                    ((NetworkGameLobbyView)menuView).myNetworkManager.currentMatch.playersOnLobby[i].playerSymbol = newSymbol;
                    image.sprite = ((NetworkGameLobbyView)menuView).myNetworkManager.possibleSymbols.GetSprite(newSymbol);
                    menuView.UpdateView();
                    imageAnimator.SetTrigger("change");
                    AudioManager.Instance.PlayOptionSelectSFX();
                    break;
                }
            }

        }

    }
}
