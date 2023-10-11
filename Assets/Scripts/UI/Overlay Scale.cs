using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverlayScale : MonoBehaviour
{
    private void Start()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();

        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // You can modify the scaling factors as needed
        float scaleFactorX = 0.5f;
        float scaleFactorY = 0.5f;

        Vector3 newScale = new Vector3(screenWidth * scaleFactorX, screenHeight * scaleFactorY, 1f);
        rectTransform.localScale = newScale;
    }
}
