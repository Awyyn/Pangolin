using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int currentLevelIndex = 0;
    public LevelManager currentLevelManager;
    public MovesManager movesManager;
    public LevelNumberUI levelNumberUI; 

    [SerializeField] private GameObject introCutscene;
    public GameObject IntroCutscene => introCutscene; // public getter

    private bool gameStarted = false;
    [SerializeField] private GameObject curtain;
    public GameObject Curtain => curtain;


    // Runtime flag
    public bool bossMode;

    public bool isBossLevel => currentLevelManager.currentLevelInstance != null &&
                               currentLevelManager.currentLevelInstance.name.Contains("Boss");

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 60;
    }

    /// <summary>
    /// Called from MainMenuManager after player clicks Continue/New Game
    /// </summary>
    public void StartGame()
    {
        if (gameStarted) return;
        gameStarted = true;

        // Keep curtain ON while deciding what to show
        if (curtain != null)
            curtain.SetActive(true);

        if (!PlayerProgress.HasSeenIntro() && introCutscene != null)
        {
            introCutscene.SetActive(true);
        }
        else
        {
            ShowLevelMenu();
            HideCurtain();
        }
    }

    /// <summary>
    /// Call this when the intro video finishes
    /// </summary>
    public void OnIntroFinished()
    {
        PlayerProgress.MarkIntroPlayed();

        if (introCutscene != null)
            introCutscene.SetActive(false);

        ShowLevelMenu();

        HideCurtain();
    }

    public void ShowLevelMenu()
    {
        ButtonManager.Instance.levelMenuManager.RefreshMenu();
    }
    private void HideCurtain()
    {
        if (curtain != null)
            curtain.SetActive(false);
    }

    public void RestartLevel()
    {
        currentLevelManager.ResetLevel();
    }

    public void StartBossFight()
    {
        if (!isBossLevel) return;
        var bossController = FindFirstObjectByType<BossFightController>();
        if (bossController != null)
            bossController.TriggerBossFight();
    }
    
    public void ResetGameStartedFlag()
    {
        gameStarted = false;
    }
}