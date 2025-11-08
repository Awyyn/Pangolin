// BossChase.cs
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class BossChase : MonoBehaviour
{
    [Header("Camera Follow Settings")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float offsetX = 3f;
    [SerializeField] private float offsetY = 0f;

    [Header("Player Settings")]
    [SerializeField] private Transform player;

    [Header("Animator (optional)")]
    [SerializeField] private Animator animator;

    private void Awake()
    {
        // Auto-assign camera if not set
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        // Find the player/pangolin
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player != null)
            animator = player.GetComponent<Animator>(); // assign the pangolin's animator here

        if (animator == null)
            Debug.LogWarning("[BossChase] Animator not found on player!");
    }

    private void LateUpdate()
    {
        // Always follow the camera
        if (cameraTransform != null)
        {
            Vector3 newPos = cameraTransform.position;
            newPos.x += offsetX;
            newPos.y += offsetY;
            newPos.z = transform.position.z; // keep original z
            transform.position = newPos;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            StartCoroutine(RestartLevel());
    }

    private IEnumerator RestartLevel()
    {
        // Optional animation
        if (animator != null)
            animator.SetTrigger("Scared");

        // Wait a bit before restarting
        yield return new WaitForSeconds(1f);

        var levelManager = FindFirstObjectByType<LevelManager>();
        if (levelManager != null)
            levelManager.ResetLevel();
    }
}