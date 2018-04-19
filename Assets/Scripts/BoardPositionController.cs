using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardPositionController : MonoBehaviour {

    private SpriteRenderer m_spriteRenderer;
    private BoardManager m_boardManager;
    private int index;

    private void Awake()
    {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_boardManager = GetComponentInParent<BoardManager>();
    }

    public void SetSprite(PlayerSprite playerSprite)
    {
        m_spriteRenderer.sprite = playerSprite.sprite;
        m_spriteRenderer.color = playerSprite.color;
    }

    public void ResetSprite()
    {
        m_spriteRenderer.sprite = null;
    }

    public void SetIndex(int i)
    {
        index = i;
    }

    private void OnMouseDown()
    {
        if (GameManager.playerCanInteract)
            m_boardManager.AddPlayerToBoard(index);

    }

}
