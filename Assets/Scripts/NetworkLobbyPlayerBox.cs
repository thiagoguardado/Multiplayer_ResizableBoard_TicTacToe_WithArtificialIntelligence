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

    public void SetupChange(Sprite sprite, Color color, string playerName, int _connectionID, MenuView _menuView)
    {
        bool changed = false;
        changed = changed || (image.sprite != sprite);
        changed = changed || (image.color != color);
        changed = changed || (label.text != playerName);

        image.sprite = sprite;
        image.color = color;
        label.text = playerName;
        menuView = _menuView;
        connectionID = _connectionID;

        if (changed) EffectivateChange();
    }

    public void ChangeColorAndSymbol()
    {
        ChangeColor(false);
        ChangeSymbol(true);
    }

    new public void ChangeColor(bool animateChange)
    {
        if (NetworkServer.active)
        {
            NetworkPlayer[] netPlayers = ((NetworkGameLobbyView)menuView).myNetworkManager.currentMatch.playersOnLobby;

            for (int i = 0; i < netPlayers.Length; i++)
            {
                if (netPlayers[i].connectionID == connectionID)
                {
                    ((NetworkGameLobbyView)menuView).myNetworkManager.currentMatch.playersOnLobby[i].color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
                    if(animateChange) EffectivateChange();
                    break;
                }
            }

        }

    }


    public void ChangeSymbol(bool animateChange)
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
                    while (newSymbol == currentSymbol)
                        newSymbol = (PlayerSymbol)UnityEngine.Random.Range(0, Enum.GetValues(typeof(PlayerSymbol)).Length);

                    ((NetworkGameLobbyView)menuView).myNetworkManager.currentMatch.playersOnLobby[i].playerSymbol = newSymbol;
                    image.sprite = ((NetworkGameLobbyView)menuView).myNetworkManager.possibleSymbols.GetSprite(newSymbol);
                    if (animateChange) EffectivateChange();
                    break;
                }
            }

        }

    }

    new public void EffectivateChange()
    {
        menuView.UpdateView();
        imageAnimator.SetTrigger("change");
        AudioManager.Instance.PlayOptionSelectSFX();
    }
}
