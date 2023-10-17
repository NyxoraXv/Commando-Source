using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Option : MonoBehaviour
{
    enum OptionSelection
    {
        Option1,
        Option2,
        Option3
    }

    [SerializeField] private RectTransform highlighter;

    private OptionSelection optionSelected;

    void Start()
    {
        optionSelected = OptionSelection.Option1; // Set the initial selection.
    }

    public void OnOptionClicked(RectTransform button)
    {
        if (button == null)
        {
            return;
        }

        switch (button.name)
        {
            case "Option1":
                optionSelected = OptionSelection.Option1;
                break;
            case "Option2":
                optionSelected = OptionSelection.Option2;
                break;
            case "Option3":
                optionSelected = OptionSelection.Option3;
                break;
        }

        MoveHighlighter(button);
    }

    void MoveHighlighter(RectTransform button)
    {
        if (highlighter == null || button == null)
        {
            return;
        }

        Vector2 newPosition = new Vector2(button.anchoredPosition.x, highlighter.anchoredPosition.y);

        float newWidth = button.sizeDelta.x;

        // Animate the highlighter's size and position.
        highlighter.DOSizeDelta(new Vector2(newWidth, highlighter.sizeDelta.y), 0.5f).SetEase(Ease.InOutCubic);
        highlighter.DOAnchorPos(newPosition, 0.5f).SetEase(Ease.InOutCubic);
    }
}
