using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class VolumeControl : MonoBehaviour
{
    public AudioMixer audioMixer;
    public string exposedParameter;
    private void Start()
    {
        if (audioMixer)
        {
            float savedVolume = PlayerPrefs.GetFloat("SavedVolume", 1.0f);
            audioMixer.SetFloat(exposedParameter, Mathf.Log10(savedVolume) * 20);
            gameObject.GetComponentInChildren<Slider>().value = savedVolume;
        }
    }
    public void SetVolume(float volume)
    {
        float normalizedVolume = Mathf.Clamp01(volume / gameObject.GetComponentInChildren<Slider>().maxValue);
        if (audioMixer)
        {
            float volumeLevel = (normalizedVolume == 0) ? -80 : Mathf.Log10(normalizedVolume) * 20;
            audioMixer.SetFloat(exposedParameter, volumeLevel);
            PlayerPrefs.SetFloat("SavedVolume", normalizedVolume);
        }
    }
}