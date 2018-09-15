using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkClient : NetworkBehaviour {

    public MatchData.MatchPlayer player;
    MyNetworkManager mynetwork;

    void Awake()
    {
        mynetwork = NetworkManager.singleton.GetComponent<MyNetworkManager>();

       
    }

    void Initialize(MatchData.MatchPlayer player)
    {
        this.player = player;
    }

}
