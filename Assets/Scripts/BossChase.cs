using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class BossChase : MonoBehaviour
{
    [Header("Camera Follow")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] public float offsetX = -15f;
    [SerializeField] private float offsetY = 0f;
    
    [SerializeField] private Animator playerAnimator;
    
    public AudioClip poacherSound;
    private Vector3 offset;

    [Header("Animator")]
    [SerializeField] private Animator animator;

    private bool hasStarted = false;
    private bool isStopped = false;
    
    public static BossChase Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        
        if (playerAnimator == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerAnimator = player.GetComponent<Animator>();
        }
        
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        if (animator == null)
            animator = GetComponent<Animator>();
    }
    
    private void Start()
    {
        if (cameraTransform != null)
        {
            offset = transform.position - cameraTransform.position;
        }
    }

    private void OnEnable()
    {
        PlayerMovement.OnPlayerStepComplete += OnPlayerStep;
        hasStarted = false;
        isStopped = false;
        animator.SetBool("isWalking", false);
    }

    private void OnDisable()
    {
        PlayerMovement.OnPlayerStepComplete -= OnPlayerStep;
    }

    private void LateUpdate()
    {
        if (cameraTransform == null) return;

        transform.position = cameraTransform.position + offset;
    }

    private void OnPlayerStep()
    {
        if (hasStarted || isStopped) return;

        hasStarted = true;

        animator.SetBool("isWalking", true);
        CameraScroller.Instance?.StartScrolling();
    }

    public void StopChase()
    {
        if (isStopped) return;

        isStopped = true;
        
        CameraScroller.Instance?.StopScrolling();
        animator.SetBool("isWalking", false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        
        if (playerAnimator != null)
            playerAnimator.SetTrigger("Scared");
        //CameraScroller.Instance?.StopScrolling();
        StartCoroutine(RestartLevel());
    }

    private IEnumerator RestartLevel()
    {
        yield return new WaitForSeconds(1f);

        animator.SetBool("isWalking", false);
        var levelManager = FindFirstObjectByType<LevelManager>();
        if (levelManager != null)
            levelManager.ResetLevel();
    }
    public void ResetBoss()
    {
        hasStarted = false;
        isStopped = false;

        animator.SetBool("isWalking", false);
        RefreshPlayer();
    }
    
    private void RefreshPlayer()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerAnimator = player.GetComponent<Animator>();
    }
    private void PlayPoacherSound() 
    {
        if (poacherSound != null)
            SFXManager.Instance.PlaySFX(poacherSound);
    }
}


// BossChase.cs
/*using UnityEngine;
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
}*/