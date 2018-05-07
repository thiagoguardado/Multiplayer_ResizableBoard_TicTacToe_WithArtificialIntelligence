using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls game configuration menu player box
/// </summary>
public class MenuPlayerBox : MonoBehaviour {

    public Image image;
    public Text label;
    private int index;
    private MenuView menuView;
    public Animator imageAnimator;
    public Animator textAnimator;

    public void Setup(Sprite sprite, Color color, PlayerType type, int _index, MenuView _menuView)
    {
        image.sprite = sprite;
        image.color = color;

        switch (type)
        {
            case PlayerType.Human:
                label.text = "Human";
                break;
            case PlayerType.AI_Easy:
                label.text = "AI EASY";
                break;
            case PlayerType.AI_Medium:
                label.text = "AI MEDIUM";
                break;
            case PlayerType.AI_Hard:
                label.text = "AI HARD";
                break;
            default:
                label.text = type.ToString();
                break;
        }

        
        index = _index;
        menuView = _menuView;
    }

    public void ChangeColor()
    {
        GameManager.players[index].color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
        menuView.UpdateView();
        imageAnimator.SetTrigger("change");
        AudioManager.Instance.PlayOptionSelectSFX();

    }

    public void ChangeType()
    {
        GameManager.players[index].playerType = (PlayerType)(((int)GameManager.players[index].playerType + 1) % Enum.GetValues(typeof(PlayerType)).Length);

        menuView.UpdateView();

        textAnimator.SetTrigger("change");

        AudioManager.Instance.PlayOptionSelectSFX();
    }

}
