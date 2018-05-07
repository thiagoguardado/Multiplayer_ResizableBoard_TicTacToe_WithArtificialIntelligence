using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {

    public static AudioManager Instance;

    public AudioMixer audioMixer; 
    public AudioSource bgmAudioSource;
    public AudioSource sfxAudioSource;

    public AudioClip menuBGM;
    public AudioClip ingamrBGM;
    public AudioClip[] playersSFX;
    public AudioClip optionSelectSFX;
    public AudioClip startgameSFX;
    public AudioClip endGameSFX;
    public AudioClip returnSFX;


    public AudioMixerSnapshot defaultMusicSnapshot;
    public AudioMixerSnapshot noMusicSnapshot;
    public AudioMixerSnapshot bgmHalfPitchSnapshot;


    public float fadeTime = 0.5f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        StartCoroutine(PlayMenuTheme());

    }


    public void ChangeToInGameMusic()
    {
        StartCoroutine(ChangeMusic(ingamrBGM));
    }

    public void ChangeToMenuMusic()
    {
        StartCoroutine(ChangeMusic(menuBGM));
    }

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

    public void PlayPlayerSFX(int playerIndex)
    {
        sfxAudioSource.PlayOneShot(playersSFX[playerIndex]);
    }

    public void PlayOptionSelectSFX()
    {
        sfxAudioSource.PlayOneShot(optionSelectSFX);
    }

    public void PlayStartGameSFX()
    {
        sfxAudioSource.PlayOneShot(startgameSFX);
    }

    public void PlayReturnSFX()
    {
        sfxAudioSource.PlayOneShot(returnSFX);
    }


    public void PlayEndGame()
    {
        bgmHalfPitchSnapshot.TransitionTo(fadeTime);
        sfxAudioSource.PlayOneShot(endGameSFX);
    }


    private IEnumerator PlayMenuTheme()
    {

        yield return null;
        FadeIn();
        PlayTheme(menuBGM);
    }

    private void PlayTheme(AudioClip clip)
    {
        bgmAudioSource.clip = clip;
        bgmAudioSource.loop = true;
        bgmAudioSource.Play();
    }

    IEnumerator ChangeMusic(AudioClip newMusic)
    {
        FadeOut();
        yield return new WaitForSeconds(fadeTime);
        PlayTheme(newMusic);
        FadeIn();
    }


    private void FadeOut()
    {
        noMusicSnapshot.TransitionTo(fadeTime);

    }

    private void FadeIn()
    {
        defaultMusicSnapshot.TransitionTo(fadeTime);
    }


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
