using UnityEngine;
using UnityEngine.UI;

public class ScrollRectController : MonoBehaviour
{
    public ScrollRect scrollRect;
    public RectTransform targetObject; // Reference to the object you want to scroll to.

    public void ScrollToTargetObject()
    {
        if (scrollRect != null && targetObject != null)
        {
            RectTransform content = scrollRect.content;

            // Calculate the target position based on the target object's position within the content.
            Vector3 targetPosition = content.InverseTransformPoint(targetObject.position);

            // Calculate the normalized position based on the target position.
            Vector2 normalizedPosition = new Vector2(
                0.5f + (targetPosition.x / content.rect.width),
                0.5f + (targetPosition.y / content.rect.height)
            );

            // Clamp the normalized position to ensure it's within the valid range (0-1).
            normalizedPosition.x = Mathf.Clamp01(normalizedPosition.x);
            normalizedPosition.y = Mathf.Clamp01(normalizedPosition.y);

            // Set the ScrollRect's normalized position to scroll to the target position smoothly.
            scrollRect.normalizedPosition = normalizedPosition;
        }
        else
        {
            Debug.LogError("ScrollRect or targetObject is not set!");
        }
    }
}
