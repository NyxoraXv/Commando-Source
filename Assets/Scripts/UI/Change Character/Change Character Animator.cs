using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; // Import the DOTween namespace

public class UIAnimator : MonoBehaviour
{
    public Transform CardParent;
    public float tweenDuration = 1.0f; // The duration of each tween.
    public float delayBetweenTweens = 0.5f; // The delay between each child's tween.

    private void OnEnable()
    {
        // Initialize an array to store each child's Transform.
        Transform[] children = new Transform[CardParent.childCount];

        for (int i = 0; i < CardParent.childCount; i++)
        {
            Transform child = CardParent.GetChild(i);

            if (child.name.Contains("Card"))
            {
                Debug.Log("Found child with name containing 'Card': " + child.name);

                // Store the child's Transform in the array.
                children[i] = child;
            }
        }

        // Tween the X position of each child with a delay between tweens.
        for (int i = 0; i < children.Length; i++)
        {
            if (children[i] != null)
            {
                Vector3 targetPosition = children[i].position + new Vector3(5.200012f, 0.0f, 0.0f);
                Vector3 startPosition = targetPosition + new Vector3(200f, 0f, 0f);
                children[i].DOLocalMoveX(targetPosition.x, tweenDuration)
                    .From(startPosition) // Set the starting value here
                    .SetDelay(i * delayBetweenTweens) // Apply a delay based on the index.
                    .SetEase(Ease.InOutCubic); // You can adjust the easing function as needed.
            }
        }

        if (gameObject.GetComponent<CanvasGroup>() != null)
        {
            gameObject.GetComponent<CanvasGroup>().DOFade(1f, 2f).SetEase(Ease.OutCubic).From(0f);
        }

    }

}
