using UnityEngine;

public class FireflyPickup : MonoBehaviour
{
    [SerializeField] private float nextLevelDelay = 1.5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("[FireflyPickup] TriggerEnter by " + collision.name);
        if (!collision.CompareTag("Player")) return;
        Debug.Log("[FireflyPickup] Player touched firefly");

        LevelManager.Instance?.OnFireflyCollected(1);
        LevelManager.Instance?.MarkLevelCompleted();

        var player = collision.GetComponent<PlayerMovement>();
        player?.animator.SetTrigger("LookUp");

        FindFirstObjectByType<CameraScroller>()?.StopScrolling();
        var boss = FindFirstObjectByType<BossChase>();
        if (boss != null) boss.enabled = false;

        LevelManager.Instance?.LoadNextLevelWithDelay(nextLevelDelay);

        Destroy(gameObject);
    }

}
