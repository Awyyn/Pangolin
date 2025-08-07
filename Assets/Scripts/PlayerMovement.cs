using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using Unity.Burst.CompilerServices;

/// <summary>
/// Controls the player movement 
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance;

    public MovesManager movesManager;  // Reference to the MovesManager
    public LevelManager levelManager;  // Reference to the LevelManager


    public float moveSpeed = 10f;

    private Vector3 targetPosition;
    private Vector3 startingPosition;
    private bool isMoving = false;
    private Vector3 lastBumpDirection;


    private Vector3 lastDirection = Vector3.zero;
    private Vector3 previousDirection = Vector3.zero;
    private bool inputLocked = false;

    public float inputCooldown = 0.25f; // Cooldown time for input

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    public AudioSource bumpAudioSource;


    private void Awake() => instance = this;

    private void Start()
    {
        if (levelManager == null)
            levelManager = LevelManager.Instance;
        if (movesManager == null)
            movesManager = FindObjectOfType<MovesManager>();

        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Get starting position from level
        SetStartPositionFromLevel();
    }


    private void Update()
    {
        if (inputLocked || LevelManager.Instance.levelCompleted) return;


        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.R)) //restart the level
        {
            RestartGame();
        }

        if (movesManager.movesLeft <= 0)//if no moves are left, stop player movement
        {
            // Optionally, you can play a sound or animation indicating no moves left
            Debug.Log("No moves left!");
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
                    StartCoroutine(InputCooldown());
                }
                else //if the path is blocked by something
                {
                    // Handle when move is blocked
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
                            if (rockScript != null && !rockScript.rockBlocked)
                            {
                                // If rock can be moved, interact with it
                                rockScript.Interact(direction);
                            }
                            else
                            {
                                StartCoroutine(BumpAnimation());
                                PlayBumpSound();
                                Debug.Log("Blocked by wall or unknown obstacle");
                            }
                        }
                    }
                    if (!reacted)
                    {
                        // Play bump animation                                                       TODO
                        //   animator.SetTrigger("isBumped");
                        StartCoroutine(BumpAnimation());
                        PlayBumpSound();
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

    public void SetStartPositionFromLevel()
    {
        LevelData levelData = FindObjectOfType<LevelData>();
        if (levelData != null && levelData.pangolinStartPoint != null)
        {
            startingPosition = levelData.pangolinStartPoint.position;
            transform.position = startingPosition;
            targetPosition = startingPosition;
        }
        else
        {
            Debug.LogWarning("Pangolin start position not assigned in LevelData!");
            startingPosition = transform.position;
            targetPosition = transform.position;
        }
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
        Vector3 originalPos = transform.position;
        float bumpDistance = 0.1f;
        float bumpSpeed = 0.05f;

        // Move back opposite to the bump direction
        transform.position = originalPos - lastBumpDirection.normalized * bumpDistance;
        yield return new WaitForSeconds(bumpSpeed);

        // Move forward (return to original)
        transform.position = originalPos;
        yield return new WaitForSeconds(bumpSpeed);
    }


    private void PlayBumpSound()
    {
        if (bumpAudioSource != null)
        {
            bumpAudioSource.Play();
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Soul") && !LevelManager.Instance.levelCompleted)
        {
            LevelManager.Instance.MarkLevelCompleted();
            Debug.Log("soul reached!");
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


    public void RestartGame()
    {
        spriteRenderer.flipX = false;
        animator.SetBool("isMoving", false);
        animator.SetFloat("moveX", 0f);
        animator.SetFloat("moveY", 0f);
        animator.SetFloat("tailAngleIndex", 0f);

        // Move this line ABOVE transform.position reset
        SetStartPositionFromLevel(); // <-- sets startingPosition again from new level

        levelManager.ResetLevel(); // <-- resets the level & moves

        transform.position = startingPosition;
        targetPosition = startingPosition;
        isMoving = false;

        foreach (var interactable in FindObjectsOfType<MonoBehaviour>(true))
        {
            if (interactable is IInteractable resettable && resettable != null)
            {
                resettable.ResetState();
            }
        }

        Debug.Log("Game Restarted!");
    }


}
