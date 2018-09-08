using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class NetworkGameLobby : MonoBehaviour {

    public void BackButton()
    {
        // cancel network

        if (!MyNetworkManager.Discovery.isServer)
        {
            NetworkManager.singleton.StopClient();

            SceneManager.LoadScene("TitleScreen");

        }
        else {

            NetworkManager.singleton.StopHost();
            MyNetworkManager.Discovery.StopBroadcast();
            SceneManager.LoadScene("TitleScreen");
        }

    }
}
