using System.Collections;
using TMPro;
using Unity.Burst.CompilerServices;
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

    public GameManager gameManager;   //new. is it needed? it shoudl be an instance or something
    public MovesManager movesManager;  
    public LevelManager levelManager;  


    public float moveSpeed = 10f;

    private Vector3 targetPosition;
    private Vector3 startingPosition;
    private bool isMoving = false;
    private Vector3 lastBumpDirection;


    private Vector3 lastDirection = Vector3.zero;
    private Vector3 previousDirection = Vector3.zero;
    public bool inputLocked { get; set; }
    public bool reachedFirefly = false;

    private bool outOfMovesTriggered = false;

    public float inputCooldown = 0.25f; // Cooldown time for input

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    public AudioClip bumpSound;



    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (levelManager == null)
            levelManager = LevelManager.Instance;
        if (movesManager == null)
            movesManager = FindFirstObjectByType<MovesManager>();

        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Get starting position from level
       // SetStartPositionFromLevel(levelManager.currentLevelInstance);
    }


    private void Update()
    {
        if (inputLocked || LevelManager.Instance == null || LevelManager.Instance.levelCompleted)
            return;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.R)) //restart the level
        {
            LevelManager.Instance.ResetLevel(); 
            return;
        }


        if (movesManager.movesLeft <= 0)
        {
            if (!outOfMovesTriggered && !isMoving) // wait until not moving
            {
                outOfMovesTriggered = true;
                animator.SetTrigger("Sleep");
                Debug.Log("Out of moves! Falling asleep.");
            }
            return; // skip movement input
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

                // Flip sprite if player moves left. Don't flip if moving right
                spriteRenderer.flipX = direction == Vector3.left;

                // Handle movement logic
                lastBumpDirection = direction;
                Vector3 nextPos = targetPosition + direction;

                movesManager.ModifyMoves(-1);


                if (GridManager.Instance.CanMoveTo(nextPos))
                {
                    targetPosition = nextPos;
                    StartCoroutine(moveToPosition(targetPosition));


                    if (movesManager.movesLeft > 0)
                    {
                        StartCoroutine(InputCooldown());
                    }

                    // after first move, start boss if in boss level
                    if (!GameManager.Instance.bossMode)
                    {
                        var bossController = FindFirstObjectByType<BossFightController>();
                        if (bossController != null)
                            bossController.TriggerBossFight();
                    }

                }
                else // if the path is blocked by something
                {
                    Collider2D hitCollider = Physics2D.OverlapPoint(nextPos);
                    bool reacted = false;

                    if (hitCollider != null)
                    {
                        IInteractable interactable = hitCollider.GetComponent<IInteractable>();
                        if (interactable != null)
                        {
                            // Interact once
                            interactable.Interact(direction);
                            reacted = true;

                            Rock rockScript = hitCollider.GetComponent<Rock>();
                            if (rockScript != null)
                            {
                                // Always bump after pushing the rock
                                StartCoroutine(BumpAnimation());
                            }
                            else
                            {
                                // For other interactables, bump too
                                StartCoroutine(BumpAnimation());
                                Debug.Log("Blocked by wall or unknown obstacle");
                            }
                        }
                    }

                    if (!reacted)
                    {
                        // Nothing to interact with — wall or edge
                        StartCoroutine(BumpAnimation());
                        Debug.Log("Blocked by wall or unknown obstacle");
                    }
                }


                inputLocked = false;
            }
            else
            {
                //idle
                animator.SetBool("isMoving", false);

            }
        }

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
            StartCoroutine(ApplyFacingNextFrame(face));
        }
        else
        {
            Debug.LogWarning("Pangolin start position not assigned in LevelData!");
            startingPosition = transform.position;
            targetPosition = transform.position;

            // Safe default
            StartCoroutine(ApplyFacingNextFrame(PangolinStartPoint.FacingDirection.Right));
        }
    }

    private IEnumerator ApplyFacingNextFrame(PangolinStartPoint.FacingDirection facing)
    {
        // Wait a frame so Animator finishes re-binding after prefab spawn
        yield return null;
        ForceFacing(facing);
    }



    private IEnumerator moveToPosition(Vector3 destination)
    {
        isMoving = true;

        // move smoothly to the destination
        while ((transform.position - destination).sqrMagnitude > 0.001f)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = destination;
        isMoving = false;

        animator.SetBool("isMoving", false);

    }

    private IEnumerator InputCooldown()
    {
        inputLocked = true;
        yield return new WaitForSeconds(inputCooldown);
        inputLocked = false;
    }
    int GetTailAngleIndex(Vector3 previousDir, Vector3 currentDir)
    {
        // If no previous direction (e.g., first move), tail is straight
        if (previousDir == Vector3.zero)
            return 0;

        // If same direction, tail stays straight
        if (previousDir == currentDir)
            return 0;

        // Determine relative tail curve based on previous move vs current move
        // Tail curves opposite the direction the pangolin just came from

        if (currentDir == Vector3.up)
        {
            if (previousDir == Vector3.right)
                return 1; // tail curves left (index 1)
            if (previousDir == Vector3.left)
                return 2; // tail curves right (index 2)
        }
        else if (currentDir == Vector3.down)
        {
            if (previousDir == Vector3.right)
                return 2; // tail curves right (index 2)
            if (previousDir == Vector3.left)
                return 1; // tail curves left (index 1)
        }
        else if (currentDir == Vector3.right)
        {
            if (previousDir == Vector3.up)
                return 2; // tail curves downwards (index 2)
            if (previousDir == Vector3.down)
                return 1; // tail curves upwards (index 1)
        }
        else if (currentDir == Vector3.left)
        {
            if (previousDir == Vector3.up)
                return 2; // tail curves downwards (index 2)
            if (previousDir == Vector3.down)
                return 1; // tail curves upwards (index 1)
        }

        // Default tail straight
        return 0;
    }



    private IEnumerator BumpAnimation()
    {
        PlayBumpSound();

        Vector3 originalPos = transform.position;
        float bumpDistance = 0.15f;
        float bumpSpeed = 0.05f;

        // Move back opposite to the bump direction
        transform.position = originalPos - lastBumpDirection.normalized * bumpDistance;
        yield return new WaitForSeconds(bumpSpeed);

        // Move forward (return to original)
        transform.position = originalPos;
        yield return new WaitForSeconds(bumpSpeed);



        // check if we are out of moves after bump
        if (movesManager.movesLeft <= 0 && !outOfMovesTriggered)
        {
            outOfMovesTriggered = true;
            animator.SetTrigger("Sleep");
            inputLocked = true; // lock further movement
        }
    }


    private void PlayBumpSound()
    {
        if (bumpSound != null)
        {
            SFXManager.instance.PlaySFX(bumpSound);
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Soul") && !LevelManager.Instance.levelCompleted)
        {
            //reachedFirefly = true; 

            LevelManager.Instance.MarkLevelCompleted();
            Debug.Log("Soul reached!");

            // Play your "looking up at firefly" animation
            animator.SetTrigger("LookUp");

            LevelManager.Instance.LoadNextLevelWithDelay(1.5f);
        }
        else if (other.CompareTag("Mushroom"))
        {
            HandleMushroom(other);
        }
    }


    private void HandleMushroom(Collider2D mushroomCollider)
    {
        Debug.Log("Stepped on Mushroom!");

        // Subtract 2 moves when stepping on a mushroom
        movesManager.ModifyMoves(-2);

        // Hide the mushroom by disabling its sprite
        SpriteRenderer mushroomSprite = mushroomCollider.GetComponent<SpriteRenderer>();
        if (mushroomSprite != null)
        {
            mushroomSprite.enabled = false;  // Hide the mushroom's sprite
        }

        // Optionally, disable the collider to prevent further interactions
        Collider2D mushroomColliderComponent = mushroomCollider.GetComponent<Collider2D>();
        if (mushroomColliderComponent != null)
        {
            mushroomColliderComponent.enabled = false;  // Disable the collider
        }
    }


    public void ForceFacing(PangolinStartPoint.FacingDirection facing)
    {
        if (!animator) return;

        // Clear any leftover state from previous level
        animator.Rebind();
        animator.Update(0f);

        float mx = 0f, my = 0f;
        string idleState = "SideIdle"; // change if your state is named differently

        switch (facing)
        {
            case PangolinStartPoint.FacingDirection.Right:
                mx = 1f; my = 0f; spriteRenderer.flipX = false; idleState = "SideIdle"; break;
            case PangolinStartPoint.FacingDirection.Left:
                mx = -1f; my = 0f; spriteRenderer.flipX = true; idleState = "SideIdle"; break;
            case PangolinStartPoint.FacingDirection.Up:
                mx = 0f; my = 1f; spriteRenderer.flipX = false; idleState = "UpIdle"; break;
            case PangolinStartPoint.FacingDirection.Down:
                mx = 0f; my = -1f; spriteRenderer.flipX = false; idleState = "DownIdle"; break;
        }

        animator.SetFloat("moveX", mx);
        animator.SetFloat("moveY", my);
        animator.SetFloat("tailAngleIndex", 0f);
        animator.SetBool("isMoving", false);

        // Snap to the correct idle clip immediately
        animator.Play(idleState, 0, 0f);
        animator.Update(0f);
    }

    public void ResetLevelFlags()
    {
        reachedFirefly = false;
        outOfMovesTriggered = false;
        inputLocked = false;
    }

}