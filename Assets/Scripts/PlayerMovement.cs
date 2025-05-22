using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public Tilemap obstacleTilemap;
    public float moveSpeed = 20f;

    private Vector3 targetPosition;
    private bool isMoving = false;

    private void Start()
    {
        targetPosition = transform.position;
    }

    private void Update()
    {
        if (!isMoving)
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
                Vector3 nextPos = targetPosition + direction;

                if (isTileWalkable(nextPos))
                {
                    targetPosition = nextPos;
                    StartCoroutine(moveToPosition(targetPosition));
                }
            }
        }
    }

    private bool isTileWalkable(Vector3 targetPos)
    {
        // convert world position to tilemap cell position
        Vector3Int tilePosition = obstacleTilemap.WorldToCell(targetPos);
        // get the tile at the cell position
        TileBase tile = obstacleTilemap.GetTile(tilePosition);
        // return true if no tile exists (meaning it's walkable)
        return tile == null;
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
}
