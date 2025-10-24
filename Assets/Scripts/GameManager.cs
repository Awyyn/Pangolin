using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int currentLevelIndex = 0;
    public LevelManager currentLevelManager;
    public MovesManager movesManager;


    public bool bossMode = false;

    void Awake()
    {
        // Ensure only one instance exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // persists between scenes
    }

    private void Start()
    {
        Application.targetFrameRate = 60;                //BE MINDFUL OF THIS
    }

    public void RestartLevel()
    {
        currentLevelManager.ResetLevel();
    }



    // Optional: uncomment if you want GameManager to control progression
    /*
    public void LoadNextLevel()
    {
        currentLevel++;
        LevelManager.Instance.LoadNextLevelWithDelay(2f);
    }
    */
}
