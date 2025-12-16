using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject mainMenuPanel;
    public GameObject confirmationPanel;
<<<<<<< HEAD
=======
    public Button continueButton;
>>>>>>> 3b00293 (level menu page fix in progress)

    private void Start()
    {
        // Check if a game was ever started
        bool gameStarted = PlayerPrefs.GetInt("GameStarted", 0) == 1;

        // Enable/disable Continue button
        continueButton.interactable = gameStarted;

<<<<<<< HEAD
        confirmationPanel.SetActive(false);// start hidden
=======
        // Show main menu
        mainMenuPanel.SetActive(true);
        confirmationPanel.SetActive(false);

        // If game never started, optionally start the game immediately
        // We'll let NewGame() handle this on button click
>>>>>>> 3b00293 (level menu page fix in progress)
    }

    public void NewGame()
    {
        bool gameStarted = PlayerPrefs.GetInt("GameStarted", 0) == 1;
        bool firstLevelCompleted = PlayerPrefs.GetInt("HighestLevel", 0) > 0;

        if (!gameStarted)
        {
            // First time ever → start game immediately
            StartNewGame();
        }
        else if (!firstLevelCompleted)
        {
            // Already started but first level not completed → start without confirmation
            StartNewGame();
        }
        else
        {
            // Game in progress and first level finished → show confirmation
            confirmationPanel.SetActive(true);
        }
    }

    private void StartNewGame()
    {
        PlayerPrefs.SetInt("GameStarted", 1);
        PlayerPrefs.SetInt("HighestLevel", 0);
        PlayerPrefs.Save();

        mainMenuPanel.SetActive(false);
        confirmationPanel.SetActive(false);

        continueButton.interactable = true;

        // Let LevelManager handle starting level logic
        if (LevelManager.Instance != null)
            LevelManager.Instance.StartNewGame();
    }

    public void CancelNewGame()
    {
        confirmationPanel.SetActive(false);
    }
<<<<<<< HEAD
    public void OpenOptions()
    {
        OptionsManager.Instance.OpenOptions();
=======

    public void ContinueGame()
    {
        mainMenuPanel.SetActive(false);
>>>>>>> 3b00293 (level menu page fix in progress)
    }

    public void OpenOptions()
    {
<<<<<<< HEAD
        OptionsManager.Instance.CloseOptions();
    }

}
=======
        if (OptionsManager.Instance != null)
            OptionsManager.Instance.ToggleOptions();
    }
}
>>>>>>> 3b00293 (level menu page fix in progress)
