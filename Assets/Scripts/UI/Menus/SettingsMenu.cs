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
        isFullscreen = true;
        resolutions = Screen.resolutions;

        List<string> resolutionStrings = new List<string>();

        string newRes;

        foreach (Resolution res in resolutions)
        {
            newRes = res.width.ToString() + " x " + res.height.ToString();

            if (!resolutionStrings.Contains(newRes))
            {
                resolutionStrings.Add(newRes);
                selectedResolutionList.Add(res);
            }
        }

        if (resolutionDropdown != null && resolutionStrings != null)
        {
            resolutionDropdown.AddOptions(resolutionStrings);
        }
    }

    public void ChangeResolution()
    {
        selectedResolution = resolutionDropdown.value;
        Screen.SetResolution(selectedResolutionList[selectedResolution].width, selectedResolutionList[selectedResolution].height, isFullscreen);
    }

    public void ChangeFullscreen()
    {
        isFullscreen = fullscreenToggle.isOn;
        Screen.SetResolution(selectedResolutionList[selectedResolution].width, selectedResolutionList[selectedResolution].height, isFullscreen);
    }
}
