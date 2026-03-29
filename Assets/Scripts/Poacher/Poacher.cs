using UnityEngine;
using System.Collections;

public class Poacher : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [Header("Start direction")]
    [SerializeField] private Direction startingDirection = Direction.Right;
    private Direction currentDirection;

    [Header("Player reference")]
    [SerializeField] private Transform pangolin;
    [SerializeField] private PlayerMovement player;

    [Header("Flashlight sprites")]
    [SerializeField] private GameObject lightRight;
    [SerializeField] private GameObject lightLeft;
    [SerializeField] private GameObject lightUp;
    [SerializeField] private GameObject lightDown;

    [Header("Feet for detection")]
    [SerializeField] private Transform feet;
    
    public AudioClip poacherSound;
    public AudioClip scaredPangolinSound;

    // coroutine handle to prevent double detection
    private Coroutine detectCoroutine;

    private void Start()
    {
        if (!animator) animator = GetComponent<Animator>();

        // auto-find player if not assigned
        if (!pangolin) pangolin = GameObject.FindGameObjectWithTag("Player").transform;
        if (!player) player = pangolin.GetComponent<PlayerMovement>();

        // auto-find feet if not assigned
        if (!feet) feet = transform.Find("Feet");

        currentDirection = startingDirection;

        UpdateIdleAnimation();
        UpdateLight();
    }

    private void UpdateLight()
    {
        lightRight.SetActive(currentDirection == Direction.Right);
        lightLeft.SetActive(currentDirection == Direction.Left);
        lightUp.SetActive(currentDirection == Direction.Up);
        lightDown.SetActive(currentDirection == Direction.Down);
    }

    private void CheckForPangolin()
    {
        Vector3Int poacherCell = GridManager.Instance.groundTilemap.WorldToCell(feet.position);
        Vector3Int playerCell = GridManager.Instance.groundTilemap.WorldToCell(pangolin.position);

        int ox = poacherCell.x;
        int oy = poacherCell.y;

        int px = playerCell.x;
        int py = playerCell.y;

        bool caught = false;

        switch (currentDirection)
        {
            case Direction.Right: caught = (py == oy) && (px > ox); break;
            case Direction.Left:  caught = (py == oy) && (px < ox); break;
            case Direction.Up:    caught = (px == ox) && (py > oy); break;
            case Direction.Down:  caught = (px == ox) && (py < oy); break;
        }

        if (caught) CatchPlayer();
    }

    private void CatchPlayer()
    {
        animator.Play(currentDirection == Direction.Right ? "PoacherAlertedRight" : "PoacherAlertedLeft");
        player.animator.Play("ScaredSide");
        if (PlayerMovement.instance != null)
        {
            SFXManager.Instance.PlaySFX(scaredPangolinSound);
            PlayerMovement.instance.inputLocked = true;
            // Optional: unlock after 1 second (length of scared animation)
            StartCoroutine(UnlockPlayerAfterScared(1f));
        }
        StartCoroutine(RestartLevelDelay());
    }
    private IEnumerator UnlockPlayerAfterScared(float duration)
    {
        yield return new WaitForSeconds(duration);
        if (PlayerMovement.instance != null)
            PlayerMovement.instance.inputLocked = false;
    }

    private IEnumerator RestartLevelDelay()
    {
        yield return new WaitForSeconds(1.5f);
        LevelManager.Instance.ResetLevel();
        animator.Play("PoacherRight");
    }

    private void PlayPoacherSound() 
    {
        if (poacherSound != null)
            SFXManager.Instance.PlaySFX(poacherSound);
    }
    
    // Called whenever player moves
    public void RotateCounterClockwise()
    {
        PlayPoacherSound();
        
        // Immediately update direction and play turn animation
        currentDirection = GetNextDirectionCCW(currentDirection);
        PlayTurnAnimation(currentDirection);
        

        // Start delayed detection, cancel previous coroutine if still running
        if (detectCoroutine != null)
            StopCoroutine(detectCoroutine);

        detectCoroutine = StartCoroutine(DetectAfterDelay(0.2f));
    }

    private IEnumerator DetectAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        CheckForPangolin();
        detectCoroutine = null;
    }

    private Direction GetNextDirectionCCW(Direction dir)
    {
        switch (dir)
        {
            case Direction.Right: return Direction.Up;
            case Direction.Up:    return Direction.Left;
            case Direction.Left:  return Direction.Down;
            case Direction.Down:  return Direction.Right;
            default: return Direction.Right;
        }
    }

    private void PlayTurnAnimation(Direction newDir)
    {
        currentDirection = newDir;
        animator.SetInteger("Facing", (int)newDir);

        switch (newDir)
        {
            case Direction.Right: animator.Play("PoacherTurnRight", 0, 0f); break;
            case Direction.Up:    animator.Play("PoacherTurnUp", 0, 0f); break;
            case Direction.Left:  animator.Play("PoacherTurnLeft", 0, 0f); break;
            case Direction.Down:  animator.Play("PoacherTurnDown", 0, 0f); break;
        }

        animator.Update(0f); // apply immediately
        UpdateLight();
    }

    private void UpdateIdleAnimation()
    {
        animator.SetInteger("Facing", (int)currentDirection);
    }

    private void OnEnable()
    {
        PlayerMovement.OnPlayerStepComplete += RotateCounterClockwise;
    }

    private void OnDisable()
    {
        PlayerMovement.OnPlayerStepComplete -= RotateCounterClockwise;
    }

    public void ResetPoacher()
    {
        currentDirection = startingDirection;
        UpdateIdleAnimation();
        UpdateLight();
    }
}

public enum Direction
{
    Right,
    Up,
    Left,
    Down
}