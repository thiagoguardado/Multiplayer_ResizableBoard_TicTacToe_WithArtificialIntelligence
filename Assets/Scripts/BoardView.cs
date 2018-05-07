using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls board appearence
/// </summary>
public class BoardView : MonoBehaviour {

    // references
    public Transform positionsParent;
    public Transform gridParent;
    public UIView uiView;
    public Animator gridAnimator;
    private BoardManager m_boardManager;
    private List<LineRenderer> gridLineRenderes = new List<LineRenderer>();

    // prefabs
    public GameObject gridLinePrefab;
    public GameObject gridPositionPrefab;

    // sprites
    public PlayerSprite circleSprite;
    public PlayerSprite crossSprite;
    public PlayerSprite triangleSprite;
    public PlayerSprite squareSprite;

    // list of positions
    private List<BoardPositionController> m_boardPositions = new List<BoardPositionController>();



    public void Initialize(BoardManager boardManager, int size)
    {
        m_boardManager = boardManager;
        CreateGrid(size);
        CreatePositions(size);
        ChangeSize(size);
        GetControllers();
        AudioManager.Instance.ShufflePlayerAudio();
    }


    /// <summary>
    /// Change board size
    /// </summary>
    /// <param name="size"></param>
    private void ChangeSize(int size)
    {
        if (size > 3)
        {
           transform.localScale = Vector3.one * (1 - ((float)size - 3)/((float)size));
        }

        transform.localPosition = new Vector3(0, -1.25f, 0);


    }

    /// <summary>
    /// Creates grid of lines
    /// </summary>
    /// <param name="size"></param>
    private void CreateGrid(int size) {

        GameObject go;
        LineRenderer lr;

        gridLineRenderes.Clear();

        for (int i = 0; i < size-1; i++)
        {
            // horizontal
            go = Instantiate(gridLinePrefab, new Vector3(0, i*2 - (size-2) , 0), Quaternion.identity,gridParent);
            go.transform.localScale = Vector3.one * size;
            lr = go.GetComponent<LineRenderer>();
            lr.widthMultiplier = 0.25f * (1 - ((float)size - 3) / (float)size);

            // vertical
            go = Instantiate(gridLinePrefab, new Vector3(i * 2 - (size - 2),0, 0), Quaternion.Euler(0,0,90), gridParent);
            go.transform.localScale = Vector3.one * size;
            lr = go.GetComponent<LineRenderer>();
            lr.widthMultiplier = 0.25f * (1 - ((float)size - 3) / (float)size);
        }


        //outer borders

        List<GameObject> l = new List<GameObject>();
        l.Add(Instantiate(gridLinePrefab, new Vector3(0, (size - 0.5f) * 2 - (size - 2), 0), Quaternion.identity, gridParent));
        l.Add(Instantiate(gridLinePrefab, new Vector3(0, (-1.5f) * 2  - (size - 2), 0), Quaternion.identity, gridParent));
        l.Add(Instantiate(gridLinePrefab, new Vector3((-1.5f) * 2 - (size - 2), 0, 0), Quaternion.Euler(0, 0, 90), gridParent));
        l.Add(Instantiate(gridLinePrefab, new Vector3((size - 0.5f) * 2 - (size - 2), 0, 0), Quaternion.Euler(0, 0, 90), gridParent));

        for (int i = 0; i < l.Count; i++)
        {
            gridLineRenderes.Add(l[i].GetComponent<LineRenderer>());
            l[i].transform.localScale = Vector3.one * (size + 1f);
        }


    }

    /// <summary>
    /// Instantiate position controller prefab on positions
    /// </summary>
    /// <param name="size"></param>
    private void CreatePositions(int size) {

        int index = 0;
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {

                GameObject go = Instantiate(gridPositionPrefab, new Vector3(j * 2 - (size-1), (-i) * 2 + (size - 1), 0), Quaternion.identity, positionsParent);
                go.name = "Position " + index;
                index++;
            }

        }

    }

    /// <summary>
    /// Gets all position controllers
    /// </summary>
    private void GetControllers() {

        BoardPositionController bpc;
        for (int i = 0; i < positionsParent.childCount; i++)
        {
            bpc = positionsParent.GetChild(i).GetComponent<BoardPositionController>();
            bpc.SetIndex(i);
            m_boardPositions.Add(bpc);
        }
    }


    /// <summary>
    /// Updates entire boardview
    /// </summary>
    public void UpdateView()
    {
        for (int i = 0; i < positionsParent.childCount; i++)
        {
            GamePlayer gp = m_boardManager.Board.FullBoard[i];
            if (gp == null)
            {
                m_boardPositions[i].ResetSprite();
            }
            else {
                m_boardPositions[i].SetSprite(gp.playerSprite);
            }
        }
    }

    /// <summary>
    /// Action executed when player added to board
    /// </summary>
    /// <param name="playerIndex"></param>
    internal void AddedPlayerToBoard(int playerIndex)
    {
        gridAnimator.SetTrigger("Tilt");
        AudioManager.Instance.PlayPlayerSFX(playerIndex);
    }


    /// <summary>
    /// Action executed when game found a winner
    /// </summary>
    /// <param name="winningPlayer"></param>
    public void PlayerWin(GamePlayer winningPlayer)
    {
        uiView.PlayerWin(winningPlayer);
        ChangeBoardColor(winningPlayer.playerSprite.color);
        AudioManager.Instance.PlayEndGame();

    }

    /// <summary>
    /// Change board color to match winner
    /// </summary>
    /// <param name="color"></param>
    private void ChangeBoardColor(Color color)
    {
        for (int i = 0; i < gridLineRenderes.Count; i++)
        {
            gridLineRenderes[i].material.color = color;
        }
    }

    /// <summary>
    /// Actions to execute when game reached a tie
    /// </summary>
    public void Tie()
    {
        uiView.Tie();
        AudioManager.Instance.PlayEndGame();
    }

}


[System.Serializable]
public class PlayerSprite {
    public Sprite sprite;
    public Color color;

    public PlayerSprite(Sprite sprite, Color color)
    {
        this.sprite = sprite;
        this.color = color;
    }
}
