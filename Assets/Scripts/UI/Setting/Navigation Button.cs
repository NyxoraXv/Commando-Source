using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class NavigationButton : MonoBehaviour
{
    enum NavigationBar
    {
        Graphics,
        Control,
        Sound
    }

    [SerializeField] GameObject graphicsPanel;
    [SerializeField] GameObject controlPanel;
    [SerializeField] GameObject soundPanel;
    [SerializeField] public RectTransform highlighter;

    private NavigationBar navbarSelection;

    void Start()
    {
        // Initially, hide all panels.
        graphicsPanel.SetActive(false);
        controlPanel.SetActive(false);
        soundPanel.SetActive(false);
    }

    public void OnGraphicsClicked(RectTransform button)
    {
        navbarSelection = NavigationBar.Graphics;
        MoveHighlighter(button);
        ShowSelectedPanel();
    }

    public void OnControlClicked(RectTransform button)
    {
        navbarSelection = NavigationBar.Control;
        MoveHighlighter(button);
        ShowSelectedPanel();
    }

    public void OnAudioClicked(RectTransform button)
    {
        navbarSelection = NavigationBar.Sound;
        MoveHighlighter(button);
        ShowSelectedPanel();
    }

    void MoveHighlighter(RectTransform button)
    {
        Vector2 newPosition = new Vector2(button.anchoredPosition.x, highlighter.anchoredPosition.y);

        // Calculate the new width based on the button's width
        float newWidth = button.sizeDelta.x;
        highlighter.DOSizeDelta(new Vector2(newWidth, highlighter.sizeDelta.y), 0.5f).SetEase(Ease.InOutCubic);

        // Move the highlighter to the new position
        highlighter.DOAnchorPos(newPosition, 0.5f).SetEase(Ease.InOutCubic);
    }



    void ShowSelectedPanel()
    {
        // First, hide all panels.
        graphicsPanel.SetActive(false);
        controlPanel.SetActive(false);
        soundPanel.SetActive(false);

        // Then, show the selected panel based on the navbarSelection.
        switch (navbarSelection)
        {
            case NavigationBar.Graphics:
                graphicsPanel.SetActive(true);
                break;
            case NavigationBar.Control:
                controlPanel.SetActive(true);
                break;
            case NavigationBar.Sound:
                soundPanel.SetActive(true);
                break;
        }
    }
}
