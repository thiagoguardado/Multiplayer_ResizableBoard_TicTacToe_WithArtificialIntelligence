using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


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
        label.text = type.ToString();
        index = _index;
        menuView = _menuView;
    }

    public void ChangeColor()
    {
        GameManager.players[index].color = new Color(Random.value, Random.value, Random.value);
        menuView.UpdateView();
        imageAnimator.SetTrigger("change");

    }

    public void ChangeType()
    {
        if (GameManager.players[index].playerType == PlayerType.AI)
        {
            GameManager.players[index].playerType = PlayerType.Human;
        }
        else
        {
            GameManager.players[index].playerType = PlayerType.AI;
        }
        menuView.UpdateView();

        textAnimator.SetTrigger("change");
    }

}
