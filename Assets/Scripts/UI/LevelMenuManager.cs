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


    public Transform pageDotsParent;
    public GameObject pageDotPrefab;
    public Sprite activeDot;
    public Sprite inactiveDot;

    private List<Image> dots = new();



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
            currentPage = 0;              // always start on first page
            PopulatePage(currentPage);
        }

        //looks for pangolin and makes the player unable to move when menu is open 
        FindFirstObjectByType<PlayerMovement>()?.SetInputEnabled(!levelMenuPanel.activeSelf);

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

        UpdateArrowState();


    }

    void ClearButtons()
    {
        foreach (var button in spawnedButtons)
        {
            Destroy(button);
        }
        spawnedButtons.Clear();
    }


//important: clamping pages so we never go out of range
    void GoToNextPage()
    {
        int maxPage = Mathf.FloorToInt((levelManager.levelPrefabs.Length - 1) / (float)levelsPerPage);
        currentPage = Mathf.Min(currentPage + 1, maxPage);
        PopulatePage(currentPage);
    }

    void GoToPreviousPage()
    {
        currentPage = Mathf.Max(currentPage - 1, 0);
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

    void UpdateArrowState()
    {
        int totalLevels = levelManager.levelPrefabs.Length;
        int maxPage = Mathf.FloorToInt((totalLevels - 1) / (float)levelsPerPage);

        previousPageButton.interactable = currentPage > 0;
        nextPageButton.interactable = currentPage < maxPage;
    }

    void CreatePageDots()
    {
        foreach (Transform t in pageDotsParent)
            Destroy(t.gameObject);

        dots.Clear();

        int totalPages = Mathf.CeilToInt(levelManager.levelPrefabs.Length / (float)levelsPerPage);

        for (int i = 0; i < totalPages; i++)
        {
            var dotObj = Instantiate(pageDotPrefab, pageDotsParent);
            dots.Add(dotObj.GetComponent<Image>());
        }
    }
    void UpdateDots()
    {
        for (int i = 0; i < dots.Count; i++)
            dots[i].sprite = (i == currentPage) ? activeDot : inactiveDot;
    }


}
