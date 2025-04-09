using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider mainSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider SFXSlider;

    public void Start()
    {
        if (PlayerPrefs.HasKey("mainVolume"))
        {
            LoadMainVolume();
        }
        else
        {
            SetMainVolume();
        }

        if (PlayerPrefs.HasKey("musicVolume"))
        {
            LoadMusicVolume();
        }
        else
        {
            SetMusicVolume();
        }

        if (PlayerPrefs.HasKey("sfxVolume"))
        {
            LoadSFXVolume();
        }
        else
        {
            SetSFXVolume();
        }
    }

    public void SetMainVolume()
    {
        float volume = Mathf.Clamp(mainSlider.value, 0.0001f, 1f);
        audioMixer.SetFloat("main", Mathf.Log10(volume)*20);
        PlayerPrefs.SetFloat("mainVolume", volume);
    }

    public void SetMusicVolume()
    {
        float volume = Mathf.Clamp(musicSlider.value, 0.0001f, 1f);
        audioMixer.SetFloat("music", Mathf.Log10(volume)*20);
        PlayerPrefs.SetFloat("musicVolume", volume);
    }

    public void SetSFXVolume()
    {
        float volume = Mathf.Clamp(SFXSlider.value, 0.0001f, 1f);
        audioMixer.SetFloat("sfx", Mathf.Log10(volume)*20);
        PlayerPrefs.SetFloat("sfxVolume", volume);
    }

    private void LoadMainVolume()
    {
        mainSlider.value = PlayerPrefs.GetFloat("mainVolume");
        SetMainVolume();
    }

    private void LoadMusicVolume()
    {
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        SetMusicVolume();
    }

    private void LoadSFXVolume()
    {
        SFXSlider.value = PlayerPrefs.GetFloat("sfxVolume");
        SetSFXVolume();
    }
}
