using UnityEngine;

public class LevelKeySwitcher : MonoBehaviour
{
    private GameObject[] levels;
    private int currentIndex = 0;

    void Start()
    {
        // Find all levels by tag (even if inactive)
        levels = FindAllLevels();
        SortLevelsByNumber(levels);
        SwitchToLevel(0); // start with the first level
    }

    void Update()
    {
        if (levels == null || levels.Length == 0) return;

        // Number keys 1–9 for direct access
        for (int i = 0; i < 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                if (i < levels.Length)
                    SwitchToLevel(i);
            }
        }

        // Arrow keys
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            int next = (currentIndex + 1) % levels.Length;
            SwitchToLevel(next);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            int prev = (currentIndex - 1 + levels.Length) % levels.Length;
            SwitchToLevel(prev);
        }
    }

    void SwitchToLevel(int index)
    {
        if (index < 0 || index >= levels.Length) return;

        currentIndex = index;
        LevelManager.Instance.InitializeLevel(index);
    }



    GameObject[] FindAllLevels()
    {
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        return System.Array.FindAll(allObjects, go =>
            go.CompareTag("Level") &&
            go.scene.IsValid() &&
            !go.hideFlags.HasFlag(HideFlags.NotEditable) &&
            !go.hideFlags.HasFlag(HideFlags.HideAndDontSave)
        );
    }

    void SortLevelsByNumber(GameObject[] levels)
    {
        System.Array.Sort(levels, (a, b) =>
        {
            return ExtractTrailingNumber(a.name).CompareTo(ExtractTrailingNumber(b.name));
        });
    }

    int ExtractTrailingNumber(string name)
    {
        string number = "";
        for (int i = name.Length - 1; i >= 0; i--)
        {
            if (char.IsDigit(name[i]))
                number = name[i] + number;
            else
                break;
        }

        if (int.TryParse(number, out int result))
            return result;

        return int.MaxValue;
    }
}
