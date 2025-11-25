using UnityEngine;

public class FireflyPickup : MonoBehaviour
{
    [SerializeField] private float nextLevelDelay = 1.5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        // Try to count the firefly (LevelManager will ignore it if the level was completed before)
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnFireflyCollected(1);
        }

        // Mark level completed (this should happen regardless of whether the firefly was counted)
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.MarkLevelCompleted();
        }

        // Play player's look-up animation (defensive: check parameter exists)
        var player = collision.GetComponent<PlayerMovement>();
        if (player != null)
        {
            // Make sure the Animator has the "LookUp" trigger parameter in the AnimatorController
            player.animator.SetTrigger("LookUp");
        }

        // Stop camera scrolling and boss chase
        var cam = FindFirstObjectByType<CameraScroller>();
        if (cam != null) cam.StopScrolling();

        var boss = FindFirstObjectByType<BossChase>();
        if (boss != null) boss.enabled = false;

        // Queue next level
        if (LevelManager.Instance != null)
            LevelManager.Instance.LoadNextLevelWithDelay(nextLevelDelay);

        // Destroy the firefly object
        Destroy(gameObject);
    }
}
