using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Controls game configuration menu screen appearence
/// </summary>
public class NetworkGameLobbyView : MenuView
{
    NetworkGameLobby lobby;
    public PlayerSymbols possibleSymbols;

    public Button playButton;
    public Button lessSignButton;
    public Button greaterSignButton;

    private bool onlyClient = true;
    public MyNetworkManager myNetworkManager { get; private set; }

    void Start()
    {
        myNetworkManager = NetworkManager.singleton.GetComponent<MyNetworkManager>();

        playButton.interactable = false;
        lessSignButton.gameObject.SetActive(false);
        greaterSignButton.gameObject.SetActive(false);
        playButton.interactable = false;

        UpdateView();
    }


    void Update()
    {

        if (lobby == null)
        {
            lobby = FindObjectOfType<NetworkGameLobby>();
        }
        else
        {
            UpdateView();
        }

    }

    public override void UpdateView()
    {
        UpdateButtons();
        UpdateBoardSize();
        UpdateMenuPlayerBoxes(); 
    }

    private void UpdateButtons()
    {
        // se cliente
        if (NetworkServer.active)
        {
            onlyClient = false;
        }

        if (!onlyClient)
        {
            playButton.interactable = true;
            lessSignButton.gameObject.SetActive(true);
            greaterSignButton.gameObject.SetActive(true);
            playButton.interactable = !(myNetworkManager.currentMatch.playersOnLobby.Length < 2);
        }
    }


    private void CheckLesserAndGreaterSigns(int currentNumber, int[] possibleNumbers, Image numberLesserSign, Image numberGreaterSign)
    {
        if (currentNumber == possibleNumbers[0])
        {
            numberLesserSign.color = signColorWhenDisabled;
            numberGreaterSign.color = Color.white;

        }
        else if (currentNumber == possibleNumbers[possibleNumbers.Length - 1])
        {
            numberLesserSign.color = Color.white;
            numberGreaterSign.color = signColorWhenDisabled;

        }
        else
        {
            numberLesserSign.color = Color.white;
            numberGreaterSign.color = Color.white;
        }
    }


    protected override void UpdateMenuPlayerBoxes()
    {
        // check if number of player boxes is equal to players
        if (lobby != null)
        {

            if (menuPlayerBoxes.Count != lobby.mynetworkManager.currentMatch.playersOnLobby.Length)
            {
                CreateNewPlayerBoxes();
            }

            for (int i = 0; i < lobby.mynetworkManager.currentMatch.playersOnLobby.Length; i++)
            {
                NetworkPlayer p = lobby.mynetworkManager.currentMatch.playersOnLobby[i];
                ((NetworkLobbyPlayerBox)menuPlayerBoxes[i]).SetupChange(possibleSymbols.GetSprite(p.playerSymbol), p.color, p.playerName, p.playerID, this);

            }
        }
    }

    private void CreateNewPlayerBoxes()
    {
        for (int i = 0; i < menuPlayerBoxes.Count; i++)
        {
            Destroy(menuPlayerBoxes[i].gameObject);
        }

        menuPlayerBoxes.Clear();

        for (int i = 0; i < lobby.mynetworkManager.currentMatch.playersOnLobby.Length; i++)
        {
            NetworkLobbyPlayerBox go = Instantiate(playerBoxPrefab as NetworkLobbyPlayerBox, menuPlayerBoxesCenter.position, Quaternion.identity, menuPlayerBoxesCenter);
            go.transform.localPosition = new Vector3((i - ((float)lobby.mynetworkManager.currentMatch.playersOnLobby.Length - 1) / 2f) * 450f, 0, 0);
            menuPlayerBoxes.Add(go);
        }

    }

    public void BackButton()
    {
        // cancel network

        if (!MyNetworkManager.Discovery.isServer)
        {
            lobby.mynetworkManager.StopClient();

            SceneManager.LoadScene("TitleScreen");
        }
        else
        {
            lobby.mynetworkManager.StopHost();
            MyNetworkManager.Discovery.MyStopBroadcast();
            MyNetworkDiscovery.isBroadcasting = false;
            SceneManager.LoadScene("TitleScreen");
        }

    }
}
