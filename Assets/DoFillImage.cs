using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ImageFillAmountTween : MonoBehaviour
{
    public float targetFillAmount = 1f;
    public float duration = 1f;

    public Image[] image;
    public Image overlay;
    
    void OnEnable()
    {
        TweenFillAmount();
        TweenOverlay();
    }

    void TweenFillAmount()
    {
        foreach (var item in image)
        {
            item.DOFillAmount(targetFillAmount, duration)
    .From(0f)
    .SetEase(Ease.InOutCubic)
    .OnComplete(OnTweenComplete);
        }

    }
    void OnTweenComplete()
    {
        Debug.Log("Tween completed!");
    }

    void TweenOverlay()
    {
        overlay.DOFade(0.4f, 0.5f).From(0f);
    }
}
