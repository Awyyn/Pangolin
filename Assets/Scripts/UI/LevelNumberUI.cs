using TMPro;
using UnityEngine;

public class LevelNumberUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText;

    public void SetLevelNumber(int levelIndex)
    {
        // levelIndex is usually 0-based → players see 1-based
        levelText.text = (levelIndex + 1).ToString() + ".";
    }
}
