using UnityEngine;

public class ResolutionSettingsController : MonoBehaviour
{
    private Resolution[] resolutions;

    private void Start()
    {
        resolutions = Screen.resolutions;
        Debug.Log("Available resolutions:");
        foreach (var res in resolutions)
        {
            Debug.Log(res.width + " x " + res.height);
        }
    }

    public void SetResolution(int quality)
    {
        int resolutionIndex = GetResolutionIndex(quality);
        if (resolutionIndex >= 0 && resolutionIndex < resolutions.Length)
        {
            Screen.SetResolution(resolutions[resolutionIndex].width, resolutions[resolutionIndex].height, Screen.fullScreen);
            Debug.Log("Resolution set to " + resolutions[resolutionIndex].width + " x " + resolutions[resolutionIndex].height);
        }
        else
        {
            Debug.LogError("Invalid quality level or resolution index.");
        }
    }

    private int GetResolutionIndex(int quality)
    {
        switch (quality)
        {
            case 0: // Low quality
                return 0; // Set your low-quality resolution index here
            case 1: // Medium quality
                return 2; // Set your medium-quality resolution index here
            case 2: // High quality
                return 5; // Set your high-quality resolution index here
            default:
                Debug.LogError("Invalid quality level.");
                return -1; // Invalid quality setting
        }
    }
}
