using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary>
/// Controls user input on credits screen
/// </summary>
public class CreditsScreenController : MonoBehaviour {

    public void ReturnToTitleScreen()
    {

        SceneManager.LoadScene("TitleScreen");

    }
}
