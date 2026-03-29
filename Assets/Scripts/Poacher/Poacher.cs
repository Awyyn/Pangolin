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

    // Called after poacher finishes turning
    private void CheckForPangolin()
    {
        
        Debug.Log($"Feet world: {feet.position}, Player world: {pangolin.position}");
        
        // Use feet position for tile calculation
        Vector3Int poacherCell = GridManager.Instance.groundTilemap.WorldToCell(feet.position);
        Vector3Int playerCell = GridManager.Instance.groundTilemap.WorldToCell(pangolin.position);
        Debug.Log($"Poacher cell: {poacherCell}, Player cell: {playerCell}");
        
        int ox = poacherCell.x;
        int oy = poacherCell.y;

        int px = playerCell.x;
        int py = playerCell.y;

        bool caught = false;

        switch (currentDirection)
        {
            case Direction.Right:
                caught = (py == oy) && (px > ox);
                break;
            case Direction.Left:
                caught = (py == oy) && (px < ox);
                break;
            case Direction.Up:
                caught = (px == ox) && (py > oy);
                break;
            case Direction.Down:
                caught = (px == ox) && (py < oy);
                break;
        }

        Debug.Log($"Poacher: ({ox},{oy}) Player: ({px},{py}) Facing: {currentDirection}, bool ({caught})");

        if (caught) CatchPlayer();
    }

    private void CatchPlayer()
    {
        animator.Play(currentDirection == Direction.Right ? "PoacherAlertedRight" : "PoacherAlertedLeft");

        player.animator.Play("ScaredSide");

        StartCoroutine(RestartLevelDelay());
    }

    private IEnumerator RestartLevelDelay()
    {
        yield return new WaitForSeconds(1.5f);
        LevelManager.Instance.ResetLevel();
    }

    // Called whenever player moves
    public void RotateCounterClockwise()
    {
        currentDirection = GetNextDirectionCCW(currentDirection);
        PlayTurnAnimation(currentDirection);

        // <-- call detection right after turning
        CheckForPangolin();
    }

    private Direction GetNextDirectionCCW(Direction dir)
    {
        switch (dir)
        {
            case Direction.Right: return Direction.Up;
            case Direction.Up: return Direction.Left;
            case Direction.Left: return Direction.Down;
            case Direction.Down: return Direction.Right;
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
            case Direction.Up: animator.Play("PoacherTurnUp", 0, 0f); break;
            case Direction.Left: animator.Play("PoacherTurnLeft", 0, 0f); break;
            case Direction.Down: animator.Play("PoacherTurnDown", 0, 0f); break;
        }

        animator.Update(0f); // apply immediately
        UpdateLight();
    }

    public void OnTurnAnimationFinished()
    {
        UpdateIdleAnimation();
        CheckForPangolin();
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

/*
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

    private bool hasStartedWalking = false;

    private void Start()
    {
        if (!animator) animator = GetComponent<Animator>();
        if (!pangolin) pangolin = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (!player) player = pangolin?.GetComponent<PlayerMovement>();
        if (!feet) feet = transform.Find("Feet");

        currentDirection = startingDirection;

        if (LevelManager.Instance.currentLevelInstance.GetComponent<LevelData>().isBossLevel)
        {
            currentDirection = Direction.Right;
            animator.SetBool("isWalking", false);
        }

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

    // Normal levels only
    private void CheckForPangolin()
    {
        if (LevelManager.Instance.currentLevelInstance.GetComponent<LevelData>().isBossLevel) return;

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
            case Direction.Left: caught = (py == oy) && (px < ox); break;
            case Direction.Up: caught = (px == ox) && (py > oy); break;
            case Direction.Down: caught = (px == ox) && (py < oy); break;
        }

        if (caught) CatchPlayer();
    }

    private void CatchPlayer()
    {
        animator.Play(currentDirection == Direction.Right ? "PoacherAlertedRight" : "PoacherAlertedLeft");
        player.animator.Play("ScaredSide");
        StartCoroutine(RestartLevelDelay());
    }

    private IEnumerator RestartLevelDelay()
    {
        yield return new WaitForSeconds(1.5f);
        LevelManager.Instance.ResetLevel();
    }

    // Called when player moves
    private void OnPlayerStep()
    {
        if (LevelManager.Instance.currentLevelInstance.GetComponent<LevelData>().isBossLevel)
        {
            if (!hasStartedWalking)
            {
                animator.SetBool("isWalking", true);
                hasStartedWalking = true;
                CameraScroller.Instance?.StartScrolling();
            }
            return; // skip normal rotation & checking
        }

        RotateCounterClockwise();
    }

    public void RotateCounterClockwise()
    {
        currentDirection = GetNextDirectionCCW(currentDirection);
        PlayTurnAnimation(currentDirection);
        CheckForPangolin();
    }

    private Direction GetNextDirectionCCW(Direction dir)
    {
        return dir switch
        {
            Direction.Right => Direction.Up,
            Direction.Up => Direction.Left,
            Direction.Left => Direction.Down,
            Direction.Down => Direction.Right,
            _ => Direction.Right
        };
    }

    private void PlayTurnAnimation(Direction newDir)
    {
        currentDirection = newDir;
        animator.SetInteger("Facing", (int)newDir);

        switch (newDir)
        {
            case Direction.Right: animator.Play("PoacherTurnRight", 0, 0f); break;
            case Direction.Up: animator.Play("PoacherTurnUp", 0, 0f); break;
            case Direction.Left: animator.Play("PoacherTurnLeft", 0, 0f); break;
            case Direction.Down: animator.Play("PoacherTurnDown", 0, 0f); break;
        }

        animator.Update(0f);
        UpdateLight();
    }

    public void OnTurnAnimationFinished()
    {
        UpdateIdleAnimation();
        CheckForPangolin();
    }

    private void UpdateIdleAnimation()
    {
        animator.SetInteger("Facing", (int)currentDirection);
    }

    private void OnEnable()
    {
        PlayerMovement.OnPlayerStepComplete += OnPlayerStep;
    }

    private void OnDisable()
    {
        PlayerMovement.OnPlayerStepComplete -= OnPlayerStep;
    }

    // Call this from the firefly when collected in boss level
    public void OnFireflyCollected()
    {
        if (!LevelManager.Instance.currentLevelInstance.GetComponent<LevelData>().isBossLevel) return;

        animator.SetBool("isWalking", false);
        currentDirection = Direction.Right;
        UpdateIdleAnimation();
        UpdateLight();
        CameraScroller.Instance?.StopScrolling();
    }

    public void ResetPoacher()
    {
        hasStartedWalking = false;

        if (LevelManager.Instance.currentLevelInstance.GetComponent<LevelData>().isBossLevel)
        {
            currentDirection = Direction.Right;
            animator.SetBool("isWalking", false);
        }
        else
        {
            currentDirection = startingDirection;
        }

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
 */