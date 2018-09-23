using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkLobbyController : MonoBehaviour {


    public void StartGame()
    {
        /*
        GameManager.Instance.StartGame();
        AudioManager.Instance.PlayStartGameSFX();
        */

        Debug.Log("StartGame");
    }

    public void Return()
    {
        if (NetworkServer.active)
        {
            MyNetworkManager.ServerDiscconectAll();
        }
        else
        {
            MyNetworkManager.ClientDisconnectAll();
        }

        GameManager.Instance.ReturnToTitleScreen();
    }


}
