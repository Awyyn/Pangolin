using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public int movesLeft = 20;  // COUNTER FOR MOVES LEFT (will move to UI sometime)                                TO DO
    public MovesLeftDisplay movesLeftDisplay;
    public Button button; 

    public Tilemap groundTilemap;
    public Tilemap obstacleTilemap;
    public float moveSpeed = 20f;

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
        movesLeftDisplay.SetMovesLeft(movesLeft);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.R)) //restart the level
        {
            RestartGame();  
        }

        if (!isMoving && movesLeft > 0)
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

                movesLeft--;  // decreases moves here!
                movesLeftDisplay.SetMovesLeft(movesLeft);  // updates UI display

                Vector3 nextPos = targetPosition + direction;

                if (CanMoveTo(nextPos))
                {
                    targetPosition = nextPos;
                    StartCoroutine(moveToPosition(targetPosition));
                }
                else
                {
                    Debug.Log("Bump animation played");
                    StartCoroutine(BumpAnimation());
                    PlayBumpSound();
                    Debug.Log("Blocked!");
                }
            }

        }
    }

    private bool CanMoveTo(Vector3 targetPos) //checking if the player can get to the next tile
    {        
                // converts world position to tilemap cell position
        Vector3Int cellPos = groundTilemap.WorldToCell(targetPos);

        TileBase groundTile = groundTilemap.GetTile(cellPos);
        if (groundTile == null)
            return false;

        TileBase obstacleTile = obstacleTilemap.GetTile(cellPos);
        if (obstacleTile != null)
            return false;

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


    /*public void RestartGame()
    {
        Debug.Log("Restart button pressed!");
        // Reset moves
        movesLeft = 20;
        movesLeftDisplay.SetMovesLeft(movesLeft);  // Update UI

        // Reset player position (if you want to reset the player to the starting position)
        transform.position = startingPosition;  // Or any other start position

        // You can also reset other game-related stuff here if needed
        targetPosition = startingPosition;
        isMoving = false;  // Ensure the player isn't moving when restarting
        Debug.Log("Game Restarted!");
    }*/

    public void RestartGame()
    {
        Debug.Log("Restart button clicked!");

        // Debugging the state of the button and UIManager
       // Debug.Log("Button Interactable: " + GetComponent<Button>().interactable);
       // Debug.Log("UIManager position: " + transform.position);

        // Reset moves
        movesLeft = 20;
        movesLeftDisplay.SetMovesLeft(movesLeft);  // Update UI

        // Reset player position
        transform.position = startingPosition;
        targetPosition = startingPosition;

        // Reset movement state
        isMoving = false;

        Debug.Log("Game Restarted!");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Raspberry") && !levelCompleted)
        {
            levelCompleted = true;
            StartCoroutine(LevelCompleteSequence());
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

}
