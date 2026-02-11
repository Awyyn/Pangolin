using TMPro;
using UnityEngine;

public class FireflyCounterUI : MonoBehaviour
{
    public static FireflyCounterUI Instance;

    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private Animator animator;

    [SerializeField] private string increaseAnimationName = "FireflyIncrease"; // ui firefly counter increase animation

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateCount(int newCount)
    {
        countText.text = newCount.ToString();
        Debug.Log("[FireflyUI] Updated text to: " + countText.text);

        if (animator != null && !string.IsNullOrEmpty(increaseAnimationName))
        {
            // Directly play the animation from the first frame
            animator.Play(increaseAnimationName, 0, 0f);
            animator.Update(0f); // force immediate evaluation
        }
    }
}
