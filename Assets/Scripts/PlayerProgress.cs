using UnityEngine;

public static class PlayerProgress
{
    // Keys for PlayerPrefs
    private const string FireflyTotalKey = "FireflyTotal";
    private const string HighestLevelKey = "HighestLevel";
    private const string CompletedLevelKeyPrefix = "CompletedLevel_";
    private const string LastPlayedLevelKey = "LastPlayedLevel";
    private const string HasStartedGameKey = "HasStartedGame";
    private const string IntroPlayedKey = "IntroPlayed"; // <--- added

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

    // ----------- Cutscene -----------
    public static bool HasSeenIntro()
    {
        return PlayerPrefs.GetInt(IntroPlayedKey, 0) == 1;
    }

    public static void MarkIntroPlayed()
    {
        PlayerPrefs.SetInt(IntroPlayedKey, 1);
        PlayerPrefs.Save();
    }

    public static void ResetIntro()
    {
        PlayerPrefs.DeleteKey(IntroPlayedKey);
    }

    // ----------- Fireflies -----------
    public static int GetFireflyCount(int totalLevels)
    {
        int count = 0;

        for (int i = 0; i < totalLevels; i++)
        {
            if (WasLevelCompletedBefore(i))
                count++;
        }

        return count;
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
    public static void ResetProgress()
    {
        PlayerPrefs.DeleteKey(HighestLevelKey);
        PlayerPrefs.DeleteKey(LastPlayedLevelKey);
        PlayerPrefs.DeleteKey(HasStartedGameKey);

        // Remove completed level flags
        int totalLevels = LevelManager.Instance.levelPrefabs.Length;

        for (int i = 0; i < totalLevels; i++)
        {
            PlayerPrefs.DeleteKey(CompletedLevelKeyPrefix + i);
        }

        PlayerPrefs.Save();
        Debug.Log("Progress reset.");
    }

    public static void ResetFireflies() // used by statues
    {
        PlayerPrefs.SetInt("CompletedLevelsCount", 0);
        PlayerPrefs.Save();
    }
}