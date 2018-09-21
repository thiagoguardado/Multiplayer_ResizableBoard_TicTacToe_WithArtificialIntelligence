using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// Controls game configuration menu screen appearence
/// </summary>
public abstract class MenuView : MonoBehaviour {

    public MenuPlayerBox playerBoxPrefab;

    public Text boardSize;
    public Image boardSizeGreaterSign;
    public Image boardSizeLesserSign;
    public Color signColorWhenDisabled;
    public Transform menuPlayerBoxesCenter;
    protected List<MenuPlayerBox> menuPlayerBoxes = new List<MenuPlayerBox>();


    public abstract void UpdateView();

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

    protected abstract void UpdateMenuPlayerBoxes();

    private void CreateNewPlayerBoxes()
    {
        for (int i = 0; i < menuPlayerBoxes.Count; i++)
        {
            Destroy(menuPlayerBoxes[i].gameObject);
        }

        menuPlayerBoxes.Clear();

        for (int i = 0; i < GameManager.numberOfPlayers; i++)
        {
            MenuPlayerBox go = Instantiate(playerBoxPrefab, menuPlayerBoxesCenter.position, Quaternion.identity, menuPlayerBoxesCenter);
            go.transform.localPosition = new Vector3((i - ((float)GameManager.numberOfPlayers - 1) / 2f) * 450f, 0, 0);
            menuPlayerBoxes.Add(go);
        }

    }
}
