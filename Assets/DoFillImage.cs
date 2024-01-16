using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ImageFillAmountTween : MonoBehaviour
{
    public float targetFillAmount = 1f;
    public float duration = 1f;

    private Image image;
    
    void OnEnable()
    {
        image = GetComponent<Image>();

        // Start the fillAmount tween
        TweenFillAmount();
    }

    void TweenFillAmount()
    {
        // Use DOTween to animate the fillAmount property with InOutCubic ease
        image.DOFillAmount(targetFillAmount, duration)
            .From(0f)
            .SetEase(Ease.InOutCubic)
            .OnComplete(OnTweenComplete); // Optional: Add a callback for completion
    }

    void OnTweenComplete()
    {
        Debug.Log("Tween completed!");
    }
}
