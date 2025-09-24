using UnityEngine;
using TMPro;

public class ResolutionChanger : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown; // Dropdown UI for resolutions
    public TMP_Dropdown fullscreenDropdown; // Dropdown UI for fullscreen/windowed
    private Resolution[] resolutions;

    void Start()
    {
        resolutions = Screen.resolutions;

        PopulateResolutionDropdown();
        PopulateFullscreenDropdown();

        // Load saved resolution index or fallback to current resolution
        int savedResIndex = PlayerPrefs.GetInt("ResolutionIndex", GetIndexForResolution(Screen.currentResolution.width, Screen.currentResolution.height));
        resolutionDropdown.value = savedResIndex;
        resolutionDropdown.RefreshShownValue();
        OnResolutionChange(savedResIndex); // apply it

        // Load saved fullscreen state (default = fullscreen)
        int savedFullscreen = PlayerPrefs.GetInt("Fullscreen", 1);
        fullscreenDropdown.value = savedFullscreen == 1 ? 0 : 1;
        fullscreenDropdown.RefreshShownValue();
        OnFullscreenChange(fullscreenDropdown.value); // apply it

        // Add listeners after applying saved settings
        resolutionDropdown.onValueChanged.AddListener(OnResolutionChange);
        fullscreenDropdown.onValueChanged.AddListener(OnFullscreenChange);
    }


    void PopulateResolutionDropdown()
    {
        resolutionDropdown.ClearOptions();
        var options = new System.Collections.Generic.List<string>();

        foreach (var res in resolutions)
        {
            string option = res.width + "x" + res.height;
            if (!options.Contains(option)) // Avoid duplicates
                options.Add(option);
        }

        resolutionDropdown.AddOptions(options);
    }

    void PopulateFullscreenDropdown()
    {
        fullscreenDropdown.ClearOptions();
        fullscreenDropdown.AddOptions(new System.Collections.Generic.List<string> { "Fullscreen", "Windowed" });
    }

    int GetIndexForResolution(int width, int height)
    {

        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == width && resolutions[i].height == height)
            {
                return i;
            }
        }
        return 0; // fallback
    }

    void OnResolutionChange(int index)
    {
        if (resolutions == null || resolutions.Length == 0)
        {
            Debug.LogWarning("No resolutions available");
            return;
        }

        if (index < 0 || index >= resolutions.Length)
        {
            Debug.LogWarning($"Resolution index {index} out of range. Resetting to 0.");
            index = 0;
        }

        Resolution res = resolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);

        PlayerPrefs.SetInt("ResolutionIndex", index);
        PlayerPrefs.Save();

        Debug.Log("Resolution set to: " + res.width + "x" + res.height);
    }


    void OnFullscreenChange(int index)
    {
        bool fullscreen = (index == 0); // 0 = Fullscreen, 1 = Windowed
        Screen.fullScreen = fullscreen;

        // Save fullscreen state
        PlayerPrefs.SetInt("Fullscreen", fullscreen ? 1 : 0);
        PlayerPrefs.Save();

        Debug.Log("Fullscreen: " + fullscreen);
    }

}
