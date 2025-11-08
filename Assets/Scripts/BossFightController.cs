// BossFightController.cs
using UnityEngine;

public class BossFightController : MonoBehaviour
{
    [Header("References (optional)")]
    public CameraScroller cameraScroller;
    public Transform boss;
    public Transform player;

    private Vector3 bossStartPos;
    private Vector3 playerStartPos;

    private void Awake()
    {
        // Auto-assign boss if null (self if attached to boss prefab)
        if (boss == null)
            boss = this.transform;

        // Auto-assign player if null
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
            Debug.LogWarning("[BossFightController] Player not found!");

        // Auto-assign cameraScroller if null
        if (cameraScroller == null)
            cameraScroller = Object.FindFirstObjectByType<CameraScroller>();
    }

    private void Start()
    {
        if (boss != null)
            bossStartPos = boss.position;

        if (player != null)
            playerStartPos = player.position;
    }

    public void TriggerBossFight()
    {
        if (cameraScroller != null)
            cameraScroller.StartScrolling();
        else
            Debug.LogWarning("[BossFightController] No CameraScroller instance found!");
    }

    public void RestartBossFight()
    {
        // Stop scrolling and reset camera
        if (cameraScroller != null)
        {
            cameraScroller.StopScrolling();
            cameraScroller.ResetCamera();
        }

        // Reset positions
        if (boss != null) boss.position = bossStartPos;
        if (player != null) player.position = playerStartPos;

        // Reset interactables in current level
        var levelInstance = LevelManager.Instance?.CurrentLevelInstance;
        if (levelInstance != null)
        {
            var interactables = levelInstance.GetComponentsInChildren<IInteractable>(true);
            foreach (var it in interactables) it.ResetState();
        }

        LevelManager.Instance?.RespawnSoul();

        Debug.Log("[BossFightController] RestartBossFight finished.");
    }
}