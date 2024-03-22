using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class PauseController : MonoBehaviour
{
    public GameObject canvas;
    public Image Overlay;
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
        //Overlay.DOFade(0.4f, 0.5f).From(0f);
    }

    public void Exit()
    {
        GameManager.PauseExit();
    }

    public void Back()
    {
        canvas.SetActive(false);
        Time.timeScale = 1;
    }
    public void ToggleGodMode()
    {
        GameManager.ToggleGodMode();

    }

    public void Restart()
    {
        GameManager.GameReset();
    }

    void showMenu()
    {
        canvas.SetActive(!canvas.activeSelf);
        Time.timeScale = (Time.timeScale + 1) % 2;
    }
}
