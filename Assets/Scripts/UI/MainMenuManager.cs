using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public Button continueButton;
    public GameObject confirmationPanel;

    private void Start()
    {
        // Disable continue if no saved progress
        if (!PlayerPrefs.HasKey("HighestLevel"))
        {
            continueButton.interactable = false;
        }

        confirmationPanel.SetActive(false);
    }

    public void ContinueGame()
    {
        // Load the main game scene (the one that has your level menu UI inside it)
        SceneManager.LoadScene("GameScene");
    }

    public void ConfirmNewGame()
    {
        PlayerPrefs.DeleteAll(); // resets all progress
        SceneManager.LoadScene("GameScene");         // also load the GameScene, so player starts fresh
    }


    public void NewGame()
    {
        confirmationPanel.SetActive(true);
    }

    public void CancelNewGame()
    {
        confirmationPanel.SetActive(false);
    }

    public void OpenOptions()
    {
        // Trigger your existing options menu
        // Could be something like optionsPanel.SetActive(true)
    }
}