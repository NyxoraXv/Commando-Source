using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class LoginAnimation : MonoBehaviour
{
    public Transform LoginContainer;

    void Start()
    {
        LoginContainer.DOMoveY(0f, 4f).SetEase(Ease.OutCubic).From(new Vector3(0f, -2f, 0f));
        LoginContainer.GetComponent<CanvasGroup>().DOFade(1f, 2f).From(0f);
    }
}
