using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private Text text;
    [SerializeField] private KeyboardPopUp keyboard;

    // Start is called before the first frame update
    void Start()
    {
        keyboard.onBackspacePressed += BackspacePressedCallback;
        keyboard.onKeyPressed += KeyPressedCallback;
    }

    private void BackspacePressedCallback()
    {
        if(text.text.Length > 0)
        text.text = text.text.Substring(0, text.text.Length - 1);
    }

    private void KeyPressedCallback(char key)
    {
        text.text += key.ToString();
    }
}
