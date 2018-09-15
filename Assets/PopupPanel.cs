using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PopupPanel : MonoBehaviour {

    public Text title;
    public InputField inputField;
    public Button button;
    public GameObject mainPanel;

    private UnityAction<string> buttonAction;

    private void Awake()
    {
        button.onClick.AddListener(ButtonClicked);
    }

    private void OpenPopup(string titleText, string inputFieldHint, string inputText, UnityAction<string> buttonAction )
    {
        this.buttonAction = buttonAction;
        mainPanel.SetActive(true);
        title.text = titleText;
        inputField.text = inputText;
        inputField.placeholder.GetComponent<Text>().text = inputFieldHint;
        EventSystem.current.SetSelectedGameObject(inputField.gameObject);
    }

    public void ClosePopup()
    {
        mainPanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void OpenChangeName(string currentName, UnityAction<string> action)
    {
        OpenPopup("Change Name", "type your name...", currentName, action);
    }
    public void OpenNewMatch(UnityAction<string> action)
    {
        OpenPopup("Create New Match:", "type match name...", "", action);
    }

    private void ButtonClicked()
    {
        buttonAction.Invoke(inputField.text);
    }

}
