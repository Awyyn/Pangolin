using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using Unity.Burst.CompilerServices;

public class PlayerMovement : MonoBehaviour
{ 
    public MovesManager movesManager; 
    public Button button; 

    public Tilemap groundTilemap;
    public Tilemap obstacleTilemap;
    public float moveSpeed = 10f;

    private Vector3 targetPosition;
    private Vector3 startingPosition;
    private bool isMoving = false;
    private Vector3 lastBumpDirection;
    private bool levelCompleted = false;

    public AudioSource bumpAudioSource;

    private void Start()
    {
        startingPosition = transform.position;
        targetPosition = transform.position;
        movesManager.ResetMoves(22);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.R)) //restart the level
        {
            RestartGame();  
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

                movesManager.ModifyMoves(-1);

                if (GridManager.Instance.CanMoveTo(nextPos))
                {
                    // move is allowed
                    targetPosition = nextPos;
                    StartCoroutine(moveToPosition(targetPosition));
                }
                else
                {
                    // move is blocked → check interactables first
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
                            if (rockScript != null && rockScript.rockBlocked)
                            {
                                StartCoroutine(BumpAnimation());
                                PlayBumpSound();
                            }
                        }
                    }

                    // If no interaction handled it → do bump by default
                    if (!reacted)
                    {
                        StartCoroutine(BumpAnimation());
                        PlayBumpSound();
                        Debug.Log("Blocked by wall or unknown obstacle");
                    }
                }
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
        if (other.CompareTag("Raspberry") && !levelCompleted)
        {
            levelCompleted = true;
            StartCoroutine(LevelCompleteSequence());
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
    private IEnumerator LevelCompleteSequence()
    {
        Debug.Log("Level Completed!");

        // Play small animation here (e.g., player celebration)
        // For simplicity, let's just wait 1 second now
        yield return new WaitForSeconds(1f);

        // TODO: Load next level (future implementation)
        Debug.Log("Next level loading... (to be implemented)");
    }
    public void RestartGame()
    {
        movesManager.ResetMoves(21);
        transform.position = startingPosition;
        targetPosition = startingPosition;
        isMoving = false;

        // Reset all IInteractable objects in the scene
        foreach (var interactable in FindObjectsOfType<MonoBehaviour>(true))
        {
            if (interactable is IInteractable resettable)
            {
                resettable.ResetState();
            }
        }

        Debug.Log("Game Restarted!");
    }


}
