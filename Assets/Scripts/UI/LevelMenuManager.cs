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
    public int CurrentPage => currentPage; // expose current page for menu refresh


    public Transform pageDotsParent;
    public GameObject pageDotPrefab;
    public Sprite activeDot;

    [Header("Page Dots")]
    private List<Image> dots = new();
    public Color activeDotColor = new Color(1f, 1f, 1f, 1f);   // 100%
    public Color inactiveDotColor = new Color(1f, 1f, 1f, 0.5f); // 50%



    void Start()
    {
        // Get the reference to LevelManager in your scene
        levelManager = FindFirstObjectByType<LevelManager>();

        // Get last played level from PlayerPrefs
        int lastPlayedLevel = PlayerProgress.GetLastPlayedLevel();
        currentPage = lastPlayedLevel / levelsPerPage; // make sure the page is correct

        // Populate the level menu on that page
        PopulatePage(currentPage);
        
        CreatePageDots();
        UpdateDots();

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
        PlayerMovement.instance?.SetInputEnabled(false); 

        if (!isActive)
        {
            PlayerMovement.instance?.SetInputEnabled(true);
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
        CreatePageDots();
        UpdateDots();


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
        GameSession.Instance.MarkRunStarted(); 

        if (index > PlayerProgress.GetHighestLevel())
            PlayerProgress.SetHighestLevel(index);

        levelManager.InitializeLevel(index);
        ToggleMenu();
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
        if (pageDotPrefab == null || pageDotsParent == null) return;

        foreach (Transform t in pageDotsParent)
            Destroy(t.gameObject);

        dots.Clear();

        int totalPages = Mathf.CeilToInt(levelManager.levelPrefabs.Length / (float)levelsPerPage);

        for (int i = 0; i < totalPages; i++)
        {
            GameObject dotObj = Instantiate(pageDotPrefab, pageDotsParent);
            Image img = dotObj.GetComponent<Image>();
            img.enabled = true;
            dots.Add(img);
        }
    }


    void UpdateDots()
    {
        for (int i = 0; i < dots.Count; i++)
        {
            dots[i].color = (i == currentPage)
                ? activeDotColor
                : inactiveDotColor;
        }
    }

    public void RefreshMenu()
    {
        currentPage = 0;
        PopulatePage(currentPage);
    }

}
