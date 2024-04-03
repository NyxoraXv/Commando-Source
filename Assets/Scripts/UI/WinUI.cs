using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class WinUI : MonoBehaviour
{
    public GameObject victoryScreen;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI luncText;
    public TextMeshProUGUI FRGText;
    public TextMeshProUGUI victoryTitle;
    public Button restartButton;
    public Button mainMenuButton;

    private int finalScore; // Example final score
    private int finalLUNC; // Example final LUNC score
    private int finalFRG; // Example final FRG score
    public float scoreAnimationDuration = 2f; // Duration for the score counting animation

    private void OnEnable()
    {
        finalScore = GameManager.GetScore();
        finalLUNC = Mathf.RoundToInt(GameManager.getLUNC());
        finalFRG = Mathf.RoundToInt(GameManager.getFRG());
        victoryScreen.SetActive(true);
        //AnimateVictoryScreen();

        CountScore(scoreText, finalScore);
        luncText.text = finalLUNC.ToString();
        FRGText.text = finalFRG.ToString();
    }

    void AnimateVictoryScreen()
    {
        // Example animation using DOTween
        victoryTitle.transform.localScale = Vector3.zero;
        victoryTitle.transform.DOScale(1f, 0.5f);

        scoreText.transform.localScale = Vector3.zero;
        scoreText.transform.DOScale(1f, 0.5f);

        restartButton.image.DOFade(1f, 0.5f);
        mainMenuButton.image.DOFade(1f, 0.5f);
    }

    void CountScore(TextMeshProUGUI textMeshPro, int finalScore)
    {
        // Example score counting animation using DOTween
        int currentScore = 0;
        DOTween.To(() => currentScore, x => currentScore = x, finalScore, scoreAnimationDuration)
            .OnUpdate(() =>
            {
                textMeshPro.text = currentScore.ToString();
            })
            .SetEase(Ease.Linear);
    }
}
