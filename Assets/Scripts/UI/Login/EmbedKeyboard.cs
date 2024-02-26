using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmbedKeyboard : MonoBehaviour
{
    public TNVirtualKeyboard virtualKeyboard;
    public GameObject virtualKeyboardParent;
    public TMPro.TMP_InputField usernameLoginTextbox, passwordLoginTextbox;

    public void usernameLoginKeyboardButtonOnclick()
    {
        virtualKeyboardParent.SetActive(true);
        virtualKeyboard.targetText = usernameLoginTextbox;
        virtualKeyboard.exampleTxt = usernameLoginTextbox;
    }
    public void passwordLoginKeyboardButtonOnclick()
    {
        virtualKeyboardParent.SetActive(true);
        virtualKeyboard.targetText = passwordLoginTextbox;
        virtualKeyboard.exampleTxt = passwordLoginTextbox;
    }

}
