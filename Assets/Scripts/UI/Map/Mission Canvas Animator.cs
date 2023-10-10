using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class MissionCanvasAnimator : MonoBehaviour
{
    [SerializeField] private List<CanvasGroupAnimationSettings> canvasGroupSettings = new List<CanvasGroupAnimationSettings>();

    [System.Serializable]
    public class CanvasGroupAnimationSettings
    {
        public CanvasGroup canvasGroup;
        public float fadeDuration = 1f;
        public Vector3 startPosition;
        public Vector3 targetPosition;
    }

    public void RefreshAnimation()
    {
        foreach (CanvasGroupAnimationSettings settings in canvasGroupSettings)
        {
            CanvasGroup canvasGroup = settings.canvasGroup;
            if (canvasGroup != null)
            {
                // Set the initial alpha to 0 to start with a fade-in effect
                canvasGroup.alpha = 0f;

                // Create a DoTween sequence
                Sequence sequence = DOTween.Sequence();

                // Add a fade animation to 1f with the specified duration
                sequence.Append(canvasGroup.DOFade(1f, settings.fadeDuration).From(0f));

                // Add a move animation to the target position
                sequence.Join(canvasGroup.transform.DOLocalMove(settings.targetPosition, settings.fadeDuration).From(settings.startPosition));

                // Play the sequence
                sequence.Play();
            }
            else
            {
                Debug.LogError("CanvasGroup component not found in the list.");
            }
        }
    }
}
