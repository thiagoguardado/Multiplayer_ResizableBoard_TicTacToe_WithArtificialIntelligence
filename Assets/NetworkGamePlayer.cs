using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkGamePlayer : NetworkBehaviour {

    public MyNetworkManager mynetworkManager { get; private set; }
    float timer = 1f;
    float refreshTime = 1f;

    private bool canPlay = false;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }


    void Start()
    {
        mynetworkManager = NetworkManager.singleton.GetComponent<MyNetworkManager>();
    }


    void Update()
    {
        if (canPlay)
        {

        }
    }

}
