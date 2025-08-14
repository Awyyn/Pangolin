using UnityEngine;

public static class PlayerProgress
{
    private const string HighestLevelKey = "HighestLevel";

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
        return levelIndex <= GetHighestLevel();
    }

    public static void ResetProgress()
    {
        PlayerPrefs.DeleteKey(HighestLevelKey);
    }
}
