using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PinHighlighter : MonoBehaviour
{
    public ScrollRect scrollRect;
    public RectTransform targetObject; // Reference to the object you want to scroll to.
    public float scrollDuration = 1.0f; // Duration of the scroll animation.

    public void ScrollToTargetObject()
    {
        if (scrollRect != null && targetObject != null)
        {
            RectTransform content = scrollRect.content;

            // Calculate the target position based on the target object's position within the content.
            Vector3 targetPosition = targetObject.anchoredPosition;

            // Calculate the normalized position based on the target position
            Vector2 normalizedPosition = new Vector2(
                0.5f + (targetPosition.x / content.rect.width),
                0.65f + (targetPosition.y / content.rect.height)
            );

            // Clamp the normalized position to ensure it's within the valid range (0-1).
            normalizedPosition.x = Mathf.Clamp01(normalizedPosition.x);
            normalizedPosition.y = Mathf.Clamp01(normalizedPosition.y);

            // Use DOTween to animate the ScrollRect's normalized position.
            DOTween.To(() => scrollRect.normalizedPosition, x => scrollRect.normalizedPosition = x, normalizedPosition, scrollDuration)
                .SetEase(Ease.OutQuad) // You can change the ease type as needed.
                .Play();
        }
        else
        {
            Debug.LogError("ScrollRect or targetObject is not set!");
        }
        Highlight();
    }

    private void Highlight()
    {
        Image[] childImages = GetComponentsInChildren<Image>(false);

        for (int i = 0; i < childImages.Length; i++)
        {
            float whiteIncrement = 255f / childImages.Length; // Total white increase from 0 to 255.
            Color currentColor = new Color(255, 255, 255); // Start from white for each image.

            // Animate the scale change for the second image.
            if (i == 1)
            {
                childImages[i].transform.DOScale(3f, 0.5f).From(2f).SetEase(Ease.InOutCubic);
            }

            // Animate the color transition from white to more white for each image.
            childImages[i].DOColor(new Color(currentColor.r - (i * whiteIncrement / 255f), currentColor.g - (i * whiteIncrement / 255f), currentColor.b - (i * whiteIncrement / 255f), 1), 1f)
                .SetEase(Ease.Linear); // You can change the ease type as needed.
        }
    }









}
