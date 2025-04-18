using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SettingsMenu : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;

    Resolution[] resolutions;
    List<Resolution> selectedResolutionList = new List<Resolution>();

    bool isFullscreen;
    int selectedResolution;

    void Start()
    {
        resolutions = Screen.resolutions;
        List<string> resolutionStrings = new List<string>();

        string newRes;

        foreach (Resolution res in resolutions)
        {
            newRes = res.width + " x " + res.height;

            if (!resolutionStrings.Contains(newRes))
            {
                resolutionStrings.Add(newRes);
                selectedResolutionList.Add(res);
            }
        }

        if (resolutionDropdown != null)
        {
            resolutionDropdown.ClearOptions();
            resolutionDropdown.AddOptions(resolutionStrings);
        }

        selectedResolution = PlayerPrefs.GetInt("resolutionIndex", 0);
        isFullscreen = PlayerPrefs.GetInt("fullscreen", 1) == 1;
        
        resolutionDropdown.value = selectedResolution;
        resolutionDropdown.RefreshShownValue();

        fullscreenToggle.isOn = isFullscreen;

        ApplyResolution();
    }

    public void ChangeResolution()
    {
        selectedResolution = resolutionDropdown.value;
        ApplyResolution();

        PlayerPrefs.SetInt("resolutionIndex", selectedResolution);
        PlayerPrefs.Save();
    }

    public void ChangeFullscreen()
    {
        isFullscreen = fullscreenToggle.isOn;
        ApplyResolution();

        PlayerPrefs.SetInt("fullscreen", isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    void ApplyResolution()
    {
        Screen.SetResolution(
            selectedResolutionList[selectedResolution].width,
            selectedResolutionList[selectedResolution].height,
            isFullscreen
        );
    }
}
