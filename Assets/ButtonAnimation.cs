using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HUDAnimation : MonoBehaviour
{
    public void Clicked(GameObject originalButton)
    {
        // Instantiate the button with the same parent as the original
        GameObject buttonInstance = Instantiate(originalButton, originalButton.transform.parent);

        // Get the Image component from the instantiated button
        Image buttonImage = buttonInstance.GetComponent<Image>();

        // Disable raycast on the button
        buttonImage.raycastTarget = false;

        // Set the initial scale and alpha
        buttonInstance.transform.localScale = Vector3.zero;
        buttonImage.color = new Color(buttonImage.color.r, buttonImage.color.g, buttonImage.color.b, 0f);

        // Use DOTween to animate the button's scale and alpha
        buttonInstance.transform.DOScale(0.8f, 1.2f);
        buttonImage.DOFade(1f, 0.5f)
            .OnComplete(() =>
            {
                // Enable raycast on the button when fade-in is complete
                buttonImage.raycastTarget = true;

                // Reverse the fade animation
                buttonImage.DOFade(0f, 0.5f)
                    .OnComplete(() => Destroy(buttonInstance)); // Destroy the button when the fade-out is complete
            });
    }
}
