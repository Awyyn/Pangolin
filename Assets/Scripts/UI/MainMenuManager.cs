using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public Button continueButton;
    public GameObject confirmationPanel;
    public GameObject mainMenu;
    public ButtonManager buttonManager;


    private void OnEnable()          // Called every time menu becomes active
    {
        RefreshContinueButton();
    }

    private void Start()             // Called once when scene first loads
    {
        Application.targetFrameRate = 60;
        confirmationPanel.SetActive(false);

        RefreshContinueButton();
    }

    private void RefreshContinueButton()
    {
        bool hasStarted = PlayerProgress.HasStartedGame();
        continueButton.interactable = hasStarted;
    }


    public void ContinueGame()
    {
        confirmationPanel.SetActive(false);
        mainMenu.SetActive(false);

        // Start the game through GameManager
        GameManager.Instance.StartGame();
    }

    public void OpenOptions()
    {
        OptionsManager.Instance.OpenOptions();
    }
    public void CloseOptions()
    {
        OptionsManager.Instance.CloseOptions();
    }

    public void ConfirmNewGame()
    {
        confirmationPanel.SetActive(false);
        StartFreshGame();   // reuse the same reset logic
    }

    public void NewGame()
    {
        bool finishedFirstLevel = PlayerProgress.WasLevelCompletedBefore(0);

        if (finishedFirstLevel)
            confirmationPanel.SetActive(true); // State C → show warning (at least first level completed)
        else
            StartFreshGame();             // State A or B → start immediately. (no progress to lose)
    }

    private void StartFreshGame()
    {
        // Reset GameManager flag so the cutscene can play
        GameManager.Instance.ResetGameStartedFlag();
        
        //SHOW curtain immediately
        GameManager.Instance.Curtain.SetActive(true);
    
        // Reset all gameplay progress
        PlayerProgress.ResetProgress();
        PlayerProgress.ResetFireflies();

        // Reset intro so it will play
        PlayerProgress.ResetIntro();

        // Mark game as started
        PlayerProgress.MarkGameStarted();

        // Update firefly counter (this is fine to keep)
        int fireflies = PlayerProgress.GetFireflyCount(
            LevelManager.Instance != null
                ? LevelManager.Instance.levelPrefabs.Length
                : 100
        );
        FireflyCounterUI.Instance?.UpdateCount(fireflies);

        // Hide main menu FIRST
        mainMenu.SetActive(false);

        // Start the game (cutscene OR menu handled inside GameManager)
        GameManager.Instance.StartGame();
    }


    public void CancelNewGame()
    {
        confirmationPanel.SetActive(false);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // stops play mode in editor
        #else
        Application.Quit(); // quits the build
        #endif
    }

}