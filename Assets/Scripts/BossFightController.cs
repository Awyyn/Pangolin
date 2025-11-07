using UnityEngine;
using UnityEngine.UIElements;

public class BossFightController : MonoBehaviour
{
    [Header("References")]
    public CameraScroller cameraScroller;
    public Transform boss;
    public Transform player;

    private bool bossStarted = false;
    private Vector3 bossStartPos;
    private Vector3 playerStartPos;

    private void Start()
    {
        bossStartPos = boss.position;
        playerStartPos = player.position;
    }

    public void TriggerBossFight()
    {
        if (CameraScroller.Instance != null)
            CameraScroller.Instance?.StartScrolling();
        else
            Debug.LogWarning("[BossFightController] No CameraScroller instance found!");
    }



    public void RestartBossFight()
    {
        Debug.Log("[BossFightController] RestartBossFight() called");

        bossStarted = false;

        if (cameraScroller == null)
        {
            // try to find one if inspector reference is missing
            cameraScroller = Object.FindFirstObjectByType<CameraScroller>();
            Debug.Log("[BossFightController] cameraScroller was null, found: " + (cameraScroller != null));
        }

        if (cameraScroller != null)
        {
            cameraScroller.StopScrolling();
            cameraScroller.ResetCamera();
        }
        else
        {
            Debug.LogWarning("[BossFightController] No CameraScroller found during RestartBossFight()");
        }

        // reset positions
        if (boss != null) boss.position = bossStartPos;

        if (player != null)
        {
            // restore player using LevelManager start position for consistency
            if (LevelManager.Instance != null && LevelManager.Instance.CurrentLevelInstance != null)
            {
                PlayerMovement.instance.SetStartPositionFromLevel(LevelManager.Instance.CurrentLevelInstance);
                PlayerMovement.instance.ForceFacing(PangolinStartPoint.FacingDirection.Right);
            }
            else
            {
                player.position = playerStartPos;
            }
        }

        // reset interactables and soul
        var levelInstance = LevelManager.Instance?.CurrentLevelInstance;
        if (levelInstance != null)
        {
            var interactables = levelInstance.GetComponentsInChildren<IInteractable>(true);
            foreach (var it in interactables) it.ResetState();
        }
        LevelManager.Instance?.RespawnSoul();

        Debug.Log("[BossFightController] RestartBossFight() finished. camera scrolling state: " + (cameraScroller != null ? cameraScroller.IsScrolling().ToString() : "no camera"));
    }


    /*

    public void RestartBossFight()
    {
        // stop scrolling and snap camera back
        if (cameraScroller != null)
        {
            cameraScroller.StopScrolling();
            cameraScroller.ResetCamera();
        }

        // reset world positions
        if (boss != null) boss.position = bossStartPos;
        if (player != null) player.position = playerStartPos;

        // reset interactables & soul
        var levelInstance = LevelManager.Instance.CurrentLevelInstance;
        if (levelInstance != null)
        {
            var interactables = levelInstance.GetComponentsInChildren<IInteractable>(true);
            foreach (var it in interactables) it.ResetState();
        }

        LevelManager.Instance.RespawnSoul();
        bossStarted = false;
    }
    */

}
