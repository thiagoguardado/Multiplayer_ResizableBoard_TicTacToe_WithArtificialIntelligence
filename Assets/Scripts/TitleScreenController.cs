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

    public void PressCreditsButton()
    {

        SceneManager.LoadScene("Credits");
        AudioManager.Instance.PlayOptionSelectSFX();

    }


    public void PressExitButton()
    {

        AudioManager.Instance.PlayOptionSelectSFX();
        Application.Quit();
    }


    private IEnumerator WaitAndChangeScreen()
    {

        yield return new WaitForSeconds(waitBeforeChangeScreen);

        SceneManager.LoadScene("GameSelection");

    }

    


}
