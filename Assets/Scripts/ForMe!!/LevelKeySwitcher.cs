using UnityEngine;

public class LevelKeySwitcher : MonoBehaviour
{
    private int currentIndex = 0;

    void Update()
    {
        if (LevelManager.Instance == null || LevelManager.Instance.levelPrefabs.Length == 0) return;

        // Number keys 1–9 to jump to a specific level
        for (int i = 0; i < 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i) && i < LevelManager.Instance.levelPrefabs.Length)
            {
                currentIndex = i;
                LevelManager.Instance.InitializeLevel(currentIndex);
            }
        }

        // Arrow keys for next/previous
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentIndex = (currentIndex + 1) % LevelManager.Instance.levelPrefabs.Length;
            LevelManager.Instance.InitializeLevel(currentIndex);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentIndex = (currentIndex - 1 + LevelManager.Instance.levelPrefabs.Length) % LevelManager.Instance.levelPrefabs.Length;
            LevelManager.Instance.InitializeLevel(currentIndex);
        }
    }
}
