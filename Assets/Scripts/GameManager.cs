using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int currentLevelIndex = 0;
    public LevelManager currentLevelManager;
    public MovesManager movesManager;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
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
