using UnityEngine;
using UnityEngine.Tilemaps;
using Cinemachine;
using System.Collections.Generic;

[RequireComponent(typeof(Tilemap))]
[RequireComponent(typeof(PolygonCollider2D))]
public class AutoPolygonCollider2D : MonoBehaviour
{
    public CinemachineConfiner cinemachineConfiner;
    public Tilemap tilemap;
    public int height = 50; // Height in blocks

    void Start()
    {
        PolygonCollider2D polygonCollider = GetComponent<PolygonCollider2D>();

        if (tilemap == null || polygonCollider == null)
        {
            Debug.LogError("Tilemap or PolygonCollider2D is missing.");
            return;
        }

        // Clear existing paths
        polygonCollider.pathCount = 0;

        // Generate collider paths from tilemap
        GenerateColliderFromTilemap(tilemap, polygonCollider);

        // Assign the collider to the Cinemachine Confiner
        if (cinemachineConfiner != null)
        {
            cinemachineConfiner.m_BoundingShape2D = polygonCollider;
            cinemachineConfiner.InvalidatePathCache();
        }
    }

    private void GenerateColliderFromTilemap(Tilemap tilemap, PolygonCollider2D polygonCollider)
    {
        BoundsInt bounds = tilemap.cellBounds;

        // Define the corner polygons based on the tilemap bounds
        Vector2 bottomLeft = tilemap.CellToWorld(new Vector3Int(bounds.xMin, bounds.yMin, 0));
        Vector2 bottomRight = tilemap.CellToWorld(new Vector3Int(bounds.xMax, bounds.yMin, 0));
        Vector2 topLeft = tilemap.CellToWorld(new Vector3Int(bounds.xMin, bounds.yMax, 0));
        Vector2 topRight = tilemap.CellToWorld(new Vector3Int(bounds.xMax, bounds.yMax, 0));

        float tileHeight = tilemap.cellSize.y * height;
        float tileWidth = tilemap.cellSize.x * (bounds.xMax - bounds.xMin);

        List<Vector2[]> paths = new List<Vector2[]>();

        // Bottom-left corner
        paths.Add(new Vector2[]
        {
            bottomLeft,
            new Vector2(bottomLeft.x, bottomLeft.y + tileHeight),
            new Vector2(bottomLeft.x + tileHeight, bottomLeft.y + tileHeight),
            new Vector2(bottomLeft.x + tileHeight, bottomLeft.y),
            bottomLeft
        });

        // Bottom-right corner
        paths.Add(new Vector2[]
        {
            bottomRight,
            new Vector2(bottomRight.x, bottomRight.y + tileHeight),
            new Vector2(bottomRight.x - tileHeight, bottomRight.y + tileHeight),
            new Vector2(bottomRight.x - tileHeight, bottomRight.y),
            bottomRight
        });

        // Top-left corner
        paths.Add(new Vector2[]
        {
            topLeft,
            new Vector2(topLeft.x, topLeft.y - tileHeight),
            new Vector2(topLeft.x + tileHeight, topLeft.y - tileHeight),
            new Vector2(topLeft.x + tileHeight, topLeft.y),
            topLeft
        });

        // Top-right corner
        paths.Add(new Vector2[]
        {
            topRight,
            new Vector2(topRight.x, topRight.y - tileHeight),
            new Vector2(topRight.x - tileHeight, topRight.y - tileHeight),
            new Vector2(topRight.x - tileHeight, topRight.y),
            topRight
        });

        // Bottom edge
        paths.Add(new Vector2[]
        {
            new Vector2(bottomLeft.x + tileHeight, bottomLeft.y),
            new Vector2(bottomRight.x - tileHeight, bottomRight.y),
            new Vector2(bottomRight.x - tileHeight, bottomRight.y + tileHeight),
            new Vector2(bottomLeft.x + tileHeight, bottomLeft.y + tileHeight),
            new Vector2(bottomLeft.x + tileHeight, bottomLeft.y)
        });

        // Top edge
        paths.Add(new Vector2[]
        {
            new Vector2(topLeft.x + tileHeight, topLeft.y),
            new Vector2(topRight.x - tileHeight, topRight.y),
            new Vector2(topRight.x - tileHeight, topRight.y - tileHeight),
            new Vector2(topLeft.x + tileHeight, topLeft.y - tileHeight),
            new Vector2(topLeft.x + tileHeight, topLeft.y)
        });

        // Assign the paths to the PolygonCollider2D
        polygonCollider.pathCount = paths.Count;
        for (int i = 0; i < paths.Count; i++)
        {
            polygonCollider.SetPath(i, paths[i]);
        }
    }
}
