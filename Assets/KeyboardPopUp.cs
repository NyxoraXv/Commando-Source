using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class KeyboardPopUp : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Key keyPrefab;
    [SerializeField] private Key backspacekeyPrefab;

    [Header(" Settings ")]
    [Range(0f, 2f)]
    [SerializeField] private float widthPercent;
    [Range(0f, 1f)]
    [SerializeField] private float heightPrecent;
    [Range(.5f, -.75f)]
    [SerializeField] private float bottomOffset;

    [Header(" Keyboard Lines")]
    [SerializeField] private KeyboardLine[] lines;

    [Header(" Key Settings ")]
    [Range(0f, 1f)]
    [SerializeField] private float keyToLineRatio;
    [Range(0f, 2f)]
    [SerializeField] private float keyXSpacing;

    [Header(" Events ")]
    public Action<char> onKeyPressed;
    public Action onBackspacePressed;


    // Start is called before the first frame update
    IEnumerator Start()
    {
        CreateKeys();
        
        yield return null;

        UpdateRectTransform();

        //rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height / 2);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateRectTransform();
        PlaceKeys();
    }

    private void UpdateRectTransform()
    {
        float width = widthPercent * Screen.width;
        float height = heightPrecent * Screen.height;

        rectTransform.sizeDelta = new Vector2(width, height);

        Vector2 position;

        position.x = Screen.width / 1024;
        position.y = bottomOffset * Screen.height + height / 2;

        rectTransform.position = position;
    }

    private void CreateKeys()
    {
        for (int i = 0; i < lines.Length; i++)
        {
            for ( int j = 0; j < lines[i].keys.Length; j++)
            {
                char key = lines[i].keys[j];

                if(key == '.')
                {
                Key keyInstance = Instantiate(backspacekeyPrefab, rectTransform);

                keyInstance.GetButton().onClick.AddListener(() => BackspacePressedCallback());
                }
                else
                {
                Key keyInstance = Instantiate(keyPrefab, rectTransform);
                keyInstance.SetKey(key);   

                keyInstance.GetButton().onClick.AddListener(() => KeyPressedCallback(key));
                }
            }
        }
    }

    private void PlaceKeys()
    {
        int lineCount = lines.Length;

        float lineHeight = rectTransform.rect.height  / lineCount;

        float keyWidth = lineHeight * keyToLineRatio;
        float xSpacing = keyXSpacing * lineHeight;

        int currentKeyIndex = 0;

        for (int i = 0; i < lineCount; i++)
        {
            bool containsBackspace = lines[i].keys.Contains(".");

            float halfKeyCount = (float)lines[i].keys.Length / 2;

            if (containsBackspace)
            halfKeyCount += .5f;
            
            float startX = rectTransform.position.x - (keyWidth + xSpacing) * halfKeyCount + (keyWidth + xSpacing) / 2;

            float lineY = rectTransform.position.y + rectTransform.rect.height / 2 - lineHeight /2 - i * lineHeight;

            for ( int j = 0; j < lines[i].keys.Length; j++)
            {
                bool isBackspaceKey = lines[i].keys[j] == '.';

                float keyX = startX + j * (keyWidth + xSpacing);

                if(isBackspaceKey)
                keyX += keyWidth;

                Vector2 keyPosition = new Vector2(keyX, lineY);

                RectTransform keyRectTransform = rectTransform.GetChild(currentKeyIndex).GetComponent<RectTransform>();
                keyRectTransform.position = keyPosition;

                float thisKeyWidth = keyWidth;

                if (isBackspaceKey)
                thisKeyWidth *= 2f;

                keyRectTransform.sizeDelta = new Vector2(thisKeyWidth, keyWidth);

                currentKeyIndex++;
            }
        }
    }

    private void BackspacePressedCallback()
    {
        Debug.Log("Backspace pressed");

        onBackspacePressed?.Invoke();
    }

    private void KeyPressedCallback(char key)
    {
        Debug.Log("Key pressed : " + key);

        onKeyPressed?.Invoke(key);
    }
}

[System.Serializable]
public struct KeyboardLine
{
    public string keys;
}