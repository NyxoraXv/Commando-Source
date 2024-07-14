using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class ProceduralLevelGenerator : MonoBehaviour
{
    public Tilemap tilemap;
    public TileBase grassTile;
    public TileBase undergroundTile;
    public TileBase grass45LeftTile;
    public TileBase grass45RightTile;
    public TileBase waterTile; // New water tile
    public GameObject enemyPrefab; // Enemy prefab
    public GameObject winTriggerPrefab; // Win trigger prefab
    public int seed = 0;
    public int width = 20;
    public int height = 30; // Increased height to accommodate underground
    public int totalEnemies = 5;
    public float noiseScale = 0.1f;
    public float heightMultiplier = 5.0f;
    public int undergroundDepth = 20; // Fixed depth for underground layer
    public int maxCanyonCount = 3; // Maximum number of canyons to place
    public int canyonDepth = 10; // Depth of the canyon
    public float hillFrequency = 0.05f; // Frequency of hills
    public float valleyFrequency = 0.05f; // Frequency of valleys
    public float plateauFrequency = 0.05f; // Frequency of plateaus

    // New tile variables
    public TileBase leftTopCornerTile;
    public TileBase rightTopCornerTile;
    public TileBase leftBottomCornerTile;
    public TileBase rightBottomCornerTile;
    public TileBase leftEdgeTile;
    public TileBase rightEdgeTile;
    public TileBase bottomTile;

    private System.Random prng;

    public void generate()
    {
        prng = new System.Random(seed); // Initialize random seed for reproducibility

        ClearTiles();
        clearEnemy();
        GenerateLevel();
        PlaceEnemies();
    }

    void clearEnemy()
    {
        GameObject[] enemy = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemyObj in enemy)
        {
            Destroy(enemyObj);
        }
    }

    void GenerateLevel()
    {
        float[,] noiseMap = GenerateNoiseMap(width, height, seed, noiseScale, heightMultiplier);
        int[] terrainHeights = new int[width];

        for (int x = 0; x < width; x++)
        {
            terrainHeights[x] = Mathf.FloorToInt(noiseMap[x, 0]) + undergroundDepth;
        }

        ModifyTerrain(terrainHeights);

        for (int x = 1; x < width; x++)
        {
            if (terrainHeights[x] - terrainHeights[x - 1] > 2)
            {
                terrainHeights[x] = terrainHeights[x - 1] + 2;
            }
        }

        for (int x = 0; x < width; x++)
        {
            int terrainY = terrainHeights[x];

            for (int y = 0; y < height; y++)
            {
                TileBase tileToPlace = null;

                if (y < undergroundDepth)
                {
                    tileToPlace = undergroundTile;
                }
                else if (y < terrainY)
                {
                    tileToPlace = undergroundTile;
                }
                else if (y == terrainY)
                {
                    tileToPlace = grassTile;
                }
                else if (y == terrainY + 1)
                {
                    if (x > 0 && terrainHeights[x - 1] > terrainY)
                    {
                        tileToPlace = grass45RightTile;
                    }
                    else if (x < width - 1 && terrainHeights[x + 1] > terrainY)
                    {
                        tileToPlace = grass45LeftTile;
                    }
                }

                if (tileToPlace != null)
                {
                    Vector3Int tilePosition = new Vector3Int(x, y, 0);
                    tilemap.SetTile(tilePosition, tileToPlace);
                }
            }
        }

        PlaceSpecialTiles(terrainHeights);

        PlaceCanyons(terrainHeights);
        PlaceWinTrigger(terrainHeights); // Place the win trigger after generating the terrain
    }

    void ModifyTerrain(int[] terrainHeights)
    {
        for (int x = 0; x < terrainHeights.Length; x++)
        {
            float hillNoise = Mathf.PerlinNoise(x * hillFrequency, seed);
            float valleyNoise = Mathf.PerlinNoise(x * valleyFrequency, seed);
            float plateauNoise = Mathf.PerlinNoise(x * plateauFrequency, seed);

            if (hillNoise > 0.6f)
            {
                int hillHeight = Mathf.FloorToInt(hillNoise * 5);
                for (int i = 0; i <= hillHeight; i++)
                {
                    if (x - i >= 0)
                    {
                        terrainHeights[x - i] = Mathf.Max(terrainHeights[x - i], terrainHeights[x] + hillHeight - i);
                    }
                }
            }
            else if (valleyNoise > 0.6f)
            {
                terrainHeights[x] -= Mathf.FloorToInt(valleyNoise * 5);
            }
            else if (plateauNoise > 0.6f)
            {
                terrainHeights[x] = Mathf.FloorToInt(plateauNoise * 10) + undergroundDepth;
            }
        }
    }

    void PlaceSpecialTiles(int[] terrainHeights)
    {
        for (int x = 0; x < width; x++)
        {
            int terrainY = terrainHeights[x];

            for (int y = terrainY; y < height; y++)
            {
                TileBase tileToPlace = null;

                if (tilemap.GetTile(new Vector3Int(x, y, 0)) == grassTile || tilemap.GetTile(new Vector3Int(x, y, 0)) == grass45LeftTile || tilemap.GetTile(new Vector3Int(x, y, 0)) == grass45RightTile)
                {
                    if (IsLeftTopCorner(x, y, terrainHeights))
                    {
                        tileToPlace = leftTopCornerTile;
                    }
                    else if (IsRightTopCorner(x, y, terrainHeights))
                    {
                        tileToPlace = rightTopCornerTile;
                    }
                    else if (IsLeftBottomCorner(x, y, terrainHeights))
                    {
                        tileToPlace = leftBottomCornerTile;
                    }
                    else if (IsRightBottomCorner(x, y, terrainHeights))
                    {
                        tileToPlace = rightBottomCornerTile;
                    }
                    else if (IsLeftEdge(x, y, terrainHeights))
                    {
                        tileToPlace = leftEdgeTile;
                    }
                    else if (IsRightEdge(x, y, terrainHeights))
                    {
                        tileToPlace = rightEdgeTile;
                    }
                    else if (IsBottom(x, y, terrainHeights))
                    {
                        tileToPlace = bottomTile;
                    }
                }

                if (tileToPlace != null)
                {
                    Vector3Int tilePosition = new Vector3Int(x, y, 0);
                    tilemap.SetTile(tilePosition, tileToPlace);
                }
            }
        }

        // Ensure corners are converted to edges if there's a slope above
        for (int x = 0; x < width; x++)
        {
            int terrainY = terrainHeights[x];
            for (int y = terrainY; y < height; y++)
            {
                if (tilemap.GetTile(new Vector3Int(x, y, 0)) == leftTopCornerTile && IsSlopeTopLeft(x, y, terrainHeights))
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), leftEdgeTile);
                    tilemap.SetTile(new Vector3Int(x, y - 1, 0), leftEdgeTile);
                }
                if (tilemap.GetTile(new Vector3Int(x, y, 0)) == rightTopCornerTile && IsSlopeTopRight(x, y, terrainHeights))
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), rightEdgeTile);
                    tilemap.SetTile(new Vector3Int(x, y - 1, 0), rightEdgeTile);
                }
            }
        }
    }

    void PlaceCanyons(int[] terrainHeights)
    {
        for (int i = 0; i < maxCanyonCount; i++)
        {
            int canyonWidth = 1; // Width between 1 and 2
            int canyonX = prng.Next(0, width - canyonWidth);
            int surfaceY = terrainHeights[canyonX];

            for (int x = canyonX; x < canyonX + canyonWidth; x++)
            {
                int waterY = surfaceY - canyonDepth;

                // Clear tiles above the water surface
                for (int y = surfaceY; y >= waterY; y--)
                {
                    Vector3Int tilePosition = new Vector3Int(x, y, 0);
                    tilemap.SetTile(tilePosition, null);
                }

                // Place water tiles at the waterY level
                Vector3Int waterPosition = new Vector3Int(x, waterY, 0);
                tilemap.SetTile(waterPosition, waterTile);
            }
        }

        // Remove tiles above water
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (tilemap.GetTile(new Vector3Int(x, y, 0)) == waterTile)
                {
                    for (int removeY = y + 1; removeY < height; removeY++)
                    {
                        tilemap.SetTile(new Vector3Int(x, removeY, 0), null);
                    }
                }
            }
        }
    }

    float[,] GenerateNoiseMap(int width, int height, int seed, float scale, float heightMultiplier)
    {
        float[,] noiseMap = new float[width, height];
        System.Random prng = new System.Random(seed);
        float offsetX = prng.Next(-100000, 100000);
        float offsetY = prng.Next(-100000, 100000);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float sampleX = x * scale + offsetX;
                float sampleY = y * scale + offsetY;

                float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * heightMultiplier;
                noiseMap[x, y] = perlinValue;
            }
        }

        return noiseMap;
    }

    public void ClearTiles()
    {
        tilemap.ClearAllTiles();
    }

    void PlaceEnemies()
    {
        List<Vector3Int> emptyPositions = new List<Vector3Int>();

        // Collect all empty tile positions
        BoundsInt bounds = tilemap.cellBounds;
        foreach (var position in bounds.allPositionsWithin)
        {
            Vector3Int localPlace = new Vector3Int(position.x, position.y, position.z);
            if (tilemap.HasTile(localPlace) && tilemap.GetTile(localPlace) == grassTile)
            {
                // Check the tile 2 blocks above the grass tile is empty and not on a slope tile
                Vector3Int abovePlace = new Vector3Int(localPlace.x, localPlace.y + 2, localPlace.z);
                if (!tilemap.HasTile(abovePlace) &&
                    tilemap.GetTile(new Vector3Int(localPlace.x, localPlace.y + 1, localPlace.z)) != grass45LeftTile &&
                    tilemap.GetTile(new Vector3Int(localPlace.x, localPlace.y + 1, localPlace.z)) != grass45RightTile)
                {
                    emptyPositions.Add(abovePlace);
                }
            }
        }

        // Ensure we don't place more enemies than available empty positions
        int enemiesToPlace = Mathf.Min(totalEnemies, emptyPositions.Count);

        // Place enemies randomly in empty positions using the seeded prng
        for (int i = 0; i < enemiesToPlace; i++)
        {
            int randomIndex = prng.Next(emptyPositions.Count);
            Vector3Int enemyPosition = emptyPositions[randomIndex];
            // Instantiate enemy prefab at the selected position
            Instantiate(enemyPrefab, tilemap.CellToWorld(enemyPosition) + new Vector3(0.5f, 0.5f, 0), Quaternion.identity);
            Debug.Log("Enemy placed at: " + enemyPosition);
            emptyPositions.RemoveAt(randomIndex);
        }
    }

    void PlaceWinTrigger(int[] terrainHeights)
    {
        // Check if the win trigger has already been placed
        if (GameObject.FindGameObjectWithTag("WinTrigger") != null)
        {
            Debug.Log("Win trigger already placed.");
            return; // Exit the method if the win trigger is already present
        }

        int winTriggerX = width - width / 6;
        int winTriggerY = terrainHeights[winTriggerX] + 1;

        Vector3Int winTriggerPosition = new Vector3Int(winTriggerX, winTriggerY, 0);
        Vector3 worldPosition = tilemap.CellToWorld(winTriggerPosition) + new Vector3(0.5f, 0.5f, 0);
        Instantiate(winTriggerPrefab, worldPosition, Quaternion.identity);
    }

    // Methods to check placement of corner and edge tiles
    bool IsLeftTopCorner(int x, int y, int[] terrainHeights)
    {
        return x > 0 && y == terrainHeights[x] && terrainHeights[x] > terrainHeights[x - 1] && !IsSlope(x - 1, y, terrainHeights);
    }

    bool IsRightTopCorner(int x, int y, int[] terrainHeights)
    {
        return x < width - 1 && y == terrainHeights[x] && terrainHeights[x] > terrainHeights[x + 1] && !IsSlope(x + 1, y, terrainHeights);
    }

    bool IsLeftBottomCorner(int x, int y, int[] terrainHeights)
    {
        return x > 0 && y == terrainHeights[x] - 1 && terrainHeights[x] < terrainHeights[x - 1] && !IsSlope(x - 1, y, terrainHeights);
    }

    bool IsRightBottomCorner(int x, int y, int[] terrainHeights)
    {
        return x < width - 1 && y == terrainHeights[x] - 1 && terrainHeights[x] < terrainHeights[x + 1] && !IsSlope(x + 1, y, terrainHeights);
    }

    bool IsLeftEdge(int x, int y, int[] terrainHeights)
    {
        return x > 0 && y < terrainHeights[x] && terrainHeights[x] < terrainHeights[x - 1] && !IsSlope(x - 1, y, terrainHeights);
    }

    bool IsRightEdge(int x, int y, int[] terrainHeights)
    {
        return x < width - 1 && y < terrainHeights[x] && terrainHeights[x] < terrainHeights[x + 1] && !IsSlope(x + 1, y, terrainHeights);
    }

    bool IsBottom(int x, int y, int[] terrainHeights)
    {
        return y == terrainHeights[x] - 1 && (x == 0 || x == width - 1 || y < terrainHeights[x - 1] && y < terrainHeights[x + 1]) && !IsSlope(x, y + 1, terrainHeights);
    }

    bool IsSlope(int x, int y, int[] terrainHeights)
    {
        return tilemap.GetTile(new Vector3Int(x, y, 0)) == grass45LeftTile || tilemap.GetTile(new Vector3Int(x, y, 0)) == grass45RightTile;
    }

    bool IsSlopeTopLeft(int x, int y, int[] terrainHeights)
    {
        return IsSlope(x - 1, y + 1, terrainHeights);
    }

    bool IsSlopeTopRight(int x, int y, int[] terrainHeights)
    {
        return IsSlope(x + 1, y + 1, terrainHeights);
    }

    bool IsGrassAboveSlope(int x, int y, int[] terrainHeights)
    {
        return !IsSlopeTopLeft(x, y, terrainHeights) && !IsSlopeTopRight(x, y, terrainHeights);
    }
}
