using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public Button continueButton;
    public GameObject confirmationPanel;

    private void Start()
    {
        Application.targetFrameRate = 60;                                             //BE MINDFUL OF THIS (this is a dublicate, not sure if needed)

        // Disable continue if no saved progress
        if (!PlayerPrefs.HasKey("HighestLevel"))
        {
            continueButton.interactable = false;
        }

        confirmationPanel.SetActive(false);// start hidden
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
        OptionsManager.Instance.OpenOptions();
    }

    public void CloseOptions()
    {
        OptionsManager.Instance.CloseOptions();
    }

}