using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controls player interaction with game screen except clickable board positions
/// </summary>
public class GameUserInputController : MonoBehaviour {

    private float timer = 0f;
    public float waitBeforeCanReturnToMenu = 1f;

    private void Update()
    {

        CheckEcapeClick();

        CheckClickAfterGameEnd();
    }

    private void CheckEcapeClick()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            GameManager.Instance.EscapeFromGameScene();
        }
    }

    private void CheckClickAfterGameEnd()
    {
        if (GameManager.GameState == GameState.GameFinished)
        {

            if (timer <= waitBeforeCanReturnToMenu)
            {
                timer += Time.deltaTime;
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    GameManager.Instance.EndGameClickAction();
                }
            }

        }
    }
}
