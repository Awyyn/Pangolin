using UnityEngine;

public class BossFightController : MonoBehaviour
{
    [Header("References")]
    public MapScroller scroller;
    public Transform boss;
    public Transform player;

    [Header("Settings")]
    public float startDelay = 0.3f;

    private bool bossStarted = false;


    private Vector3 playerStartPos;
    private Vector3 bossStartPos;
    private Vector3 mapRootStartPos;

    private void Start()
    {
        mapRootStartPos = scroller.transform.position;
        playerStartPos = player.position;
        bossStartPos = boss.position;
    }


    public void TriggerBossFight()
    {
        if (bossStarted) return;
        bossStarted = true;
        GameManager.Instance.bossMode = true;

        if (scroller != null)
        {
            scroller.enabled = true;
            scroller.StartScrolling();
            if (player != null)
            {
                player.SetParent(scroller.transform);
            }

        }

        if (boss != null)
            boss.position = bossStartPos;
    }

    public void RestartBossFight()
    {
        bossStarted = false;
        GameManager.Instance.bossMode = false;

        // reset map
        if (scroller != null)
        {
            scroller.StopScrolling();
            scroller.transform.position = mapRootStartPos;
        }

        // reset pangolin
        if (player != null)
            player.position = playerStartPos;

        // reset boss
        if (boss != null)
            boss.position = bossStartPos;


        // reset interactables
        var levelInstance = LevelManager.Instance.CurrentLevelInstance;
        if (levelInstance != null)
        {
            var interactables = levelInstance.GetComponentsInChildren<IInteractable>(true);
            foreach (var it in interactables)
                it.ResetState();
        }

        // reset firefly
        LevelManager.Instance.RespawnSoul();
    }



}
