using UnityEngine;
using DG.Tweening;
using TMPro;

public class PopUpInformation : MonoBehaviour
{
    public RectTransform container, borderLeft, borderRight;
    public TextMeshProUGUI textMeshPro;

    private CanvasGroup canvasGroup;
    private bool isTweening = false;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void TriggerPopUpInformation()
    {
        if (isTweening)
        {
            // Avoid triggering the pop-up while it's still animating
            return;
        }

        DOTween.Init();

        container.sizeDelta = new Vector2(0f, container.sizeDelta.y);
        container.DOSizeDelta(new Vector2(426f, container.sizeDelta.y), 1f).SetEase(Ease.InOutCubic);

        float containerWidthChange = 426f - container.sizeDelta.x;
        float borderLeftNewX = borderLeft.anchoredPosition.x - containerWidthChange / 2f;
        float borderRightNewX = borderRight.anchoredPosition.x + containerWidthChange / 2f;
        borderLeft.anchoredPosition = new Vector2(borderLeftNewX, borderLeft.anchoredPosition.y);
        borderRight.anchoredPosition = new Vector2(borderRightNewX, borderRight.anchoredPosition.y);

        Color initialColor = textMeshPro.color;
        initialColor.a = 0f;
        textMeshPro.color = initialColor;
        textMeshPro.DOColor(new Color(initialColor.r, initialColor.g, initialColor.b, 1f), 1f).SetEase(Ease.InOutCubic);

        canvasGroup.alpha = 0f;
        canvasGroup.DOFade(0.75f, 1f).SetEase(Ease.InOutCubic).OnComplete(() =>
        {
            canvasGroup.DOFade(0f, 0.5f).SetEase(Ease.InOutCubic).SetDelay(0.5f).OnComplete(() =>
            {
                // Reset to default values when the animation is complete
                ResetToDefault();
            });
        });

        isTweening = true;
    }

    private void ResetToDefault()
    {
        container.sizeDelta = new Vector2(0f, container.sizeDelta.y);
        borderLeft.anchoredPosition = Vector2.zero;
        borderRight.anchoredPosition = Vector2.zero;
        textMeshPro.color = new Color(textMeshPro.color.r, textMeshPro.color.g, textMeshPro.color.b, 0f);
        canvasGroup.alpha = 0f;

        isTweening = false;
    }
}
