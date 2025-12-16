using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    [Header("Main Menu")]
    public GameObject mainMenuPanel;

    [Header("Level Menu")]
    public GameObject levelMenuPanel;
    public LevelMenuManager levelMenuManager;

    private void Start()
    {
        // Main menu should be visible when the game starts
        mainMenuPanel.SetActive(true);
    }

    public void ToggleMainMenu()
    {
        mainMenuPanel.SetActive(!mainMenuPanel.activeSelf);
    }

    public void ContinueGame()
    {
        // Hide main menu when continuing
        mainMenuPanel.SetActive(false);
    }

    public void ToggleOptionsMenu()
    {
        if (OptionsManager.Instance != null)
        {
            OptionsManager.Instance.ToggleOptions(); 
        }
    }

    public void ToggleLevelMenu()
    {
        bool isActive = levelMenuPanel.activeSelf;
        levelMenuPanel.SetActive(!isActive);

        if (!isActive)
        {
            levelMenuManager.PopulatePage(levelMenuManager.CurrentPage);
        }
    }
}
