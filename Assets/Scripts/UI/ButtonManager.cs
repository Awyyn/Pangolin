using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    public GameObject optionsMenu;
    public GameObject levelMenuPanel;  // The level menu panel to show/hide
    public LevelMenuManager levelMenuManager;  // Reference to LevelMenuManager to control paging


    public void ToggleOptionsMenu()
    {
        // Toggle the active state of the options menu
        optionsMenu.SetActive(!optionsMenu.activeSelf);
    }

    public void ToggleLevelMenu()
    {
        // Toggle the visibility of the level menu
        bool isActive = levelMenuPanel.activeSelf;
        levelMenuPanel.SetActive(!isActive);

        // If the menu is being shown, ensure the first page is populated
        if (!isActive)
        {
            levelMenuManager.PopulatePage(0);  // Show the first page
        }
    }
}
