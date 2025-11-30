using UnityEngine; 

public static class PlayerProgress
{
    private const string HighestLevelKey = "HighestLevel";
    private const string CompletedLevelsKeyPrefix = "CompletedLevel_";

    public static bool WasLevelCompletedBefore(int levelIndex)
        => PlayerPrefs.GetInt(CompletedLevelsKeyPrefix + levelIndex, 0) == 1;

    public static void MarkLevelCompletedForever(int levelIndex)
    {
        PlayerPrefs.SetInt(CompletedLevelsKeyPrefix + levelIndex, 1);
        PlayerPrefs.Save();
    }

    public static int GetHighestLevel()
        => PlayerPrefs.GetInt(HighestLevelKey, 0);

    public static void SetHighestLevel(int levelIndex)
    {
        if (levelIndex > GetHighestLevel())
        {
            PlayerPrefs.SetInt(HighestLevelKey, levelIndex);
            PlayerPrefs.Save();
        }
    }

    public static void ResetProgress(int totalLevels)
    {
        PlayerPrefs.DeleteKey(HighestLevelKey);

        for (int i = 0; i < totalLevels; i++)
            PlayerPrefs.DeleteKey(CompletedLevelsKeyPrefix + i);

        FireflyManager.Instance?.ResetFireflies(); // also reset fireflies
        PlayerPrefs.Save();

        Debug.Log("Progress fully reset!");
    }
    
    public static bool IsLevelUnlocked(int levelIndex)
    {
        if (levelIndex == 0) return true;
        return levelIndex <= GetHighestLevel();
    }

}
