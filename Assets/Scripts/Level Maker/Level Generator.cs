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
    public TileBase secondTile; // New tile for the second half of the level
    public TileBase secondUndergroundTile; // New underground tile for the second half of the level
    public TileBase secondGrass45RightTile; // New 45-degree left tile for the second half of the level
    public TileBase secondGrass45LeftTile; // New 45-degree right tile for the second half of the level
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
    public int slopeStartWidth = 10; // Width after which the slope starts
    public int slopeDepth = 10; // Depth of the slope
    public float hillFrequency = 0.05f; // Frequency of hills
    public float valleyFrequency = 0.05f; // Frequency of valleys
    public float plateauFrequency = 0.05f; // Frequency of plateaus


    private System.Random prng;

    public void generate()
    {
        prng = new System.Random(seed); // Initialize random seed for reproducibility
        
        
        ClearTiles();
        clearEnemy();
        ClearWinTrigger();
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

    void ClearWinTrigger()
    {
        GameObject winTrigger = GameObject.FindGameObjectWithTag("WinTrigger");
        if (winTrigger != null)
        {
            Destroy(winTrigger);
            Debug.Log("Win trigger destroyed.");
        }
        else
        {
            Debug.Log("No win trigger found to destroy.");
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

        PlaceTiles(terrainHeights);

        // Adjust terrain heights to create a downward slope after a certain width
        for (int x = slopeStartWidth; x < width; x++)
        {
            terrainHeights[x] -= (x - slopeStartWidth) * slopeDepth / (width - slopeStartWidth);
        }

        PlaceCanyons(terrainHeights);
        PlaceWinTrigger(terrainHeights); // Place the win trigger after generating the terrain
    }

    void PlaceTiles(int[] terrainHeights)
    {
        for (int x = 0; x < width; x++)
        {
            int terrainY = terrainHeights[x];
            TileBase currentTile = (x < width / 2) ? grassTile : secondTile; // Change tile halfway through the level
            TileBase currentUndergroundTile = (x < width / 2) ? undergroundTile : secondUndergroundTile; // Change underground tile halfway through the level
            TileBase currentGrass45LeftTile = (x < width / 2) ? grass45LeftTile : secondGrass45LeftTile; // Change left slope tile halfway through the level
            TileBase currentGrass45RightTile = (x < width / 2) ? grass45RightTile : secondGrass45RightTile; // Change right slope tile halfway through the level

            for (int y = 0; y < height; y++)
            {
                TileBase tileToPlace = null;

                if (y < undergroundDepth)
                {
                    tileToPlace = currentUndergroundTile;
                }
                else if (y < terrainY)
                {
                    tileToPlace = currentUndergroundTile;
                }
                else if (y == terrainY)
                {
                    tileToPlace = currentTile;
                }
                else if (y == terrainY + 1)
                {
                    if (x > 0 && terrainHeights[x - 1] > terrainY)
                    {
                        tileToPlace = currentGrass45RightTile;
                    }
                    else if (x < width - 1 && terrainHeights[x + 1] > terrainY)
                    {
                        tileToPlace = currentGrass45LeftTile;
                    }
                }

                if (tileToPlace != null)
                {
                    Vector3Int tilePosition = new Vector3Int(x, y, 0);
                    tilemap.SetTile(tilePosition, tileToPlace);
                }
            }
        }
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

    void PlaceCanyons(int[] terrainHeights)
    {
        for (int i = 0; i < maxCanyonCount; i++)
        {
            int canyonWidth = prng.Next(1, 3); // Width between 1 and 2
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
                Vector3Int tilePosition = new Vector3Int(x, y, 0);
                if (tilemap.GetTile(tilePosition) == waterTile)
                {
                    // Clear any tiles above the water
                    for (int aboveY = y + 1; aboveY < height; aboveY++)
                    {
                        Vector3Int aboveTilePosition = new Vector3Int(x, aboveY, 0);
                        tilemap.SetTile(aboveTilePosition, null);
                    }
                }
            }
        }
    }

    float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, float heightMultiplier)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];
        System.Random prng = new System.Random(seed);
        float offsetX = prng.Next(-100000, 100000);
        float offsetY = prng.Next(-100000, 100000);

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
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

        // Place the win trigger near the end of the level
        int winTriggerX = width - width / 5;

        // Set winTriggerY to be just above the terrain at winTriggerX
        int winTriggerY = terrainHeights[winTriggerX] + 1;

        Vector3Int winTriggerPosition = new Vector3Int(winTriggerX, winTriggerY, 0);
        Vector3 worldPosition = tilemap.CellToWorld(winTriggerPosition) + new Vector3(0.5f, 0.5f, 0);
        Instantiate(winTriggerPrefab, worldPosition, Quaternion.identity);

        Debug.Log("Win trigger placed at: " + winTriggerPosition);
    }
}
