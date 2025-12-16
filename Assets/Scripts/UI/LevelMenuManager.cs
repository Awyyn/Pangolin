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

    public int levelsPerPage = 8;
    private int currentPage = 0;
    private List<GameObject> spawnedButtons = new List<GameObject>();
    public int CurrentPage => currentPage; // expose current page for menu refresh


    void Start()
    {
        // Get the reference to LevelManager in your scene
        levelManager = FindFirstObjectByType<LevelManager>();

        // Get last played level from PlayerPrefs
        int lastPlayedLevel = PlayerProgress.GetLastPlayedLevel();
        currentPage = lastPlayedLevel / levelsPerPage; // make sure the page is correct

        // Populate the level menu on that page
        PopulatePage(currentPage);

        // Highlight/select last played level button (optional)
        // You could add a method in LevelButton to visually select it

        previousPageButton.onClick.AddListener(GoToPreviousPage);
        nextPageButton.onClick.AddListener(GoToNextPage);

        levelMenuPanel.SetActive(true); // show menu at start
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

        int totalLevels = levelManager.levelPrefabs.Length;

        // Calculate start and end levels for this page
        int startLevel = page * levelsPerPage;
        int endLevel = Mathf.Min(startLevel + levelsPerPage, totalLevels);

        for (int i = startLevel; i < endLevel; i++)
        {
            GameObject buttonObj = Instantiate(levelButtonPrefab, levelGrid);
            LevelButton levelButton = buttonObj.GetComponent<LevelButton>();


            bool isUnlocked = PlayerProgress.IsLevelUnlocked(i);
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

        if (index > PlayerProgress.GetHighestLevel())
        {
            PlayerProgress.SetHighestLevel(index);
        }

        levelManager.InitializeLevel(index);  // Restart the level

        ToggleMenu();  // Close the level menu
    }

}
