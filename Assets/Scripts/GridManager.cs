using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    public Tilemap groundTilemap;
    public Tilemap obstacleTilemap;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public bool CanMoveTo(Vector3 worldPosition)
    {
        if (groundTilemap == null || obstacleTilemap == null)
            return false; // grid isn’t ready yet

        Vector3Int cellPos = groundTilemap.WorldToCell(worldPosition);

        if (groundTilemap.GetTile(cellPos) == null)
            return false;

        if (obstacleTilemap.GetTile(cellPos) != null)
            return false;

        foreach (var col in Physics2D.OverlapPointAll(worldPosition))
        {
            if (col.CompareTag("Rock") || col.CompareTag("LeafPile"))
                return false;
        }

        return true;
    }

}
