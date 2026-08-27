using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenuController : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private void Start()
    {
        float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);

        masterSlider.SetValueWithoutNotify(masterVolume);
        musicSlider.SetValueWithoutNotify(musicVolume);
        sfxSlider.SetValueWithoutNotify(sfxVolume);

        SetMixerVolume("MasterVolume", masterVolume);
        SetMixerVolume("MusicVolume", musicVolume);
        SetMixerVolume("SFXVolume", sfxVolume);
    }

    public void SetMasterVolume(float volume)
    {
        SetMixerVolume("MasterVolume", volume);

        PlayerPrefs.SetFloat("MasterVolume", volume);
        PlayerPrefs.Save();
    }

    public void SetMusicVolume(float volume)
    {
        SetMixerVolume("MusicVolume", volume);

        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        SetMixerVolume("SFXVolume", volume);

        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
    }

    private void SetMixerVolume(string parameter, float volume)
    {
        float decibels;

        if (volume <= 0.0001f)
            decibels = -80f;
        else
            decibels = Mathf.Log10(volume) * 20f;

        audioMixer.SetFloat(parameter, decibels);
    }
}