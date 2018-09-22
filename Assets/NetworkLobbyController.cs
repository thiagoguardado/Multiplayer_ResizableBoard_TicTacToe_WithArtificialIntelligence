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
        MyNetworkManager.DiscconectAll();
        GameManager.Instance.ReturnToTitleScreen();

        Debug.Log("Back");
    }


}
