using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    public AudioMixer audioMixerMainMenu;
    public AudioMixer audioMixerInGame;
    public Slider overallVolumeSliderMainMenu;
    public Slider musicVolumeSliderMainMenu;
    public Slider sfxVolumeSliderMainMenu;
    public Slider overallVolumeSliderInGame;
    public string overallVolumeParameterMainMenu = "OverallVolume";
    public string musicVolumeParameterMainMenu = "MusicVolume";
    public string sfxVolumeParameterMainMenu = "SFXVolume";
    public string overallVolumeParameterInGame = "OverallVolume";

    private void Start()
    {
        SetAllSavedVolumes();
    }

    public void SetAllSavedVolumes()
    {
        SetSavedVolume(audioMixerMainMenu, overallVolumeParameterMainMenu, overallVolumeSliderMainMenu);
        SetSavedVolume(audioMixerMainMenu, musicVolumeParameterMainMenu, musicVolumeSliderMainMenu);
        SetSavedVolume(audioMixerMainMenu, sfxVolumeParameterMainMenu, sfxVolumeSliderMainMenu);
        SetSavedVolume(audioMixerInGame, overallVolumeParameterInGame, overallVolumeSliderInGame);
    }

    private void SetSavedVolume(AudioMixer audioMixer, string parameter, Slider slider)
    {
        if (audioMixer && slider)
        {
            float savedVolume = PlayerPrefs.GetFloat("SavedVolume_" + parameter, 1.0f);
            audioMixer.SetFloat(parameter, Mathf.Log10(savedVolume) * 20);
            slider.value = savedVolume;
        }
    }

    public void SetOverallVolumeMainMenu(float volume)
    {
        SetVolume(volume, audioMixerMainMenu, overallVolumeParameterMainMenu);
    }

    public void SetMusicVolumeMainMenu(float volume)
    {
        SetVolume(volume, audioMixerMainMenu, musicVolumeParameterMainMenu);
    }

    public void SetSFXVolume(float volume)
    {
        SetVolume(volume, audioMixerMainMenu, sfxVolumeParameterMainMenu);
    }

    public void SetOverallVolumeInGame(float volume)
    {
        SetVolume(volume, audioMixerInGame, overallVolumeParameterInGame);
    }

    private void SetVolume(float volume, AudioMixer audioMixer, string parameter)
    {
        if (audioMixer)
        {
            float normalizedVolume = Mathf.Clamp01(volume / overallVolumeSliderMainMenu.maxValue);
            float volumeLevel = (normalizedVolume == 0) ? -80 : Mathf.Log10(normalizedVolume) * 20;
            audioMixer.SetFloat(parameter, volumeLevel);
            PlayerPrefs.SetFloat("SavedVolume_" + parameter, normalizedVolume);
        }
    }
}
