using UnityEngine;

public class BossChase : MonoBehaviour
{
    public Transform player;
    public float loseDistance = 1.5f;

    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);
        if (distance < loseDistance)
        {
            var boss = Object.FindFirstObjectByType<BossFightController>();
            if (boss != null)
            boss.RestartBossFight();
        }
    }
}
