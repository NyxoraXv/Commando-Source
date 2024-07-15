using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerSpawner : MonoBehaviour
{
    public Tilemap tilemap; // Reference to the tilemap
    public GameObject playerPrefab; // Reference to the player prefab

    private void Start()
    {
        SpawnPlayer();
    }
    public void SpawnPlayer()
    {
        if (playerPrefab == null || tilemap == null)
        {
            Debug.LogError("Player prefab or tilemap is not assigned.");
            return;
        }

        // Find the highest position in the tilemap to spawn the player
        Vector3Int spawnPosition = FindHighestTilePosition();

        // Convert the tile position to world position and instantiate the player
        Vector3 worldPosition = tilemap.CellToWorld(spawnPosition) + new Vector3(4f, 4f, 0); // Adjust position to center the player
        Instantiate(playerPrefab, worldPosition, Quaternion.identity);

        Debug.Log("Player spawned at: " + spawnPosition);
    }

    private Vector3Int FindHighestTilePosition()
    {
        BoundsInt bounds = tilemap.cellBounds;
        Vector3Int highestPosition = new Vector3Int(bounds.xMin, bounds.yMin, 0);

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMax; y >= bounds.yMin; y--)
            {
                Vector3Int position = new Vector3Int(x, y, 0);
                if (tilemap.HasTile(position))
                {
                    highestPosition = position;
                    return highestPosition; // Exit the loop as soon as we find the highest tile
                }
            }
        }

        return highestPosition;
    }
}
