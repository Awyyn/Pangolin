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
        CloseOptions();
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
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        PlayerProgress.MarkGameStarted();   // marks State B (fresh run in progress)

        mainMenu.SetActive(false);
        ButtonManager.Instance.levelMenuManager.RefreshMenu();
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