using UnityEngine;

public static class PlayerProgress
{
    // Keys for PlayerPrefs
    private const string FireflyTotalKey = "FireflyTotal";
    private const string HighestLevelKey = "HighestLevel";
    private const string CompletedLevelKeyPrefix = "CompletedLevel_";
    private const string LastPlayedLevelKey = "LastPlayedLevel";

    private const string FireflyKey = "FireflyCount";

    private const string HasStartedGameKey = "HasStartedGame";
    
    // ----------- Game Started -----------
    public static void MarkGameStarted()
    {
        PlayerPrefs.SetInt(HasStartedGameKey, 1);
        PlayerPrefs.Save();
    }

    public static bool HasStartedGame()
    {
        return PlayerPrefs.GetInt(HasStartedGameKey, 0) == 1;
    }

        // ----------- Fireflies -----------
    public static void SetFireflyCount(int count)
    {
        PlayerPrefs.SetInt(FireflyKey, count);
        PlayerPrefs.Save();
    }

    public static int GetFireflyCount()
    {
        return PlayerPrefs.GetInt(FireflyKey, 0); // 0 if not saved yet
    }

    public static void ResetFireflies()
    {
        SetFireflyCount(0);
    }

    public static int GetFireflyTotal()
    {
        return PlayerPrefs.GetInt(FireflyTotalKey, 0);
    }

    public static void SetFireflyTotal(int total)
    {
        PlayerPrefs.SetInt(FireflyTotalKey, total);
        PlayerPrefs.Save();
    }

    public static void AddFireflies(int amount)
    {
        SetFireflyTotal(GetFireflyTotal() + amount);
    }

    public static bool SpendFireflies(int amount)
    {
        int total = GetFireflyTotal();
        if (total < amount) return false;
        SetFireflyTotal(total - amount);
        return true;
    }

    // ----------- Levels -----------
    public static bool WasLevelCompletedBefore(int levelIndex)
    {
        return PlayerPrefs.GetInt(CompletedLevelKeyPrefix + levelIndex, 0) == 1;
    }

    public static void MarkLevelCompletedForever(int levelIndex)
    {
        PlayerPrefs.SetInt(CompletedLevelKeyPrefix + levelIndex, 1);
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

    // ----------- Last Played Level -----------
    public static void SetLastPlayedLevel(int levelIndex)
    {
        PlayerPrefs.SetInt(LastPlayedLevelKey, levelIndex);
        PlayerPrefs.Save();
    }

    public static int GetLastPlayedLevel()
    {
        return PlayerPrefs.GetInt(LastPlayedLevelKey, 0);
    }

    // ----------- Reset Progress -----------
    public static void ResetProgress(int totalLevels)
    {
        PlayerPrefs.DeleteKey(FireflyTotalKey);
        PlayerPrefs.DeleteKey(HighestLevelKey);
        PlayerPrefs.DeleteKey(LastPlayedLevelKey);

        for (int i = 0; i < totalLevels; i++)
        {
            PlayerPrefs.DeleteKey(CompletedLevelKeyPrefix + i);
        }

        PlayerPrefs.Save();
        Debug.Log("Progress reset!");
    }
}
