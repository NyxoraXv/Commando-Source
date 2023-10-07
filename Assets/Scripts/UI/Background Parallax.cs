using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BackgroundParallax : MonoBehaviour
{
    public static BackgroundParallax Instance;

    public void doParallax(float scaleEnd, float scaleDuration, Vector3 moveEnd, float moveDuration, Vector3 rotateEnd, float rotateDuration, Ease ParallaxEase)
    {
        Transform background = gameObject.transform;

        background.DOScale(scaleEnd, scaleDuration).SetEase(ParallaxEase);
        background.DOLocalMove(moveEnd, scaleDuration).SetEase(ParallaxEase);
        background.DOLocalRotate(rotateEnd, rotateDuration).SetEase(ParallaxEase);
    }
}
