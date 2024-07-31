using UnityEngine;
using DG.Tweening;

public class PopUpExit : MonoBehaviour
{
    public GameObject popup;
    public CanvasGroup canvasGroup;
    public float popupFadeDuration = 0.5f;
    public Ease popupEase = Ease.InOutCubic;

    [Header("Exit")]
    public GameObject object1;
    public GameObject object2;

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    public void NotExit()
    {
        HidePopup();
    }

    public void ShowPopup()
    {
        if (!popup.activeSelf)
        {
            popup.SetActive(true);
            canvasGroup.alpha = 0f;
            canvasGroup.DOFade(1.0f, popupFadeDuration).SetEase(popupEase);
        }
    }

    public void HidePopup()
    {
        if (popup.activeSelf)
        {
            canvasGroup.DOFade(0.0f, popupFadeDuration).SetEase(popupEase).OnComplete(() => popup.SetActive(false));
        }

        object1.SetActive(true);
        object2.SetActive(true);
    }
}
