using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controls player interaction with game screen after the game finishes
/// </summary>
public class GameUiController : MonoBehaviour {

    private float timer = 0f;
    public float waitBeforeCanReturnToMenu = 1f;

    private void Update()
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
                    AudioManager.Instance.PlayReturnSFX();
                    GameManager.Instance.ReturnToMenu();

                }
            }

        }
    }





}
