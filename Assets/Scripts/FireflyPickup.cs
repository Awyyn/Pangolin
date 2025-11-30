using UnityEngine;

public class FireflyPickup : MonoBehaviour
{
    [SerializeField] private float nextLevelDelay = 1.5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        var player = collision.GetComponent<PlayerMovement>();
        player?.animator.SetTrigger("LookUp");

        FindFirstObjectByType<CameraScroller>()?.StopScrolling();
        var boss = FindFirstObjectByType<BossChase>();
        if (boss != null) boss.enabled = false;

        // Increment firefly for the level if not already completed
        LevelManager.Instance?.OnFireflyCollected();

        // Mark the level as completed
        LevelManager.Instance?.MarkLevelCompleted();

        // Load next level after delay
        LevelManager.Instance?.LoadNextLevelWithDelay(nextLevelDelay);

        Destroy(gameObject);
    }
}
