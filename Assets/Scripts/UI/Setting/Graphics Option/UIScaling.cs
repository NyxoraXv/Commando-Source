using UnityEngine;
using UnityEngine.UI;

public class UIScaling : MonoBehaviour
{
    public CanvasScaler canvasScaler;
    public Slider scaleFactorSlider;

    private float minScaleFactor = 0.01f;  // Adjusted minScaleFactor
    private float maxScaleFactor = 1.2f;   // Adjusted maxScaleFactor

    void Start()
    {
        // Ensure you have references to the Canvas Scaler and Slider components
        if (canvasScaler == null)
        {
            canvasScaler = GetComponent<CanvasScaler>();
        }

        if (scaleFactorSlider == null)
        {
            Debug.LogError("Please assign the UI Slider for controlling the scale factor.");
        }

        // Subscribe to the slider's value change event
        scaleFactorSlider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    void OnSliderValueChanged(float value)
    {
        // Use an exponential mapping to adjust the scale factor
        float scale = Mathf.Pow(maxScaleFactor, value);

        // Ensure the scale factor is within the desired range
        scale = Mathf.Clamp(scale, minScaleFactor, maxScaleFactor);

        // Adjust the reference resolution width based on the scale factor
        canvasScaler.referenceResolution = new Vector2(1920 * scale, 1080);
    }
}
