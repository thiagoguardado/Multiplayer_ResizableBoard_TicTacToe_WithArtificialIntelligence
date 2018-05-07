using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Constrols player input and displays player sprite
/// </summary>
public class BoardPositionController : MonoBehaviour {

    private SpriteRenderer m_spriteRenderer;    // current sprite being displayed
    private BoardManager m_boardManager;        // current board manager controlling board
    private int index;                          // index on boardmanager array of positions

    private void Awake()
    {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_boardManager = GetComponentInParent<BoardManager>();
    }

    /// <summary>
    /// Set sprite and color
    /// </summary>
    /// <param name="playerSprite"></param>
    public void SetSprite(PlayerSprite playerSprite)
    {
        m_spriteRenderer.sprite = playerSprite.sprite;
        m_spriteRenderer.color = playerSprite.color;
    }

    /// <summary>
    /// Reset sprite to null
    /// </summary>
    public void ResetSprite()
    {
        m_spriteRenderer.sprite = null;
    }

    /// <summary>
    /// Set index
    /// </summary>
    /// <param name="i"></param>
    public void SetIndex(int i)
    {
        index = i;
    }

    /// <summary>
    /// Act if player clicks on position
    /// </summary>
    private void OnMouseDown()
    {
        if (GameManager.playerCanInteract)

            if (m_boardManager.Board.CurrentPlayer.playerType == PlayerType.Human)
            {
                m_boardManager.AddPlayerToBoard(index);
            }
    }

}
