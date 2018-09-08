using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
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

    public void PressStartLANNetworkButton()
    {

        titleAnimator.SetBool("Snap", true);
        AudioManager.Instance.PlayStartGameSFX();
        MyNetworkManager.NetworkType = NetworkType.LAN;
        StartCoroutine(WaitAndChangeScreen("NetworkGameSetup"));

    }

    public void PressStartInternetNetworkButton()
    {

        titleAnimator.SetBool("Snap", true);
        AudioManager.Instance.PlayStartGameSFX();
        MyNetworkManager.NetworkType = NetworkType.Internet;
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
