using UnityEngine;

public class StatueOffering : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private bool hasActivated = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasActivated) return;
        if (!other.CompareTag("Player")) return;

        hasActivated = true;

        ConsumeAllFireflies();
        StartOfferingSequence();
    }

    private void ConsumeAllFireflies()
    {
        int current = PlayerProgress.GetFireflyCount(LevelManager.Instance.levelPrefabs.Length);

        if (current <= 0) return;

        PlayerProgress.ResetFireflies();

        FireflyCounterUI.Instance?.UpdateCount(0);
        FireflyCounterUI.Instance?.PlayDecreaseAnimation();
    }

    private void StartOfferingSequence()
    {
        animator.SetTrigger("Offer");
    }
}