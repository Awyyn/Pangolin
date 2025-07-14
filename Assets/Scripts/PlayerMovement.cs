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
    public MovesManager movesManager;  // Reference to the MovesManager
    public LevelManager levelManager;  // Reference to the LevelManager

    public Button button; 

    public Tilemap groundTilemap;
    public Tilemap obstacleTilemap;
    public float moveSpeed = 10f;

    private Vector3 targetPosition;
    private Vector3 startingPosition;
    private bool isMoving = false;
    private Vector3 lastBumpDirection;
    private bool levelCompleted = false;
    //private Vector3 previousDirection;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    public AudioSource bumpAudioSource;



    private void Start()
    {
        if (levelManager == null)
            levelManager = LevelManager.Instance;
        if (movesManager == null)//optional? idk if needed
            movesManager = FindObjectOfType<MovesManager>();


        startingPosition = transform.position;
        targetPosition = transform.position;

        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        //previousDirection = Vector3.zero;

    }

    private void Update()
    {

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
                lastBumpDirection = direction;  // save direction for bump animation
                Vector3 nextPos = targetPosition + direction;

                // Decrease moves left on each move
                movesManager.ModifyMoves(-1); // Decrease moves left after a move

                if (GridManager.Instance.CanMoveTo(nextPos)) //IF CAN MOVE
                {
                    //move is allowed

                    if (direction == Vector3.left || direction == Vector3.right)
                    {
                        animator.SetBool("isMovingSide", true);
                        animator.SetBool("isMovingTop", false);
                        animator.SetBool("isIdle", false);
                        animator.SetBool("isSide", true);

                        // Flip sprite in the correct direction
                        spriteRenderer.flipX = direction == Vector3.left;
                        spriteRenderer.flipY = direction == Vector3.up;
                    }
                    else if (direction == Vector3.up || direction == Vector3.down)
                    {
                        animator.SetBool("isMovingTop", true);
                        animator.SetBool("isMovingSide", false);
                        animator.SetBool("isIdle", false);
                        animator.SetBool("isSide", false);

                        spriteRenderer.flipY = direction == Vector3.down;
                    }

                    // Execute the move
                    targetPosition = nextPos;
                    StartCoroutine(moveToPosition(targetPosition));
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
            }
            else
            {
                //idle
                animator.SetBool("isMovingSide", false);
                animator.SetBool("isMovingTop", false);
                animator.SetBool("isIdle", true);
            }
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

        animator.SetBool("isMovingSide", false);
        animator.SetBool("isMovingTop", false);
        animator.SetBool("isIdle", true);
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
        if (other.CompareTag("Soul") && !levelCompleted)    //If pleyer touched Soul, the level is      COMPLETED
        {
            levelCompleted = true;
            Debug.Log("Soul touched!");
            LevelManager.Instance.LoadNextLevelWithDelay(2f);
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
        animator.SetBool("isMovingSide", false);
        animator.SetBool("isMovingTop", false);
        animator.SetBool("isIdle", true);
        animator.SetBool("isSide", true);

        levelManager.ResetLevel(); // <-- calls SetLevelMoves + movesManager.ResetMoves

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
