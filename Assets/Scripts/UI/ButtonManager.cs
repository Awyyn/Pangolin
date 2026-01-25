using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    public static ButtonManager Instance { get; private set; }

    public GameObject levelMenuPanel;  // The level menu panel to show/hide
    public LevelMenuManager levelMenuManager;  // Reference to LevelMenuManager to control paging

    public GameObject optionsPanel;
    public GameObject mainMenu;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
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

    public void LoadMainMenu()
    {
        OptionsManager.Instance.CloseOptions();
        mainMenu.SetActive(true);
    }



}
