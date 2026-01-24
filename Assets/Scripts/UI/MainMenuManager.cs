using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public Button continueButton;
    public GameObject confirmationPanel;
    public GameObject mainMenu;
    public ButtonManager buttonManager;

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
        confirmationPanel.SetActive(false);
        mainMenu.SetActive(false);
    }

    public void OpenOptions()
    {
        OptionsManager.Instance.OpenOptions();
    }
    public void CloseOptions()
    {
        Debug.Log("CLOSE CLICKED");
        OptionsManager.Instance.CloseOptions();
    }

    public void ConfirmNewGame()
    {
        PlayerPrefs.DeleteAll(); // resets all progress
        // set the main menu inactve and level menu active 

        mainMenu.SetActive(false);
    }


    public void NewGame()
    {
        confirmationPanel.SetActive(true);
    }

    public void CancelNewGame()
    {
        confirmationPanel.SetActive(false);
    }

}