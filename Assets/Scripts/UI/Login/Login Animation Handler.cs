using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class LoginAnimation : MonoBehaviour
{
    private Image Container;
    
    void Start()
    {
        Container = GetComponent<Image>();
        float endAmount = 1f;

        DOTween.To(() => Container.fillAmount, x => Container.fillAmount = x, endAmount, 1f)
        .SetEase(Ease.OutCubic);
    }
}
