using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardView : MonoBehaviour {

    public PlayerSprite circleSprite;
    public PlayerSprite crossSprite;

    private List<BoardPositionController> m_boardPositions = new List<BoardPositionController>();
    private BoardManager m_boardManager;


	private void Awake() {
        m_boardManager = GetComponent<BoardManager>();
        InitializeBoardView();
    }

    private void InitializeBoardView() {
        BoardPositionController bpc;
        for (int i = 0; i < transform.childCount; i++)
        {
            bpc = transform.GetChild(i).GetComponent<BoardPositionController>();
            bpc.SetIndex(i);
            m_boardPositions.Add(bpc);
        }
    }

    public void UpdateView()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            switch (m_boardManager.board[i])
            {
                case Player.None:
                    m_boardPositions[i].ResetSprite();
                    break;
                case Player.Circle:
                    m_boardPositions[i].SetSprite(circleSprite);
                    break;
                case Player.Cross:
                    m_boardPositions[i].SetSprite(crossSprite);
                    break;
            }
        }
    }

}

[System.Serializable]
public class PlayerSprite {
    public Sprite sprite;
    public Color color;
}
