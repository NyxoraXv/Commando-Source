using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InputManager : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private TMP_Text textMeshPro;
    [SerializeField] private KeyboardPopUp keyboard;

    // Start is called before the first frame update
    void Start()
    {
        keyboard.onBackspacePressed += BackspacePressedCallback;
        keyboard.onKeyPressed += KeyPressedCallback;
    }

    private void BackspacePressedCallback()
    {
        if(textMeshPro.text.Length > 0)
        textMeshPro.text = textMeshPro.text.Substring(0, textMeshPro.text.Length - 1);
    }

    private void KeyPressedCallback(char key)
    {
        textMeshPro.text += key.ToString();
    }
}
