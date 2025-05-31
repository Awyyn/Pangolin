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

                if (CanMoveTo(nextPos))
                {
                    targetPosition = nextPos;
                    StartCoroutine(moveToPosition(targetPosition));
                }
                else
                {
                    Collider2D hitCollider = Physics2D.OverlapPoint(nextPos); // check for interactable object at nextPos

                    if (hitCollider != null)
                    {
                        if (hitCollider.CompareTag("Rock"))
                        {
                            Vector3 rockNextPos = nextPos + direction;

                            if (CanMoveTo(rockNextPos))
                            {
                                // push the rock smoothly
                                StartCoroutine(MoveRock(hitCollider.gameObject, rockNextPos, moveSpeed));

                                // optionally play push sound/animation here                                                  TO-DO
                            }
                            else
                            {
                                // rock can't be pushed
                                StartCoroutine(BumpAnimation());
                                PlayBumpSound();
                                Debug.Log("Rock can't move further! Blocked");
                            } //bump by rock
                        }
                        else if (hitCollider.CompareTag("LeafPile"))
                        {
                            Vector3 leafNextPos = nextPos + direction;

                            if (CanMoveTo(leafNextPos))
                            {
                                // Trigger fade + smooth move to next tile
                                LeafPile leafScript = hitCollider.GetComponent<LeafPile>();
                                if (leafScript != null)
                                {
                                    float fadeDuration = 1.0f;  // seconds to fade & move
                                    leafScript.FadeOut(leafNextPos, fadeDuration);
                                }
                                // player does NOT move automatically here -> must move next turn manually
                            }
                            else
                            {
                                StartCoroutine(BumpAnimation());
                                PlayBumpSound();
                                Debug.Log("Blocked by leaf pile!");
                            }//bump by leaf
                        }
                        else
                        {
                            StartCoroutine(BumpAnimation());
                            PlayBumpSound();
                            Debug.Log("Blocked by another object!");
                        }//bump - blocked by tilemap obstacle or nothing detected 
                    }
                    else 
                    {
                        StartCoroutine(BumpAnimation());
                        PlayBumpSound();
                        Debug.Log("Blocked by tilemap obstacle!");
                    }// No object detected, but the player is blocked by a tilemap obstacle
                }
            }
        }
    }

    private bool CanMoveTo(Vector3 targetPos) //checking if the GAME OBJECT can get to the next tile
    {        
                // converts world position to tilemap cell position
        Vector3Int cellPos = groundTilemap.WorldToCell(targetPos);

        TileBase groundTile = groundTilemap.GetTile(cellPos);
        if (groundTile == null)
            return false;

        TileBase obstacleTile = obstacleTilemap.GetTile(cellPos);
        if (obstacleTile != null)
            return false;

        // Check if rock is blocking
        Collider2D[] colliders = Physics2D.OverlapPointAll(targetPos);
        foreach (var col in colliders)
        {
            if (col.CompareTag("Rock") || col.CompareTag("LeafPile"))
                return false;
        }

        return true;
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

    private IEnumerator MoveRock(GameObject rock, Vector3 destination, float speed)
    {
        while ((rock.transform.position - destination).sqrMagnitude > 0.001f)
        {
            rock.transform.position = Vector3.MoveTowards(rock.transform.position, destination, speed * Time.deltaTime);
            yield return null;
        }
        rock.transform.position = destination;
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
    public void RestartGame()                                           //RESTART
    {
        // Reset moves
        movesManager.ResetMoves(21);

        // Reset player position
        transform.position = startingPosition;
        targetPosition = startingPosition;
        isMoving = false;

        // Reset rocks
        foreach (Rock rock in FindObjectsOfType<Rock>())
        {
            rock.ResetRock();
        }

        // Reset mushrooms
        foreach (Mushroom mushroom in FindObjectsOfType<Mushroom>())
        {
            mushroom.ResetMushroom();
        }

        // Reset leaf piles (only if still present)
        foreach (LeafPile leaf in FindObjectsOfType<LeafPile>(true))
        {
            leaf.ResetLeafPile();
        }

        Debug.Log("Game Restarted!");
    }
}
