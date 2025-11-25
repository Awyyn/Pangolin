using TMPro;
using UnityEngine;

public class FireflyCounterUI : MonoBehaviour
{
    public static FireflyCounterUI Instance;

    [SerializeField] private TextMeshProUGUI countText;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateCount(int newCount)
    {
        countText.text = newCount.ToString();
        Debug.Log("[FireflyUI] Updated text to: " + countText.text);
    }
}
