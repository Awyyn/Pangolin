using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int currentLevelIndex = 0;
    public LevelManager currentLevelManager;
    public MovesManager movesManager;

    // Runtime flag
    public bool bossMode;

    // Check if the current level prefab name contains "Boss"
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
        Application.targetFrameRate = 60;
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

}
