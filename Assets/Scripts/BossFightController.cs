using UnityEngine;

public class BossFightController : MonoBehaviour
{
    [Header("References")]
    public MapScroller scroller;
    public Transform boss;
    public Transform player;

    [Header("Settings")]
    public float startDelay = 0.3f;

    private Vector3 bossStartPos;
    private bool bossStarted = false;

    void Start()
    {
        bossStartPos = boss.position;
        if (scroller != null)
            scroller.enabled = false; // off until fight begins
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
        }

        if (boss != null)
            boss.position = bossStartPos;
    }

    public void RestartBossFight()
    {
        bossStarted = false;
        GameManager.Instance.bossMode = false;

        if (scroller != null)
        {
            scroller.transform.position = Vector3.zero;
            scroller.enabled = false;
        }

        if (player != null)
            player.position = Vector3.zero;

        if (boss != null)
            boss.position = bossStartPos;
    }
}
