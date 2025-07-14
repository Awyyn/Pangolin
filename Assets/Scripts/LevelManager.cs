using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    //Level Setup
    public GameObject[] levels;              // Each level is a GameObject with its own Grid
    public float transitionDelay = 2f;       // Time before switching levels

    //References
    public MovesManager movesManager;        // Reference to MovesManager

    public int movesLeft { get; private set; }
    private int currentLevelIndex = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        LevelManager.Instance.InitializeLevel(0);
    }

    public void InitializeLevel(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= levels.Length)
        {
            Debug.LogError("Invalid level index: " + levelIndex);
            return;
        }

        // Deactivate all levels
        foreach (GameObject level in levels)
            level.SetActive(false);

        // Activate selected level
        levels[levelIndex].SetActive(true);
        currentLevelIndex = levelIndex;

        // Assign tilemaps from the current level
        AssignTilemaps(levelIndex);

        // Set and reset moves
        SetLevelMoves(levelIndex);
        ResetLevel();

        Debug.Log("Initialized level " + (levelIndex + 1) + " with " + movesLeft + " moves.");

    }

    public void ResetLevel()
    {
        SetLevelMoves(currentLevelIndex);
        movesManager.ResetMoves(movesLeft);
        Debug.Log("Level " + (currentLevelIndex + 1) + " has been reset with " + movesLeft + " moves left.");

    }

    public void SetLevelMoves(int levelIndex)
    {
        GameObject level = levels[levelIndex];
        LevelData data = level.GetComponent<LevelData>();

        if (data != null)
        {
            movesLeft = data.allowedMoves;
        }
        else
        {
            Debug.LogError("LevelData component missing on level " + level.name);
            movesLeft = 0;
        }
    }


    private void AssignTilemaps(int levelIndex)
    {
        Transform gridTransform = levels[levelIndex].transform.Find("Grid");

        if (gridTransform == null)
        {
            Debug.LogError("Grid object NOT FOUND under level " + levelIndex);
            return;
        }

        var newGround = gridTransform.Find("GroundTilemap")?.GetComponent<Tilemap>();
        var newObstacle = gridTransform.Find("ObstacleTilemap")?.GetComponent<Tilemap>();

        GridManager.Instance.groundTilemap = newGround;
        GridManager.Instance.obstacleTilemap = newObstacle;

        if (newGround == null) Debug.LogError("Ground tilemap NOT FOUND!");
        if (newObstacle == null) Debug.LogError("Obstacle tilemap NOT FOUND!");
    }

    public void LoadNextLevelWithDelay(float delay)
    {
        StartCoroutine(LoadNextLevelCoroutine(delay));
    }

    private IEnumerator LoadNextLevelCoroutine(float delay)
    {
        Debug.Log($"Next level will load in {delay} seconds...");
        yield return new WaitForSeconds(delay);

        // Disable current level
        if (currentLevelIndex < levels.Length)
            levels[currentLevelIndex].SetActive(false);

        // Advance index
        currentLevelIndex++;

        // Load next if available
        if (currentLevelIndex < levels.Length)
        {
            InitializeLevel(currentLevelIndex);
            Debug.Log("Level " + (currentLevelIndex + 1) + " loaded.");

        }
        else
        {
            Debug.Log("No more levels! Game complete.");
            // TODO: trigger game-end screen
        }
    }
}
