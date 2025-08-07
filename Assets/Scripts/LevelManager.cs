using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public MovesManager movesManager;

    public GameObject[] levelPrefabs;
    private GameObject currentLevelInstance;
    public bool levelCompleted { get; private set; } = false;

    public float transitionDelay = 2f;



    public int movesLeft { get; private set; }
    private int currentLevelIndex = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (levelPrefabs.Length == 0)
        {
            Debug.LogError("No level prefabs assigned! please assign them in the inspector.");
            return;
        }

        InitializeLevel(0);
    }

    public void InitializeLevel(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= levelPrefabs.Length)
        {
            Debug.LogError("invalid level index: " + levelIndex);
            return;
        }

        currentLevelIndex = levelIndex;
        levelCompleted = false; // reset completion status for the new level

        CleanupOldLevel();

        currentLevelInstance = Instantiate(levelPrefabs[levelIndex]);

        AssignTilemaps();

        SetLevelMoves(levelIndex);
        movesManager.ResetMoves(movesLeft);

        ResetPangolinPosition();

        Debug.Log("Initialized level " + (levelIndex + 1) + " with " + movesLeft + " moves.");
    }

    public void ResetLevel()
    {
        if (currentLevelInstance == null) return;

        levelCompleted = false;  // <<< Reset completion status for the new level
        CleanupOldLevel();

        currentLevelInstance = Instantiate(levelPrefabs[currentLevelIndex]);

        AssignTilemaps();
        SetLevelMoves(currentLevelIndex);
        movesManager.ResetMoves(movesLeft);

        ResetPangolinPosition();

        Debug.Log("Level " + (currentLevelIndex + 1) + " has been reset.");
    }

    public void MarkLevelCompleted()
    {
        levelCompleted = true;
    }


    private void CleanupOldLevel()
    {
        var leftovers = GameObject.FindGameObjectsWithTag("Level");
        foreach (var obj in leftovers)
            Destroy(obj);

        if (currentLevelInstance != null)
        {
            Destroy(currentLevelInstance);
            currentLevelInstance = null;
        }
    }

    public void SetLevelMoves(int levelIndex)
    {
        if (currentLevelInstance == null) return;

        var data = currentLevelInstance.GetComponent<LevelData>();
        if (data != null)
        {
            movesLeft = data.allowedMoves;
        }
        else
        {
            Debug.LogError("LevelData component missing on level " + currentLevelInstance.name);
            movesLeft = 0;
        }
    }

    private void AssignTilemaps()
    {
        var gridTransform = currentLevelInstance.transform.Find("Grid");

        if (gridTransform == null)
        {
            Debug.LogError("Grid object not found in level prefab");
            return;
        }

        var newGround = gridTransform.Find("GroundTilemap")?.GetComponent<Tilemap>();
        var newObstacle = gridTransform.Find("ObstacleTilemap")?.GetComponent<Tilemap>();

        GridManager.Instance.groundTilemap = newGround;
        GridManager.Instance.obstacleTilemap = newObstacle;

        if (newGround == null) Debug.LogError("Ground tilemap not found!");
        if (newObstacle == null) Debug.LogError("Obstacle tilemap not found!");
    }

    private void ResetPangolinPosition()
    {
        var player = FindObjectOfType<PlayerMovement>();
        if (player != null)
        {
            player.SetStartPositionFromLevel();
        }
    }

    private IEnumerator LoadNextLevelCoroutine(float delay)
    {

        yield return new WaitForSeconds(delay);

        // save progress
        if (currentLevelIndex > PlayerProgress.GetHighestLevel())
        {
            PlayerProgress.SetHighestLevel(currentLevelIndex);

            var menu = FindObjectOfType<LevelMenuManager>();
            if (menu != null)
                menu.PopulatePage(0);
        }

        currentLevelIndex++;

        if (currentLevelIndex < levelPrefabs.Length)
        {
            InitializeLevel(currentLevelIndex);
            Debug.Log("Level " + (currentLevelIndex + 1) + " loaded.");
        }
        else
        {
            Debug.Log("No more levels! Game complete.");
        }
    }

    public void LoadNextLevelWithDelay(float delay)
    {
        StartCoroutine(LoadNextLevelCoroutine(delay));
    }

    public int GetCurrentIndex()
    {
        return currentLevelIndex;
    }
}
