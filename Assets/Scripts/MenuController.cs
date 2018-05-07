using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls player input on menu screen
/// </summary>
public class MenuController : MonoBehaviour {


    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            Return();
        }

    }

    public void StartGame()
    {

        GameManager.Instance.StartGame();
        AudioManager.Instance.PlayStartGameSFX();

    }

    public void Return()
    {
        GameManager.Instance.ReturnToTitleScreen();
        AudioManager.Instance.PlayReturnSFX();
    }

}
