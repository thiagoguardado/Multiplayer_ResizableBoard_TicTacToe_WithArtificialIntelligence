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

    public bool onlyClient = false;
    public MyNetworkManager myNetworkManager { get; private set; }

    void Start()
    {
        // se cliente
        if (NetworkClient.active && !NetworkServer.active)
        {
            onlyClient = true;
        }

        if (onlyClient)
        {
            playButton.interactable = false;
            lessSignButton.gameObject.SetActive(false);
            greaterSignButton.gameObject.SetActive(false);
        }

        myNetworkManager = NetworkManager.singleton.GetComponent<MyNetworkManager>();

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

        UpdateBoardSize();
        UpdateMenuPlayerBoxes();
    }

    private void UpdateBoardSize()
    {
        boardSize.text = GameManager.boardSize.ToString();

        // checkButtons
        CheckLesserAndGreaterSigns(GameManager.boardSize, MenuManager.Instance.PossibleBoardSize, boardSizeLesserSign, boardSizeGreaterSign);

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
                ((NetworkLobbyPlayerBox)menuPlayerBoxes[i]).Setup(possibleSymbols.GetSprite(p.playerSymbol), p.color, p.playerName, p.connectionID, this);

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
            MyNetworkManager.Discovery.StopBroadcast();
            SceneManager.LoadScene("TitleScreen");
        }

    }
}
