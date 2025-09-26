using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public MovesManager movesManager;

    public GameObject[] levelPrefabs;
    public GameObject currentLevelInstance;
    public GameObject soulPrefab;
    private GameObject currentSoulInstance;

    public PlayerMovement pangolin; 



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
        PlayerMovement.instance.reachedFirefly = false;

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
        SpawnSoul();
        SetLevelMoves(levelIndex);
        movesManager.ResetMoves(movesLeft);

        if (pangolin != null)
        {
            pangolin.SetStartPositionFromLevel(currentLevelInstance);

        }


        Debug.Log("Initialized level " + (levelIndex + 1) + " with " + movesLeft + " moves.");
    }

    public void ResetLevel()
    {
        if (currentLevelInstance == null) return;

        levelCompleted = false;  // <<< Reset completion status for the new level
        pangolin.ResetLevelFlags();
        CleanupOldLevel();

        currentLevelInstance = Instantiate(levelPrefabs[currentLevelIndex]);
        AssignTilemaps();
        SpawnSoul();

        SetLevelMoves(currentLevelIndex);
        movesManager.ResetMoves(movesLeft);

        pangolin.SetStartPositionFromLevel(currentLevelInstance);
        pangolin.ForceFacing(PangolinStartPoint.FacingDirection.Right);



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
        {
            if (obj.scene.IsValid()) // only destroy scene objects
                Destroy(obj);
        }

        if (currentLevelInstance != null && currentLevelInstance.scene.IsValid())
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
    private void SpawnSoul()
    {
        // despawn old soul
        if (currentSoulInstance != null)
        {
            Destroy(currentSoulInstance);
            currentSoulInstance = null;
        }

        if (soulPrefab == null)
        {
            Debug.LogError("Soul prefab not assigned in LevelManager!");
            return;
        }

        Transform spawnPoint = currentLevelInstance.transform
            .Find("Grid/SoulSpawnPoint");

        if (spawnPoint != null)
        {
            currentSoulInstance = Instantiate(soulPrefab, spawnPoint.position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("SoulSpawnPoint not found in level prefab " + currentLevelInstance.name);
        }
    }

    private IEnumerator LoadNextLevelCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        currentLevelIndex++; // move to next level first

        // Unlock the next level in the menu
        if (currentLevelIndex > PlayerProgress.GetHighestLevel())
        {
            PlayerProgress.SetHighestLevel(currentLevelIndex); // unlock next level
        }

        // Refresh the menu so unlocked levels show up
        var menu = FindObjectOfType<LevelMenuManager>();
        if (menu != null)
            menu.PopulatePage(menu.CurrentPage); // refresh the currently visible page

        if (currentLevelIndex < levelPrefabs.Length)
        {
            InitializeLevel(currentLevelIndex); // start next level
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