using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkClient : NetworkBehaviour {

    public MatchPlayer player;
    MyNetworkManager mynetwork;

    void Start()
    {
        mynetwork = NetworkManager.singleton.GetComponent<MyNetworkManager>();
    }

    void Initialize(MatchPlayer player)
    {
        this.player = player;
    }




}
