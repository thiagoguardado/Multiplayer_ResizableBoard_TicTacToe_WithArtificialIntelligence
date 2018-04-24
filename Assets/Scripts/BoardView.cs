using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardView : MonoBehaviour {

    public Transform positionsParent;
    public Transform gridParent;
    public GameObject gridLinePrefab;
    public GameObject gridPositionPrefab;

    // sprites
    public PlayerSprite circleSprite;
    public PlayerSprite crossSprite;
    public PlayerSprite triangleSprite;
    public PlayerSprite squareSprite;

    private List<BoardPositionController> m_boardPositions = new List<BoardPositionController>();
    private BoardManager m_boardManager;


    public void Initialize(BoardManager boardManager, int size)
    {
        m_boardManager = boardManager;
        CreateGrid(size);
        CreatePositions(size);
        ChangeSize(size);
        GetControllers();
    }

    private void ChangeSize(int size)
    {
        if (size > 3)
        {
           transform.localScale = Vector3.one * (1 - ((float)size - 3)/(float)size);
        }

    }

    private void CreateGrid(int size) {

        GameObject go;

        for (int i = 0; i < size-1; i++)
        {
            // horizontal
            go = Instantiate(gridLinePrefab, new Vector3(0, i*2 - (size-2) , 0), Quaternion.identity,gridParent);
            go.transform.localScale = Vector3.one * size;
            go.GetComponent<LineRenderer>().widthMultiplier = 0.25f * (1 - ((float)size - 3) / (float)size);

            // vertical
            go = Instantiate(gridLinePrefab, new Vector3(i * 2 - (size - 2),0, 0), Quaternion.Euler(0,0,90), gridParent);
            go.transform.localScale = Vector3.one * size;
            go.GetComponent<LineRenderer>().widthMultiplier = 0.25f * (1 - ((float)size - 3) / (float)size);
        }


        //outer borders

        List<GameObject> l = new List<GameObject>();
        l.Add(Instantiate(gridLinePrefab, new Vector3(0, (size - 1) * 2 - (size - 3), 0), Quaternion.identity, gridParent));
        l.Add(Instantiate(gridLinePrefab, new Vector3(0, (-2) * 2  - (size - 3), 0), Quaternion.identity, gridParent));
        l.Add(Instantiate(gridLinePrefab, new Vector3((-2) * 2 - (size - 3), 0, 0), Quaternion.Euler(0, 0, 90), gridParent));
        l.Add(Instantiate(gridLinePrefab, new Vector3((size - 1) * 2 - (size - 3), 0, 0), Quaternion.Euler(0, 0, 90), gridParent));

        for (int i = 0; i < l.Count; i++)
        {
            l[i].transform.localScale = Vector3.one * (size + 1);
        }


    }

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

    private void GetControllers() {

        BoardPositionController bpc;
        for (int i = 0; i < positionsParent.childCount; i++)
        {
            bpc = positionsParent.GetChild(i).GetComponent<BoardPositionController>();
            bpc.SetIndex(i);
            m_boardPositions.Add(bpc);
        }
    }

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

}

[System.Serializable]
public class PlayerSprite {
    public Sprite sprite;
    public Color color;
}
