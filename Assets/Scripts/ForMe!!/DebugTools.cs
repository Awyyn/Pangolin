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
        PlayerProgress.ResetProgress(LevelManager.Instance.levelPrefabs.Length);
        Debug.Log("Progress reset!");
    }
}

