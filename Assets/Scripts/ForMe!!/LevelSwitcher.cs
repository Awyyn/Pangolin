#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class LevelSwitcher : EditorWindow
{
    private GameObject[] levels;

    [MenuItem("Tools/Level Switcher")]
    public static void ShowWindow()
    {
        GetWindow<LevelSwitcher>("Level Switcher");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Refresh Level List (incl. inactive)"))
        {
            FindAllLevels();
        }

        if (levels != null && levels.Length > 0)
        {
            EditorGUILayout.Space();

            if (GUILayout.Button("Deactivate All Levels"))
            {
                foreach (GameObject level in levels)
                {
                    level.SetActive(false);
                }
            }

            EditorGUILayout.Space();

            foreach (GameObject level in levels)
            {
                string label = level.activeSelf ? level.name + " [ACTIVE] " : level.name;
                if (GUILayout.Button("Switch to " + label))
                {
                    foreach (GameObject l in levels)
                    {
                        l.SetActive(false);
                    }

                    level.SetActive(true);
                    Selection.activeGameObject = level;
                    EditorGUIUtility.PingObject(level);
                    SceneView.lastActiveSceneView.FrameSelected();
                }
            }
        }
        else
        {
            EditorGUILayout.HelpBox("No levels found. Click 'Refresh Level List' to load.", MessageType.Info);
        }
    }
    private void FindAllLevels()
    {
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        levels = System.Array.FindAll(allObjects, go =>
            go.CompareTag("Level") &&
            go.scene.IsValid() &&
            !EditorUtility.IsPersistent(go)
        );

        // Sort by extracted number from name (e.g., "level1", "level2")
        System.Array.Sort(levels, (a, b) =>
        {
            int numA = ExtractTrailingNumber(a.name);
            int numB = ExtractTrailingNumber(b.name);
            return numA.CompareTo(numB);
        });
    }

    private int ExtractTrailingNumber(string name)
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

        return int.MaxValue; // fallback if no number
    }



}
#endif
