using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using static PangolinStartPoint;

/// <summary>
/// Controls the player movement 
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance;

    public GameManager gameManager;
    public MovesManager movesManager;  
    public LevelManager levelManager;  

    public float moveSpeed = 10f;
    private bool inputEnabled = true; // default: input allowed


    private Vector3 targetPosition;
    private Vector3 startingPosition;
    private bool isMoving = false;
    private Vector3 lastBumpDirection;

    private Vector3 lastDirection = Vector3.zero;
    private Vector3 previousDirection = Vector3.zero;

    public bool inputLocked { get; set; }
    public bool reachedFirefly = false;
    private bool outOfMovesTriggered = false;

    public float inputCooldown = 0.25f;   // normal cooldown between moves
    public float bumpCooldown = 0.6f;     // cooldown after bump

    public Animator animator;
    private SpriteRenderer spriteRenderer;

    public AudioClip bumpSound;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (levelManager == null)
            levelManager = LevelManager.Instance;
        if (movesManager == null)
            movesManager = FindFirstObjectByType<MovesManager>();
    }

    private void Update()
    {
        if (!inputEnabled)
            return;
        if (inputLocked || LevelManager.Instance == null || LevelManager.Instance.levelCompleted)
            return;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.R))
        {
            LevelManager.Instance.ResetLevel();
            return;
        }

        if (movesManager.movesLeft <= 0)
        {
            if (!outOfMovesTriggered && !isMoving)
            {
                outOfMovesTriggered = true;
                animator.SetTrigger("Sleep");
                Debug.Log("Out of moves! Falling asleep.");
            }
            return;
        }

        if (!isMoving && movesManager.movesLeft > 0)
        {
            Vector3 direction = Vector3.zero;

            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                direction = Vector3.up;
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                direction = Vector3.down;
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                direction = Vector3.left;
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                direction = Vector3.right;

            if (direction != Vector3.zero)
            {
                inputLocked = true;

                // Store direction history
                previousDirection = lastDirection;
                lastDirection = direction;

                // Animator parameters
                animator.SetFloat("moveX", direction.x);
                animator.SetFloat("moveY", direction.y);
                int tailIndex = GetTailAngleIndex(previousDirection, direction);
                animator.SetFloat("tailAngleIndex", tailIndex);
                animator.SetBool("isMoving", true);

                // Flip sprite if moving left
                spriteRenderer.flipX = direction == Vector3.left;

                // Handle movement
                lastBumpDirection = direction;
                Vector3 nextPos = targetPosition + direction;

                movesManager.ModifyMoves(-1);

                if (GridManager.Instance.CanMoveTo(nextPos))
                {
                    targetPosition = nextPos;
                    StartCoroutine(MoveToPosition(targetPosition));
                    StartCoroutine(InputCooldown());
                    if (!GameManager.Instance.bossMode && GameManager.Instance.isBossLevel)
                    {
                        var bossController = Object.FindFirstObjectByType<BossFightController>();
                        if (bossController != null)
                        {
                            Debug.Log("[PlayerMovement] Triggering boss fight");
                            bossController.TriggerBossFight();
                        }
                        else
                        {
                            Debug.LogWarning("[PlayerMovement] No BossFightController found!");
                        }

                        GameManager.Instance.bossMode = true;
                    }
                }
                else
                {
                    // blocked by wall or interactable
                    Collider2D hitCollider = Physics2D.OverlapPoint(nextPos);
                    bool reacted = false;

                    if (hitCollider != null)
                    {
                        IInteractable interactable = hitCollider.GetComponent<IInteractable>();
                        if (interactable != null)
                        {
                            interactable.Interact(direction);
                            reacted = true;

                            Rock rockScript = hitCollider.GetComponent<Rock>();
                            if (rockScript != null)
                            {
                                StartCoroutine(BumpAnimation());
                                StartCoroutine(InputCooldown(bumpCooldown));
                            }
                            else
                            {
                                StartCoroutine(BumpAnimation());
                                StartCoroutine(InputCooldown(bumpCooldown));
                            }
                        }
                    }

                    if (!reacted)
                    {
                        StartCoroutine(BumpAnimation());
                        StartCoroutine(InputCooldown(bumpCooldown));
                    }
                }

                inputLocked = false;
            }
            else
            {
                animator.SetBool("isMoving", false);
            }
        }
    }

    private IEnumerator MoveToPosition(Vector3 destination)
    {
        isMoving = true;

        while ((transform.position - destination).sqrMagnitude > 0.001f)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = destination;
        isMoving = false;
        animator.SetBool("isMoving", false);
    }

    private IEnumerator InputCooldown(float delay = 0.25f)
    {
        inputLocked = true;
        yield return new WaitForSeconds(delay);
        inputLocked = false;
    }

    private IEnumerator BumpAnimation()
    {
        PlayBumpSound();

        // Make bump directional instantly
        animator.SetFloat("moveX", lastBumpDirection.x);
        animator.SetFloat("moveY", lastBumpDirection.y);

        animator.ResetTrigger("Bump");
        animator.SetTrigger("Bump");
        animator.SetFloat("tailAngleIndex", 0f);

        // No waiting; animation plays immediately
        yield break;
    }

    // Called automatically by Animation Event at the end of "Bump" clip
    public void OnBumpAnimationEnd()
    {
        // Clean up bump trigger, but do NOT block movement
        animator.ResetTrigger("Bump");
        animator.SetBool("isMoving", false);
        SnapToIdleAfterBump();

    }
    private void PlayBumpSound()
    {
        if (bumpSound != null)
            SFXManager.Instance.PlaySFX(bumpSound);
    }

    private void SnapToIdleAfterBump()
    {
        // choose idle based on last bump direction
        string idleState = "SideIdle";
        if (lastBumpDirection.y > 0) idleState = "UpIdle";
        else if (lastBumpDirection.y < 0) idleState = "DownIdle";
        // flipX already set when bump happened

        animator.Play(idleState, 0, 0f);   // snap immediately
        animator.Update(0f);              // force immediate apply
    }


    private int GetTailAngleIndex(Vector3 previousDir, Vector3 currentDir)
    {
        if (previousDir == Vector3.zero || previousDir == currentDir)
            return 0;

        if (currentDir == Vector3.up)
        {
            if (previousDir == Vector3.right) return 1;
            if (previousDir == Vector3.left) return 2;
        }
        else if (currentDir == Vector3.down)
        {
            if (previousDir == Vector3.right) return 2;
            if (previousDir == Vector3.left) return 1;
        }
        else if (currentDir == Vector3.right)
        {
            if (previousDir == Vector3.up) return 2;
            if (previousDir == Vector3.down) return 1;
        }
        else if (currentDir == Vector3.left)
        {
            if (previousDir == Vector3.up) return 2;
            if (previousDir == Vector3.down) return 1;
        }

        return 0;
    }

    public void ResetLevelFlags()
    {
        StopAllCoroutines();
        reachedFirefly = false;
        outOfMovesTriggered = false;
        inputLocked = false;
    }

    public void ForceFacing(PangolinStartPoint.FacingDirection facing)
    {
        if (!animator) return;

        float mx = 0f, my = 0f;
        string idleState = "SideIdle";

        switch (facing)
        {
            case PangolinStartPoint.FacingDirection.Right: mx = 1f; my = 0f; spriteRenderer.flipX = false; idleState = "SideIdle"; break;
            case PangolinStartPoint.FacingDirection.Left: mx = -1f; my = 0f; spriteRenderer.flipX = true; idleState = "SideIdle"; break;
            case PangolinStartPoint.FacingDirection.Up: mx = 0f; my = 1f; spriteRenderer.flipX = false; idleState = "UpIdle"; break;
            case PangolinStartPoint.FacingDirection.Down: mx = 0f; my = -1f; spriteRenderer.flipX = false; idleState = "DownIdle"; break;
        }

        animator.SetFloat("moveX", mx);
        animator.SetFloat("moveY", my);
        animator.SetFloat("tailAngleIndex", 0f);
        animator.SetBool("isMoving", false);

        animator.Play(idleState, 0, 0f);
        animator.Update(0f);
    }
    public void SetStartPositionFromLevel(GameObject levelInstance)
    {
        LevelData levelData = levelInstance.GetComponent<LevelData>();

        if (levelData != null && levelData.PangolinStartPoint != null)
        {
            startingPosition = levelData.PangolinStartPoint.position;
            transform.position = startingPosition;
            targetPosition = startingPosition;

            // Reset movement state
            isMoving = false;
            inputLocked = false;
            previousDirection = Vector3.zero;
            lastDirection = Vector3.right;

            // Read facing from the start point & apply next frame
            var sp = levelData.PangolinStartPoint.GetComponent<PangolinStartPoint>();
            var face = sp ? sp.startFacing : PangolinStartPoint.FacingDirection.Right;
            StartCoroutine(ResetAnimatorNextFrame(face));
        }
        else
        {
            Debug.LogWarning("Pangolin start position not assigned in LevelData!");
            startingPosition = transform.position;
            targetPosition = transform.position;

            StartCoroutine(ResetAnimatorNextFrame(PangolinStartPoint.FacingDirection.Right));
        }
    }

    private IEnumerator ResetAnimatorNextFrame(PangolinStartPoint.FacingDirection facing)
    {
        yield return null;
        ForceFacing(facing);
    }
    public void SetInputEnabled(bool enabled)
    {
        inputEnabled = enabled;
    }


}
