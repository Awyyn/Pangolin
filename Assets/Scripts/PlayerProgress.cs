using UnityEngine;

public static class PlayerProgress
{
    static string key = "HighestLevelCompleted";

    public static int GetHighestLevel()
    {
        return PlayerPrefs.GetInt(key, 0);
    }

    public static void SetHighestLevel(int level)
    {
        PlayerPrefs.SetInt(key, level);
        PlayerPrefs.Save();
    }

    public static bool IsLevelUnlocked(int level)
    {
        int highest = GetHighestLevel();

        if (highest == 0)
        {
            // Only unlock the first level (index 0)
            return level == 0;
        }
        else
        {
            // Unlock all levels up to highest completed + 1
            return level <= highest + 1;
        }
    }


}

