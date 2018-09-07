using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public enum NetworkType {LAN, Internet}

public class MyNetworkManager : NetworkManager {

    public NetworkType NetworkType { get; set; }
}
