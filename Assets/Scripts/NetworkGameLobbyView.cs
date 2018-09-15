using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Controls game configuration menu screen appearence
/// </summary>
public class NetworkGameLobbyView : MenuView
{
    MyNetworkManager mynetworkManager;

    void Start()
    {
        mynetworkManager = UnityEngine.Networking.NetworkManager.singleton.GetComponent<MyNetworkManager>();
        if (mynetworkManager != null)
        {
            Debug.Log("encontrou");
        }
    }

    void Update()
    {
        UpdateView();
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

        if (menuPlayerBoxes.Count != mynetworkManager.currentMatch.playersOnLobby.Count)
        {
            CreateNewPlayerBoxes();
        }

        for (int i = 0; i < mynetworkManager.currentMatch.playersOnLobby.Count; i++)
        {
            MatchData.MatchPlayer p = mynetworkManager.currentMatch.playersOnLobby[i];
            menuPlayerBoxes[i].Setup(p.symbolAndSprite.playerSprite, p.color, p.playerName, i, this);

        }
    }

    private void CreateNewPlayerBoxes()
    {
        for (int i = 0; i < menuPlayerBoxes.Count; i++)
        {
            Destroy(menuPlayerBoxes[i].gameObject);
        }

        menuPlayerBoxes.Clear();

        for (int i = 0; i < mynetworkManager.currentMatch.playersOnLobby.Count; i++)
        {
            MenuPlayerBox go = Instantiate(playerBoxPrefab, menuPlayerBoxesCenter.position, Quaternion.identity, menuPlayerBoxesCenter);
            go.transform.localPosition = new Vector3((i - ((float)mynetworkManager.currentMatch.playersOnLobby.Count - 1) / 2f) * 450f, 0, 0);
            menuPlayerBoxes.Add(go);
        }

    }

    public void BackButton()
    {
        // cancel network

        if (!MyNetworkManager.Discovery.isServer)
        {
            mynetworkManager.StopClient();

            SceneManager.LoadScene("TitleScreen");

        }
        else
        {

            mynetworkManager.StopHost();
            MyNetworkManager.Discovery.StopBroadcast();
            SceneManager.LoadScene("TitleScreen");
        }

    }
}
