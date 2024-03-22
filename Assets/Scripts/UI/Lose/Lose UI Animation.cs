using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LoseUIAnimation : MonoBehaviour
{
    private Image targetImage;
    public float duration = 2f;
    
    private void OnEnable()
    {
        targetImage = this.GetComponent<Image>();
        targetImage.fillAmount = 0f;
        TweenFillAmount();
    }

    void TweenFillAmount()
    {
        targetImage.DOFillAmount(1f, duration)
            .SetEase(Ease.InOutCubic) // You can change the easing function if needed
            .OnComplete(() => Debug.Log("FillAmount animation completed")); // Optional completion callback
    }
}
