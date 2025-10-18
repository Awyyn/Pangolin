using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }
    private PlayerMovement playerMovement;

    public Tilemap groundTilemap;
    public Tilemap obstacleTilemap;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        playerMovement = Object.FindFirstObjectByType<PlayerMovement>();

    }


    public bool CanMoveTo(Vector3 worldPosition, Collider2D ignore = null)
    {
        playerMovement.inputLocked = true;

        if (groundTilemap == null || obstacleTilemap == null)
            return false;

        Vector3Int cellPos = groundTilemap.WorldToCell(worldPosition);

        // No ground tile -> can’t move
        if (groundTilemap.GetTile(cellPos) == null)
            return false;

        // Obstacle tile -> can’t move
        if (obstacleTilemap.GetTile(cellPos) != null)
            return false;

        // Check for solid objects
        Collider2D[] colliders = Physics2D.OverlapPointAll(worldPosition);
        foreach (var col in colliders)
        {
            if (col == ignore) continue; //ignore self

            // ignores triggers and the player itself
            if (!col.isTrigger && !col.CompareTag("Player"))
            {
                Debug.Log($"[GridManager] Blocked by {col.name}");
                return false;
            }
        }

        return true;
    }



}
