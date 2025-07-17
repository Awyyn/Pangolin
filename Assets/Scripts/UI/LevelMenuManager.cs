using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LevelMenuManager : MonoBehaviour
{
    public GameObject levelMenuPanel;
    public GameObject levelButtonPrefab;
    public Transform levelGrid;

    public Button previousPageButton;
    public Button nextPageButton;

    private LevelManager levelManager;  // Reference to the LevelManager

    public int levelsPerPage = 10;
    private int currentPage = 0;
    private List<GameObject> spawnedButtons = new List<GameObject>();

    void Start()
    {
        // Get the reference to LevelManager in your scene
        levelManager = FindObjectOfType<LevelManager>();

        // Populate the level menu with the first page of levels
        PopulatePage(currentPage);

        previousPageButton.onClick.AddListener(GoToPreviousPage);
        nextPageButton.onClick.AddListener(GoToNextPage);

        levelMenuPanel.SetActive(false); // hide at start
    }

    public void ToggleMenu()
    {
        bool isActive = levelMenuPanel.activeSelf;
        levelMenuPanel.SetActive(!isActive);

        if (!isActive)
        {
            PopulatePage(0);  // Show the first page when opening
        }
    }

    public void PopulatePage(int page)
    {
        // Clear the existing buttons
        ClearButtons();

        int totalLevels = levelManager.levels.Length;

        // Calculate start and end levels for this page
        int startLevel = page * levelsPerPage;
        int endLevel = Mathf.Min(startLevel + levelsPerPage, totalLevels);

        for (int i = startLevel; i < endLevel; i++)
        {
            GameObject buttonObj = Instantiate(levelButtonPrefab, levelGrid);
            LevelButton levelButton = buttonObj.GetComponent<LevelButton>();

            // Check if the level is unlocked based on PlayerPrefs
            bool isUnlocked = PlayerPrefs.GetInt($"Level_{i}_Completed", 0) == 1 || i == 0;
            levelButton.Setup(i, isUnlocked, OnLevelSelected);

            spawnedButtons.Add(buttonObj);
        }

        // Disable previous/next page buttons when on the first/last page
        previousPageButton.interactable = (page > 0);
        nextPageButton.interactable = (endLevel < totalLevels);
    }

    void ClearButtons()
    {
        foreach (var button in spawnedButtons)
        {
            Destroy(button);
        }
        spawnedButtons.Clear();
    }

    void GoToNextPage()
    {
        currentPage++;
        PopulatePage(currentPage);
    }

    void GoToPreviousPage()
    {
        currentPage--;
        PopulatePage(currentPage);
    }

    void OnLevelSelected(int index)
    {
        Debug.Log($"Selected level {index + 1}");

        // Mark the level as completed in PlayerPrefs
        PlayerPrefs.SetInt($"Level_{index}_Completed", 1);
        PlayerPrefs.Save();

        // If the level is selected mid-game, restart it
        levelManager.InitializeLevel(index);  // Restart the level (no need for LoadLevel)

        ToggleMenu();  // Close the level menu
    }
}
