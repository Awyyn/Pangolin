using TMPro;
using UnityEngine;

public class FireflyCounterUI : MonoBehaviour
{
    public static FireflyCounterUI Instance;

    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private Animator animator;

    [SerializeField] private string increaseAnimationName = "FireflyIncrease";
    [SerializeField] private string decreaseAnimationName = "FireflyDecrease";

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateCount(int newCount)
    {
        countText.text = newCount.ToString();
    }

    public void PlayIncreaseAnimation()
    {
        if (animator != null)
            animator.Play(increaseAnimationName, 0, 0f);
    }

    public void PlayDecreaseAnimation()
    {
        if (animator != null)
            animator.Play(decreaseAnimationName, 0, 0f);
    }
}