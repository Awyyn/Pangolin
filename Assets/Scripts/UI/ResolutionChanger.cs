using UnityEngine;
using TMPro;

public class ResolutionChanger : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown; // Reference to the dropdown UI
    private Resolution[] resolutions;

    void Start()
    {
        resolutions = Screen.resolutions;  // Get all available resolutions

        // Populate the dropdown with available resolutions
        PopulateDropdown();

        // Set the default resolution to Full HD
        resolutionDropdown.value = GetIndexForResolution(1920, 1080);
        resolutionDropdown.onValueChanged.AddListener(OnResolutionChange);
    }

    // Populate the dropdown with predefined resolutions
    void PopulateDropdown()
    {
        resolutionDropdown.ClearOptions();  // Clear any existing options
        var options = new System.Collections.Generic.List<TMP_Dropdown.OptionData>();

        // Add some resolutions to the dropdown manually (or dynamically)
        options.Add(new TMP_Dropdown.OptionData("1920x1080"));
        options.Add(new TMP_Dropdown.OptionData("1600x900"));
        options.Add(new TMP_Dropdown.OptionData("1280x720"));
        options.Add(new TMP_Dropdown.OptionData("1024x768"));

        resolutionDropdown.AddOptions(options); // Add them to the dropdown list
    }

    // Get the index of a resolution in the available list (for the selected resolution)
    int GetIndexForResolution(int width, int height)
    {
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == width && resolutions[i].height == height)
            {
                return i; // Return the index of the matching resolution
            }
        }
        return 0;  // Default index if no match
    }

    // Called when a new resolution is selected in the dropdown
    void OnResolutionChange(int index)
    {
        // Get the selected resolution from the dropdown list
        string selectedResolution = resolutionDropdown.options[index].text;
        string[] dimensions = selectedResolution.Split('x');
        int width = int.Parse(dimensions[0]);
        int height = int.Parse(dimensions[1]);

        // Apply the new resolution to the screen
        Screen.SetResolution(width, height, Screen.fullScreen);

        // Log the new resolution to confirm it's being set correctly
        Debug.Log("Resolution set to: " + width + "x" + height);
    }
}
