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
            Debug.LogError("No level prefabs assigned! please assign them in the inspector.");
            return;
        }

        InitializeLevel(0);
    }

    public void InitializeLevel(int levelIndex)
    {
        // Reset pangolin's firefly flag
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
        GameManager.Instance.currentLevelManager = this;

        AssignTilemaps();
        SpawnSoul();
        SetLevelMoves(levelIndex);
        movesManager.ResetMoves(movesLeft);

        // === Pangolin setup ===
        if (pangolin != null)
        {
            // Set position from level
            pangolin.SetStartPositionFromLevel(currentLevelInstance);

            // Rebind animator to prevent it from freezing
            if (pangolin.animator != null)
            {
                pangolin.animator.Rebind();
                pangolin.animator.Update(0f);
            }
        }

        // Initialize rocks
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

        DestroyAllDustFX(); // ensures all ongoing dust animations stop instantly

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
        //pangolin.ForceFacing(PangolinStartPoint.FacingDirection.Right);
   
        StartCoroutine(DelayedForceFacing(PangolinStartPoint.FacingDirection.Right));


        Debug.Log("Level " + (currentLevelIndex + 1) + " has been reset (in-place).");
    }



    public void MarkLevelCompleted()
    {
        levelCompleted = true;
        PlayerProgress.MarkLevelCompletedForever(currentLevelIndex);
    }


    private void CleanupOldLevel()
    {
        var leftovers = GameObject.FindGameObjectsWithTag("Level");
        Debug.Log("[LevelManager] CleanupOldLevel found " + leftovers.Length + " objects with tag 'Level'");
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

    public void OnFireflyCollected(int amount)
    {
        // Defensive checks
        if (FireflyManager.Instance == null)
        {
            Debug.LogWarning("FireflyManager missing. Skipping firefly add.");
            return;
        }

        // Only add to the global total if the level was NOT completed before (permanent completion)
        if (!PlayerProgress.WasLevelCompletedBefore(currentLevelIndex))
        {
            FireflyManager.Instance.AddFirefly(amount);
            Debug.Log("Firefly counted for level " + (currentLevelIndex + 1));
        }
        else
        {
            Debug.Log("Firefly ignored. Level already completed before.");
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
        var menu = FindFirstObjectByType<LevelMenuManager>();
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


    private IEnumerator DelayedForceFacing(PangolinStartPoint.FacingDirection facing)
    {
        yield return null; // wait one frame for Animator to fully bind
        pangolin.ForceFacing(facing);
    }


    public void LoadNextLevelWithDelay(float delay)
    {
        StartCoroutine(LoadNextLevelCoroutine(delay));
    }

    public int GetCurrentIndex()
    {
        return currentLevelIndex;
    }

    private void DestroyAllDustFX()
    {
        var dustObjects = GameObject.FindGameObjectsWithTag("DustFX");
        foreach (var obj in dustObjects)
            Destroy(obj);
    }



}