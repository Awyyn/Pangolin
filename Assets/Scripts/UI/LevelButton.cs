using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelButton : MonoBehaviour
{
    public TextMeshProUGUI levelNumberText;  // To show the level number
    public GameObject lockIcon;  // Lock icon for locked levels
    public Button button;  // Reference to the button component

    private int levelIndex;
    private System.Action<int> onClickCallback;

    // Setup method to assign values dynamically to each button
    public void Setup(int index, bool isUnlocked, System.Action<int> callback)
    {
        levelIndex = index;
        onClickCallback = callback;

        levelNumberText.text = (index + 1).ToString();  // Set the level number
        lockIcon.SetActive(!isUnlocked);  // Show the lock icon if locked
        button.interactable = isUnlocked;  // Button is interactable only if unlocked
    }

    // Method called when the button is clicked
    public void OnClick()
    {
        if (button.interactable && onClickCallback != null)
        {
            onClickCallback(levelIndex);  // Pass the level index to the callback
        }
    }
}
