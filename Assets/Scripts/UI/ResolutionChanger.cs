using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ResolutionChanger : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;   // Resolution list
    public TMP_Dropdown fullscreenDropdown;   // Fullscreen / Windowed

    private Resolution[] resolutions;
    
    void Start()
    {
        resolutions = Screen.resolutions;

        PopulateResolutionDropdown();
        PopulateFullscreenDropdown();

        // --- LOAD SAVED SETTINGS ---

        int savedResIndex = PlayerPrefs.GetInt("ResolutionIndex", 0);
        resolutionDropdown.value = savedResIndex;
        resolutionDropdown.RefreshShownValue();
        ApplyResolution(savedResIndex);

        int savedFullscreen = PlayerPrefs.GetInt("Fullscreen", 1);
        fullscreenDropdown.value = savedFullscreen == 1 ? 0 : 1;
        fullscreenDropdown.RefreshShownValue();
        ApplyFullscreen(fullscreenDropdown.value);

        // Add listeners AFTER applying saved values
        resolutionDropdown.onValueChanged.AddListener(ApplyResolution);
        fullscreenDropdown.onValueChanged.AddListener(ApplyFullscreen);
    }

    void PopulateResolutionDropdown()
    {
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();

        foreach (var res in resolutions)
        {
            string option = res.width + "x" + res.height;
            if (!options.Contains(option)) // avoid duplicates
                options.Add(option);
        }

        resolutionDropdown.AddOptions(options);
    }

    void PopulateFullscreenDropdown()
    {
        fullscreenDropdown.ClearOptions();
        fullscreenDropdown.AddOptions(new List<string> { "Fullscreen", "Windowed" });
    }

    void ApplyResolution(int index)
    {
        if (index < 0 || index >= resolutions.Length) index = 0;

        Resolution res = resolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);

        PlayerPrefs.SetInt("ResolutionIndex", index);
        PlayerPrefs.Save();
    }

    void ApplyFullscreen(int index)
    {
        bool fullscreen = (index == 0);
        Screen.fullScreen = fullscreen;

        PlayerPrefs.SetInt("Fullscreen", fullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }
}
