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
   

    public void PressStartLocalButton()
    {

        titleAnimator.SetBool("Snap", true);
        AudioManager.Instance.PlayStartGameSFX();
        StartCoroutine(WaitAndChangeScreen("LocalGameSetup"));


    }

    public void PressStartNetworkButton()
    {

        titleAnimator.SetBool("Snap", true);
        AudioManager.Instance.PlayStartGameSFX();
        StartCoroutine(WaitAndChangeScreen("NetworkGameSetup"));

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


    private IEnumerator WaitAndChangeScreen(string nextScene)
    {

        yield return new WaitForSeconds(waitBeforeChangeScreen);

        SceneManager.LoadScene(nextScene);

    }

    


}
