using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

public class PauseController : MonoBehaviour
{
    public GameObject canvas;

    [Header("Settings")]
    public TextMeshProUGUI bgmText;
    public TextMeshProUGUI sfxText;
    public TextMeshProUGUI bgmTextCounter;
    public TextMeshProUGUI sfxTextCounter;
    public TextMeshProUGUI godModeText;

    private void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            showMenu();
        }
    }

    public void Open()
    {
        showMenu();
    }

    public void Exit()
    {
        GameplayManager.PauseExit();
    }

    public void Back()
    {
        canvas.SetActive(false);
        Time.timeScale = 1;
    }

    public void ToggleGodMode()
    {
        if (GameplayManager.ToggleGodMode())
            godModeText.SetText("GOD MODE ON");
        else
            godModeText.SetText("GOD MODE OFF");
    }



    void showMenu()
    {
        canvas.SetActive(!canvas.activeSelf);
        Time.timeScale = (Time.timeScale + 1) % 2;
    }
}
