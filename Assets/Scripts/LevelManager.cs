using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{
    public GameObject CurrentLevelInstance => currentLevelInstance;
    public GameObject CurrentSoulInstance => currentSoulInstance;
    public void RespawnSoul() => SpawnSoul();

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
            Debug.LogError("No level prefabs assigned! Please assign them in the inspector.");
            return;
        }
    
        currentLevelIndex = PlayerProgress.GetLastPlayedLevel();
        FireflyCounterUI.Instance?.UpdateCount(PlayerProgress.GetFireflyCount());
        InitializeLevel(currentLevelIndex);

    }

    public void InitializeLevel(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= levelPrefabs.Length)
        {
            Debug.LogError("Invalid level index: " + levelIndex);
            return;
        }

        currentLevelIndex = levelIndex;
        levelCompleted = false; // reset completion status for the new level

        PlayerProgress.SetLastPlayedLevel(currentLevelIndex); // remember progress session
        PlayerProgress.MarkGameStarted();                     // ensures state B survives restart (in-progress run without 1. level completed)


        CleanupOldLevel();

        currentLevelInstance = Instantiate(levelPrefabs[levelIndex]);
        GameManager.Instance.currentLevelManager = this;
        GameManager.Instance.currentLevelIndex = levelIndex;
        // Update UI level number
        if (GameManager.Instance.levelNumberUI != null)
        {
            GameManager.Instance.levelNumberUI.SetLevelNumber(levelIndex);
        }


        AssignTilemaps();
        SpawnSoul();
        SetLevelMoves(levelIndex);
        movesManager.ResetMoves(movesLeft);

        // === Pangolin setup ===
        if (pangolin != null)
        {
            pangolin.SetStartPositionFromLevel(currentLevelInstance);
            if (pangolin.animator != null)
            {
                pangolin.animator.Rebind();
                pangolin.animator.Update(0f);
            }
        }

        foreach (Rock rock in FindObjectsByType<Rock>(FindObjectsSortMode.None))
        {
            rock.Initialize();
        }

        FindFirstObjectByType<CameraScroller>()?.ResetCamera();

        Debug.Log($"Finished initializing level {levelIndex + 1} with {movesLeft} moves.");
        Debug.Log("/////////////////////////////////////////////////////////////////////////////////////");
    }

    public void ResetLevel()
    {
        if (currentLevelInstance == null) return;

        DestroyAllDustFX();
        levelCompleted = false;
        pangolin.ResetLevelFlags();

        // Reset interactables
        var monoBehaviours = currentLevelInstance.GetComponentsInChildren<MonoBehaviour>(true);
        var interactables = monoBehaviours.OfType<IInteractable>();
        foreach (var it in interactables)
        {
            var mb = it as MonoBehaviour;
            if (mb != null) mb.StopAllCoroutines();
            it.ResetState();
        }

        var plates = currentLevelInstance.GetComponentsInChildren<PressurePlate>(true);
        foreach (var p in plates) p.ResetState();

        var doors = currentLevelInstance.GetComponentsInChildren<DoorController>(true);
        foreach (var d in doors) d.ResetState();

        if (currentSoulInstance != null)
        {
            Destroy(currentSoulInstance);
            currentSoulInstance = null;
        }
        SpawnSoul();

        SetLevelMoves(currentLevelIndex);
        movesManager.ResetMoves(movesLeft);

        FindFirstObjectByType<CameraScroller>()?.ResetCamera();

        pangolin.SetStartPositionFromLevel(currentLevelInstance);
        StartCoroutine(DelayedForceFacing(PangolinStartPoint.FacingDirection.Right));

        Debug.Log("Level " + (currentLevelIndex + 1) + " has been reset (in-place).");
    }

    private void CleanupOldLevel()
    {
        var leftovers = GameObject.FindGameObjectsWithTag("Level");
        foreach (var obj in leftovers)
        {
            if (obj.scene.IsValid())
            {
                Destroy(obj);
                Debug.Log("[LevelManager] Destroyed: " + obj.name);
            }
        }

        if (currentLevelInstance != null && currentLevelInstance.scene.IsValid())
        {
            Destroy(currentLevelInstance);
            currentLevelInstance = null;
            Debug.Log("[LevelManager] Destroyed currentLevelInstance");
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

        Transform spawnPoint = currentLevelInstance.transform.Find("Grid/SoulSpawnPoint");
        if (spawnPoint != null)
        {
            currentSoulInstance = Instantiate(soulPrefab, spawnPoint.position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("SoulSpawnPoint not found in level prefab " + currentLevelInstance.name);
        }
    }

    public void OnFireflyCollected()
    {
        // Check if this level was already completed before (persistently)
        if (!PlayerProgress.WasLevelCompletedBefore(currentLevelIndex))
        {
            // Increment total fireflies by 1
            int newTotal = PlayerProgress.GetFireflyCount() + 1;

            // Save persistently
            PlayerProgress.SetFireflyCount(newTotal);

            // Update UI
            FireflyCounterUI.Instance?.UpdateCount(newTotal);

            Debug.Log("[LevelManager] Firefly counted for level " + (currentLevelIndex + 1));
        }
        else
        {
            Debug.Log("[LevelManager] Level already completed, firefly ignored.");
        }
    }


    public void MarkLevelCompleted()
    {
        levelCompleted = true;
        PlayerProgress.MarkLevelCompletedForever(currentLevelIndex);
    }


    private IEnumerator LoadNextLevelCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        currentLevelIndex++;

        if (currentLevelIndex > PlayerProgress.GetHighestLevel())
        {
            PlayerProgress.SetHighestLevel(currentLevelIndex);
        }

        var menu = FindFirstObjectByType<LevelMenuManager>();
        if (menu != null) menu.PopulatePage(menu.CurrentPage);

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

    private IEnumerator DelayedForceFacing(PangolinStartPoint.FacingDirection facing)
    {
        yield return null;
        pangolin.ForceFacing(facing);
    }

    public void LoadNextLevelWithDelay(float delay)
    {
        StartCoroutine(LoadNextLevelCoroutine(delay));
    }

    public int GetCurrentIndex() => currentLevelIndex;

    private void DestroyAllDustFX()
    {
        var dustObjects = GameObject.FindGameObjectsWithTag("DustFX");
        foreach (var obj in dustObjects) Destroy(obj);
    }
}
