using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Controls backgriund and sound effects 
/// </summary>
public class AudioManager : MonoBehaviour {

    public static AudioManager Instance; // singleton

    public AudioMixer audioMixer;       // main audio mixer
    public AudioSource bgmAudioSource;  // audio source for background music
    public AudioSource sfxAudioSource;  // audio source for sound effects

    public AudioClip menuBGM;           // menus BGM
    public AudioClip ingamrBGM;         // game BGM
    public AudioClip[] playersSFX;      // list of possible players action sfx
    public AudioClip optionSelectSFX;   // menu options click sfx
    public AudioClip startgameSFX;      // menu start game option click sfx
    public AudioClip endGameSFX;        // game end reached sound effect
    public AudioClip returnSFX;         // return to previous menu sfx

    public AudioMixerSnapshot defaultMusicSnapshot; // default snapshot on main audio mixer
    public AudioMixerSnapshot noMusicSnapshot;      // muted bgm snapshot
    public AudioMixerSnapshot bgmHalfPitchSnapshot; // half pitch bgm snapchot

    public float fadeTime = 0.5f; // bgm fade in and fade out time

    private void Awake()
    {

        // singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // start playing menu theme
        StartCoroutine(PlayMenuTheme());

    }

    /// <summary>
    /// set bgm to in game theme
    /// </summary>
    public void ChangeToInGameMusic()
    {
        StartCoroutine(ChangeMusic(ingamrBGM));
    }

    /// <summary>
    /// set bgm to menu theme
    /// </summary>
    public void ChangeToMenuMusic()
    {
        StartCoroutine(ChangeMusic(menuBGM));
    }

    /// <summary>
    /// shuffles players action sfx list
    /// </summary>
    public void ShufflePlayerAudio()
    {
        for (int i = 0; i < playersSFX.Length; i++)
        {
            int shuffleIndex = UnityEngine.Random.Range(0, playersSFX.Length);
            AudioClip previousClip = playersSFX[shuffleIndex];
            playersSFX[shuffleIndex] = playersSFX[i];
            playersSFX[i] = previousClip;
        }
    }

    /// <summary>
    /// play player action sfx based on its index
    /// </summary>
    /// <param name="playerIndex"></param>
    public void PlayPlayerSFX(int playerIndex)
    {
        sfxAudioSource.PlayOneShot(playersSFX[playerIndex]);
    }

    /// <summary>
    /// play menu option selected sfx
    /// </summary>
    public void PlayOptionSelectSFX()
    {
        sfxAudioSource.PlayOneShot(optionSelectSFX);
    }

    /// <summary>
    /// play menu start game option sfx
    /// </summary>
    public void PlayStartGameSFX()
    {
        sfxAudioSource.PlayOneShot(startgameSFX);
    }

    /// <summary>
    /// play return to previous menu sfx
    /// </summary>
    public void PlayReturnSFX()
    {
        sfxAudioSource.PlayOneShot(returnSFX);
    }

    /// <summary>
    /// changes bgm and play end game reached sfx
    /// </summary>
    public void PlayEndGame()
    {
        bgmHalfPitchSnapshot.TransitionTo(fadeTime);
        sfxAudioSource.PlayOneShot(endGameSFX);
    }


    // starts playing menu theme
    private IEnumerator PlayMenuTheme()
    {
        yield return null;  // needs to skip a frame due to bug
        FadeIn();           // fade music in
        PlayTheme(menuBGM); // start playing menu theme
    }

    /// <summary>
    /// Sets and play a music in bgm audio source
    /// </summary>
    /// <param name="clip"></param>
    private void PlayTheme(AudioClip clip)
    {
        bgmAudioSource.clip = clip;
        bgmAudioSource.loop = true;
        bgmAudioSource.Play();
    }

    /// <summary>
    /// Change current music
    /// </summary>
    IEnumerator ChangeMusic(AudioClip newMusic)
    {
        // fades out
        FadeOut();

        // wait for fading time
        yield return new WaitForSeconds(fadeTime);

        // setup new music
        PlayTheme(newMusic);

        // fades in
        FadeIn();
    }

    /// <summary>
    /// Fades bgm out
    /// </summary>
    private void FadeOut()
    {
        noMusicSnapshot.TransitionTo(fadeTime);

    }

    /// <summary>
    /// Fades bgm in
    /// </summary>
    private void FadeIn()
    {
        defaultMusicSnapshot.TransitionTo(fadeTime);
    }

}
