using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmbedKeyboard : MonoBehaviour
{
    public TNVirtualKeyboard virtualKeyboard;
    public GameObject virtualKeyboardParent;
    public TMPro.TMP_InputField usernameLoginTextbox, passwordLoginTextbox, usernameRegisterTextbox, passwordRegisterTextbox, emailRegisterTextbox, virtualKeyboardInputfield;
    private TMPro.TMP_InputField activeTextMesh = null;
    private string previousTextValue = "";

    private void Update()
    {
        if (activeTextMesh != null && activeTextMesh.text != previousTextValue)
        {
            previousTextValue = activeTextMesh.text;
            virtualKeyboardInputfield.text = activeTextMesh.text;
        }
    }

    public void emailLoginKeyboardButtonOnclick()
    {
        virtualKeyboardParent.SetActive(true);
        virtualKeyboard.targetText = usernameLoginTextbox;
        virtualKeyboard.exampleTxt = usernameLoginTextbox;
        activeTextMesh = usernameLoginTextbox;
    }

    public void passwordLoginKeyboardButtonOnclick()
    {
        virtualKeyboardParent.SetActive(true);
        virtualKeyboard.targetText = passwordLoginTextbox;
        virtualKeyboard.exampleTxt = passwordLoginTextbox;
        activeTextMesh = passwordLoginTextbox;
    }

    public void usernameRegisterKeyboardButtonOnclick()
    {
        virtualKeyboardParent.SetActive(true);
        virtualKeyboard.targetText = usernameRegisterTextbox;
        virtualKeyboard.exampleTxt = usernameRegisterTextbox;
        activeTextMesh = usernameRegisterTextbox;
    }

    public void passwordRegisterKeyboardButtonOnclick()
    {
        virtualKeyboardParent.SetActive(true);
        virtualKeyboard.targetText = passwordRegisterTextbox;
        virtualKeyboard.exampleTxt = passwordRegisterTextbox;
        activeTextMesh = passwordRegisterTextbox;
    }

    public void emailRegisterKeyboardButtonOnclick()
    {
        virtualKeyboardParent.SetActive(true);
        virtualKeyboard.targetText = emailRegisterTextbox;
        virtualKeyboard.exampleTxt = emailRegisterTextbox;
        activeTextMesh = emailRegisterTextbox;
    }
}
