using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkGamePlayer : NetworkBehaviour {

    public MyNetworkManager mynetworkManager { get; private set; }
    float timer = 1f;
    float refreshTime = 1f;

    private bool canPlay = false;

    private void OnEnable()
    {
        BoardManager.AddedPlayerToBoard += TriggerCommandPlayerAdded;
        BoardManager.BoardInitialized += UpdateCurrentPlayer;
    }

    private void OnDisable()
    {
        BoardManager.AddedPlayerToBoard -= TriggerCommandPlayerAdded;
        BoardManager.BoardInitialized -= UpdateCurrentPlayer;
    }

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }


    void Start()
    {
        mynetworkManager = NetworkManager.singleton.GetComponent<MyNetworkManager>();

        GameManager.playerCanInteract = false;
    }


    private void TriggerCommandPlayerAdded(int positionAdded)
    {
        if (isLocalPlayer)
        {
            UpdateCurrentPlayer();
            CmdPlayerAddedToBoard(positionAdded);
        }
    }

    [Command]
    private void CmdPlayerAddedToBoard(int positionAdded)
    {
        RpcPlayerAddedToBoard(positionAdded);
    }

    [ClientRpc]
    private void RpcPlayerAddedToBoard(int positionAdded)
    {
        BoardManager.Instance.AddPlayerToBoard(positionAdded);
    }

    private void UpdateCurrentPlayer()
    {
        if (isLocalPlayer)
        {
            if (BoardManager.Instance.Board.CurrentPlayer.playerSymbol == mynetworkManager.currentPlayer.playerSymbol)
            {
                GameManager.playerCanInteract = true;
            }
            else
            {
                GameManager.playerCanInteract = false;
            }
        }
    }

}
