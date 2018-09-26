using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkLobbyController : MonoBehaviour {


    public void StartGame()
    {
        Debug.Log("Pressed Start Game");
        MyNetworkManager.Discovery.MyStopBroadcast();

        // update networked itens
        NetworkGameLobby[] clientsScript = FindObjectsOfType<NetworkGameLobby>();
        foreach (var script in clientsScript)
        {
            
            if (script.isLocalPlayer)
            {
                script.RpcStartGame();
                Debug.Log("Sent RPC");
                break;
            }
            
        }
        
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
