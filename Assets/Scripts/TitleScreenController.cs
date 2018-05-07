using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controls player input in title screen
/// </summary>
public class TitleScreenController : MonoBehaviour {

    public Animator titleAnimator;
    public float waitBeforeChangeScreen;
   

    public void PressStartButton()
    {

        titleAnimator.SetBool("Snap", true);
        StartCoroutine(WaitAndChangeScreen());
        AudioManager.Instance.PlayStartGameSFX();

    }

    private IEnumerator WaitAndChangeScreen()
    {

        yield return new WaitForSeconds(waitBeforeChangeScreen);

        SceneManager.LoadScene("GameSelection");

    }

    


}
