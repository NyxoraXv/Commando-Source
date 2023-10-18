using UnityEngine;
using UnityEngine.UI;

public class ResizeImageBasedOnScreen : MonoBehaviour
{
    private Image targetImage; // Reference to the Image component you want to resize

    private void Start()
    {
        targetImage = gameObject.GetComponent<Image>();

        // Get the current screen resolution
        int screenWidth = Screen.width;
        int screenHeight = Screen.height;

        // Set the desired width and height for your image
        float desiredWidth = screenWidth * 0.8f; // You can adjust this value as needed
        float desiredHeight = screenHeight * 0.5f; // You can adjust this value as needed

        // Set the size of the Image component
        SetImageSize(desiredWidth, desiredHeight);
    }

    private void SetImageSize(float width, float height)
    {
        // Check if the targetImage is assigned
        if (targetImage != null)
        {
            // Get the RectTransform component of the Image
            RectTransform imageRectTransform = targetImage.rectTransform;

            // Set the size of the Image component
            imageRectTransform.sizeDelta = new Vector2(width, height);
        }
        else
        {
            Debug.LogWarning("Target Image component is not assigned.");
        }
    }
}
