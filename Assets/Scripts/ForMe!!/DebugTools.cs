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
        PlayerPrefs.DeleteKey("HighestLevelCompleted");
        PlayerPrefs.Save();

        Debug.Log("Progress reset!");
    }
}
