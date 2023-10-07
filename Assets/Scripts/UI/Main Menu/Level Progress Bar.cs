using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LevelProgressBar : MonoBehaviour
{

    private Image progressBarImage;

    private void Start()
    {

        LevelManager.Instance.onXPAdded += HandleXPAdded;
        progressBarImage = GetComponent<Image>();
    }

    private void OnDestroy()
    {
        LevelManager.Instance.onXPAdded -= HandleXPAdded;
    }

    public void HandleXPAdded(int xpAdded)
    {

        float maxXP = LevelManager.Instance.maxXPPerLevel[LevelManager.Instance.currentLevel];
        float currentXP = LevelManager.Instance.CurrentXP;
        float percentageProgress = currentXP / maxXP;

        DOTween.To(() => progressBarImage.fillAmount, x => progressBarImage.fillAmount = x, percentageProgress, 1f)
            .SetEase(Ease.OutCubic);
    }

    private void Refresh()
    {
        progressBarImage = GetComponent<Image>();
        float maxXP = LevelManager.Instance.maxXPPerLevel[LevelManager.Instance.currentLevel];
        float currentXP = LevelManager.Instance.CurrentXP;
        float percentageProgress = currentXP / maxXP;

        DOTween.To(() => progressBarImage.fillAmount, x => progressBarImage.fillAmount = x, percentageProgress, 1f)
            .SetEase(Ease.OutCubic);
    }

    private void OnEnable()
    {
        Refresh();
    }
}
