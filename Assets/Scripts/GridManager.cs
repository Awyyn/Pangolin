using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    public Tilemap groundTilemap;
    public Tilemap obstacleTilemap;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public bool CanMoveTo(Vector3 worldPosition)
    {
        Vector3Int cellPos = groundTilemap.WorldToCell(worldPosition);

        TileBase groundTile = groundTilemap.GetTile(cellPos);
        if (groundTile == null)
            return false;

        TileBase obstacleTile = obstacleTilemap.GetTile(cellPos);
        if (obstacleTile != null)
            return false;

        Collider2D[] colliders = Physics2D.OverlapPointAll(worldPosition);
        foreach (var col in colliders)
        {
            if (col.CompareTag("Rock") || col.CompareTag("LeafPile"))
                return false;
        }

        return true;
    }
}
