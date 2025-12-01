using UnityEngine;
using UnityEngine.UI;

public class DebugTools : MonoBehaviour
{
    public Button resetProgressButton;

    void Start()
    {
        resetProgressButton.onClick.AddListener(ResetProgress);
    }

    public void ResetProgress()
    {
        int totalLevels = FindFirstObjectByType<LevelManager>().levelPrefabs.Length;
        PlayerProgress.ResetProgress(totalLevels);

        PlayerProgress.ResetFireflies();           // reset fireflies
        FireflyCounterUI.Instance?.UpdateCount(0); // update UI
        Debug.Log("Progress reset!");
    }

}

