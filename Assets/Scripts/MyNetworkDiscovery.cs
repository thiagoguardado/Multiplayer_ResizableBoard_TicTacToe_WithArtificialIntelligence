using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MyNetworkDiscovery : NetworkDiscovery
{
    public static bool isBroadcasting = false;

    public void InitializeAsServer()
    {
        Initialize();
        StartAsServer();
        isBroadcasting = true;
    }

    public void InitializeAsClient()
    {
        Initialize();
        StartAsClient();
        isBroadcasting = true;
    }

    public void RestartAsServer()
    {
        StartAsServer();
        isBroadcasting = true;
    }

    public void RestartAsClient()
    {
        StartAsClient();
        isBroadcasting = true;
    }

    public void MyStopBroadcast()
    {
        if (isBroadcasting)
        {
            StopBroadcast();
            isBroadcasting = false;
        }
    }
 

}
