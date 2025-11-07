using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// GridManager with safe auto-assignment of tilemaps from the currently loaded level instance.
/// This prevents startup-order issues when level-affine objects (rocks, etc.) call grid helpers in Start().
/// </summary>
public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [Header("Tilemaps (can be left unassigned — auto-detected at runtime)")]
    public Tilemap groundTilemap;
    public Tilemap obstacleTilemap;

    [Header("Map offset (assign MapRoot here if level scrolls)")]
    public Transform mapRoot; // optional; leave null for static camera

    [Tooltip("Optional offset to apply to CellToWorld results")]
    public Vector3 cellCenterOffset = Vector3.zero;

    private bool autoAssignedLogged = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Try to ensure tilemaps are available before any calls that need them.
    // Returns true if groundTilemap is valid after call.
    bool EnsureTilemaps()
    {
        if (groundTilemap != null) return true;

        // Try to pull from LevelManager's current level instance
        if (LevelManager.Instance != null && LevelManager.Instance.CurrentLevelInstance != null)
        {
            var levelRoot = LevelManager.Instance.CurrentLevelInstance.transform;
            var grid = levelRoot.Find("Grid");
            if (grid != null)
            {
                var ground = grid.Find("GroundTilemap")?.GetComponent<Tilemap>();
                var obstacle = grid.Find("ObstacleTilemap")?.GetComponent<Tilemap>();

                if (ground != null)
                {
                    groundTilemap = ground;
                    obstacleTilemap = obstacle; // may be null, that's ok

                    if (!autoAssignedLogged)
                    {
                        Debug.Log("[GridManager] Auto-assigned tilemaps from current level.");
                        autoAssignedLogged = true;
                    }

                    return true;
                }
            }
        }

        // nothing found
        return false;
    }

    public Vector3Int WorldToCell(Vector3 worldPosition)
    {
        if (!EnsureTilemaps())
        {
            Debug.LogWarning("[GridManager] WorldToCell called but groundTilemap is not assigned. Returning Vector3Int.zero.");
            return Vector3Int.zero;
        }

        Vector3 local = (mapRoot != null) ? worldPosition - mapRoot.position : worldPosition;
        return groundTilemap.WorldToCell(local);
    }

    public Vector3 CellToWorld(Vector3Int cell)
    {
        if (!EnsureTilemaps())
        {
            Debug.LogWarning("[GridManager] CellToWorld called but groundTilemap is not assigned. Returning Vector3.zero.");
            return Vector3.zero;
        }

        // Use GetCellCenterWorld to get center of tile
        Vector3 localCenter = groundTilemap.GetCellCenterWorld(cell);
        Vector3 world = (mapRoot != null) ? localCenter + mapRoot.position : localCenter;
        return world + cellCenterOffset;
    }

    public bool IsCellWalkable(Vector3Int cell)
    {
        if (!EnsureTilemaps()) return false;
        if (groundTilemap.GetTile(cell) == null) return false;
        if (obstacleTilemap != null && obstacleTilemap.GetTile(cell) != null) return false;
        return true;
    }

    // Backwards-compatible helper used by older code
    public bool CanMoveTo(Vector3 worldPosition, Collider2D ignore = null)
    {
        if (!EnsureTilemaps()) return false;

        Vector3Int cellPos = WorldToCell(worldPosition);
        if (groundTilemap.GetTile(cellPos) == null) return false;
        if (obstacleTilemap != null && obstacleTilemap.GetTile(cellPos) != null) return false;

        Vector3 checkWorld = CellToWorld(cellPos);
        Collider2D[] colliders = Physics2D.OverlapPointAll(checkWorld);
        foreach (var col in colliders)
        {
            if (col == null) continue;
            if (col == ignore) continue;
            if (!col.isTrigger && !col.CompareTag("Player"))
                return false;
        }

        return true;
    }
}
