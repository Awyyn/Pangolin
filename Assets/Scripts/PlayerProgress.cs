using UnityEngine;

public static class PlayerProgress
{
    private const string HighestLevelKey = "HighestLevel";
    private const string CompletedLevelsKey = "CompletedLevel_";
    public static bool WasLevelCompletedBefore(int levelIndex)
    {
        return PlayerPrefs.GetInt(CompletedLevelsKey + levelIndex, 0) == 1;
    }

    public static void MarkLevelCompletedForever(int levelIndex)
    {
        PlayerPrefs.SetInt(CompletedLevelsKey + levelIndex, 1);
        PlayerPrefs.Save();
    }

    public static int GetHighestLevel()
    {
        return PlayerPrefs.GetInt(HighestLevelKey, 0);
    }

    public static void SetHighestLevel(int levelIndex)
    {
        if (levelIndex > GetHighestLevel())
        {
            PlayerPrefs.SetInt(HighestLevelKey, levelIndex);
            PlayerPrefs.Save();
        }
    }

    public static bool IsLevelUnlocked(int levelIndex)
    {
        // Level 0 is always unlocked
        if (levelIndex == 0) return true;
        return levelIndex <= GetHighestLevel();
    }


    public static void ResetProgress()
    {
        PlayerPrefs.DeleteKey(HighestLevelKey);
    }
}
