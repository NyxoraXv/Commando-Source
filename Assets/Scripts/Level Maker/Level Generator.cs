using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System;

public class ProceduralLevelGenerator : MonoBehaviour
{
    public Tilemap tilemap;
    public TileBase grassTile;
    public TileBase undergroundTile;
    public TileBase grass45LeftTile;
    public TileBase grass45RightTile;
    public TileBase waterTile; // New water tile
    public TileBase caveFloorTile; // New cave floor tile
    public TileBase caveWallTile; // New cave wall tile
    public GameObject[] enemyPrefabs; // Array of enemy prefabs
    public GameObject winTriggerPrefab; // Win trigger prefab
    public int totalSupplies;
    public GameObject medkitPrefab;
    public GameObject fireBoostPrefab;
    public GameObject ammoSupplyPrefab;
    public int seed = 0;
    public int width = 50;
    public int height = 30; // Increased height to accommodate underground
    public int totalEnemies = 10;
    public float noiseScale = 0.1f;
    public float heightMultiplier = 5.0f;
    public int undergroundDepth = 20; // Fixed depth for underground layer
    public int maxCanyonCount = 3; // Maximum number of canyons to place
    public int canyonDepth = 10; // Depth of the canyon
    public float hillFrequency = 0.05f; // Frequency of hills
    public float valleyFrequency = 0.05f; // Frequency of valleys
    public float plateauFrequency = 0.05f; // Frequency of plateaus
    public float caveFrequency = 0.1f; // Frequency of caves
    public float caveThreshold = 0.5f; // Threshold for cave generation

    // New tile variables
    public TileBase leftTopCornerTile;
    public TileBase rightTopCornerTile;
    public TileBase leftBottomCornerTile;
    public TileBase rightBottomCornerTile;
    public TileBase leftEdgeTile;
    public TileBase rightEdgeTile;
    public TileBase bottomTile;

    public GameObject playerPrefab; // Reference to the player prefab
    public Vector3 playerOffset = new Vector3(0.5f, 0.5f, 0); // Player offset

    private System.Random prng;
    private Vector3Int playerSpawnPosition;

    public LayerMask groundLayerMask;

    private void Start()
    {
        Mission_Data missionData = GenerativeMissionManager.instance.missionData;

        seed = missionData.seed;
        totalEnemies = missionData.total_enemy;
        totalSupplies = Convert.ToInt32(Math.Round(missionData.total_enemy / 1.5f));
        width = missionData.width;
        height = missionData.height;

        Vector3 enemyPosition = transform.position; // Assuming enemy's position
        Vector3 groundPosition = FindGroundPosition(enemyPosition);
        LayerMask groundLayerMask = LayerMask.GetMask("Walkable");

        Generate();
    }

    void Update()
    {
        Vector3 enemyPosition = transform.position; // Assuming enemy's position
        Vector3 groundPosition = FindGroundPosition(enemyPosition);
    }

    public void Generate()
    {
        prng = new System.Random(seed); // Initialize random seed for reproducibility

        ClearTiles();
        GenerateLevel();
        PlacePlayer(); // Place the player first
        PlaceEnemies();
        PlaceCollectibles();
    }

    void PlaceCollectibles()
    {
        List<Vector3Int> validPositions = new List<Vector3Int>();

        // Collect all valid positions for placing collectibles
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
                    validPositions.Add(abovePlace);
                }
            }
        }

        // Ensure we don't place more supplies than available valid positions
        int suppliesToPlace = Mathf.Min(totalSupplies, validPositions.Count);

        // Define the safe zone around the player's starting position
        Vector3Int playerStart = playerSpawnPosition;
        int safeZoneRadius = 3; // Adjust this value as needed

        // Determine the win trigger position
        int winTriggerX = width - width / 6;
        Vector3Int winTriggerPosition = new Vector3Int(winTriggerX, tilemap.cellBounds.yMin, 0);
        for (int y = tilemap.cellBounds.yMax; y >= tilemap.cellBounds.yMin; y--)
        {
            if (tilemap.HasTile(new Vector3Int(winTriggerX, y, 0)))
            {
                winTriggerPosition.y = y;
                break;
            }
        }

        // Place supplies randomly in valid positions using the seeded prng
        for (int i = 0; i < suppliesToPlace; i++)
        {
            if (validPositions.Count == 0)
                break;

            int randomIndex = prng.Next(validPositions.Count);
            Vector3Int supplyPosition = validPositions[randomIndex];

            // Ensure supplies are not placed near the win trigger position or the player's starting position
            if (supplyPosition.x >= winTriggerPosition.x - 2 ||
                Vector3Int.Distance(supplyPosition, playerStart) <= safeZoneRadius)
            {
                validPositions.RemoveAt(randomIndex);
                i--;
                continue;
            }

            // Randomly select a supply prefab to instantiate
            GameObject supplyPrefab = null;
            int supplyType = prng.Next(0, 3);
            switch (supplyType)
            {
                case 0:
                    supplyPrefab = medkitPrefab;
                    break;
                case 1:
                    supplyPrefab = fireBoostPrefab;
                    break;
                case 2:
                    supplyPrefab = ammoSupplyPrefab;
                    break;
            }

            if (supplyPrefab != null)
            {
                Instantiate(supplyPrefab, tilemap.CellToWorld(supplyPosition) + playerOffset, Quaternion.identity);
                Debug.Log("Supply placed at: " + supplyPosition);
            }
        }
    }

    void ClearEnemiesAndCollectibles()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] winTriggers = GameObject.FindGameObjectsWithTag("WinTrigger");
        GameObject[] player = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject enemy in enemies)
        {
            DestroyImmediate(enemy);
        }
        foreach (GameObject Player in player)
        {
            DestroyImmediate(Player);
        }

        foreach (GameObject winTrigger in winTriggers)
        {
            DestroyImmediate(winTrigger);
        }
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.layer == LayerMask.NameToLayer("Collectible"))
            {
                DestroyImmediate(obj);
            }
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

        // Ensure no cliff is higher than 1 block
        for (int x = 1; x < width; x++)
        {
            if (terrainHeights[x] - terrainHeights[x - 1] > 1)
            {
                terrainHeights[x] = terrainHeights[x - 1] + 1;
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
        PlaceCaves(terrainHeights); // Place caves after generating the terrain
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

        // Ensure hills are no higher than 1 block without a slope
        for (int x = 1; x < terrainHeights.Length; x++)
        {
            if (terrainHeights[x] - terrainHeights[x - 1] > 1)
            {
                terrainHeights[x] = terrainHeights[x - 1] + 1;
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

        // Replace grass tiles under slopes with dirt (underground) tiles
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3Int currentTilePosition = new Vector3Int(x, y, 0);
                if (tilemap.GetTile(currentTilePosition) == grass45LeftTile || tilemap.GetTile(currentTilePosition) == grass45RightTile)
                {
                    Vector3Int belowTilePosition = new Vector3Int(x, y - 1, 0);
                    if (tilemap.GetTile(belowTilePosition) == grassTile)
                    {
                        tilemap.SetTile(belowTilePosition, undergroundTile);
                    }
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

    void PlaceCaves(int[] terrainHeights)
    {
        float[,] caveNoiseMap = GenerateNoiseMap(width, undergroundDepth, seed + 1, caveFrequency, 1.0f);
        List<Vector3Int> caveEntrances = new List<Vector3Int>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < undergroundDepth; y++)
            {
                if (caveNoiseMap[x, y] > caveThreshold)
                {
                    Vector3Int tilePosition = new Vector3Int(x, y, 0);
                    tilemap.SetTile(tilePosition, null); // Clear tile to create cave
                }
            }
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < undergroundDepth; y++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);
                if (tilemap.GetTile(tilePosition) == null)
                {
                    // Place cave floor and wall tiles
                    if (y == 0 || tilemap.GetTile(new Vector3Int(x, y - 1, 0)) != null)
                    {
                        tilemap.SetTile(tilePosition, caveFloorTile);
                    }
                    else if (y == undergroundDepth - 1 || tilemap.GetTile(new Vector3Int(x, y + 1, 0)) != null)
                    {
                        tilemap.SetTile(tilePosition, caveWallTile);
                    }
                    else if (x == 0 || tilemap.GetTile(new Vector3Int(x - 1, y, 0)) != null)
                    {
                        tilemap.SetTile(tilePosition, caveWallTile);
                    }
                    else if (x == width - 1 || tilemap.GetTile(new Vector3Int(x + 1, y, 0)) != null)
                    {
                        tilemap.SetTile(tilePosition, caveWallTile);
                    }
                }
            }
        }

        // Ensure caves have entrances and create a one-way path
        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 0; y < undergroundDepth; y++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);
                if (tilemap.GetTile(tilePosition) == caveFloorTile)
                {
                    Vector3Int aboveTilePosition = new Vector3Int(x, y + 1, 0);
                    if (tilemap.GetTile(aboveTilePosition) == null)
                    {
                        tilemap.SetTile(aboveTilePosition, caveFloorTile);
                        caveEntrances.Add(tilePosition);
                    }
                }
            }
        }

        // Create a one-way path by placing walls above cave entrances
        foreach (var entrance in caveEntrances)
        {
            Vector3Int aboveEntrance = new Vector3Int(entrance.x, entrance.y + 1, 0);
            if (tilemap.GetTile(aboveEntrance) == caveFloorTile)
            {
                tilemap.SetTile(aboveEntrance, caveWallTile);
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
        ClearEnemiesAndCollectibles();
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
                // Check the tile 1 block above the grass tile is empty and not on a slope tile
                Vector3Int abovePlace = new Vector3Int(localPlace.x, localPlace.y + 1, localPlace.z);
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

        // Define the safe zone around the player's starting position
        Vector3Int playerStart = playerSpawnPosition;
        int safeZoneRadius = 3; // Adjust this value as needed

        // Determine the win trigger position
        int winTriggerX = width - width / 6;
        Vector3Int winTriggerPosition = new Vector3Int(winTriggerX, tilemap.cellBounds.yMin, 0);
        for (int y = tilemap.cellBounds.yMax; y >= tilemap.cellBounds.yMin; y--)
        {
            if (tilemap.HasTile(new Vector3Int(winTriggerX, y, 0)))
            {
                winTriggerPosition.y = y;
                break;
            }
        }

        // Place enemies randomly in empty positions using the seeded prng
        for (int i = 0; i < enemiesToPlace; i++)
        {
            if (emptyPositions.Count == 0)
                break;

            int randomIndex = prng.Next(emptyPositions.Count);
            Vector3Int enemyPosition = emptyPositions[randomIndex];

            // Ensure enemies are not placed too close to or behind the player
            if (Vector3Int.Distance(enemyPosition, playerStart) <= safeZoneRadius || enemyPosition.x <= playerStart.x)
            {
                emptyPositions.RemoveAt(randomIndex);
                i--;
                continue;
            }

            // Ensure enemies are not placed beyond the win trigger position
            if (enemyPosition.x >= winTriggerPosition.x)
            {
                emptyPositions.RemoveAt(randomIndex);
                i--;
                continue;
            }

            // Ensure enemies are not placed above water tiles (canyons)
            bool isAboveWater = false;
            for (int y = enemyPosition.y - 1; y >= enemyPosition.y - canyonDepth; y--)
            {
                Vector3Int tileBelow = new Vector3Int(enemyPosition.x, y, enemyPosition.z);
                if (tilemap.GetTile(tileBelow) == waterTile)
                {
                    isAboveWater = true;
                    break;
                }
            }

            if (isAboveWater)
            {
                emptyPositions.RemoveAt(randomIndex);
                i--;
                continue;
            }

            // Randomly select an enemy prefab from the array using the seeded prng
            GameObject enemyPrefab = enemyPrefabs[prng.Next(enemyPrefabs.Length)];

            // Instantiate enemy prefab at the selected position
            Instantiate(enemyPrefab, tilemap.CellToWorld(enemyPosition) + playerOffset, Quaternion.identity);
            Debug.Log("Enemy placed at: " + enemyPosition);

            // Skip the next 2 positions to maintain a 2-unit gap
            emptyPositions.RemoveAt(randomIndex);
            if (randomIndex < emptyPositions.Count)
            {
                emptyPositions.RemoveAt(randomIndex);
            }
            if (randomIndex < emptyPositions.Count)
            {
                emptyPositions.RemoveAt(randomIndex);
            }
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

        // Ensure winTriggerY is within the terrain height
        int winTriggerY = terrainHeights[winTriggerX];

        // Find a grass tile at or to the right of winTriggerX
        for (int x = winTriggerX; x < width; x++)
        {
            // Update winTriggerY to be the height at the current x position
            winTriggerY = terrainHeights[x];

            if (tilemap.GetTile(new Vector3Int(x, winTriggerY, 0)) == grassTile)
            {
                Vector3Int winTriggerPosition = new Vector3Int(x, winTriggerY + 1, 0);
                Vector3 worldPosition = tilemap.CellToWorld(winTriggerPosition) + new Vector3(0.5f, 0.5f, 0);
                Instantiate(winTriggerPrefab, worldPosition, Quaternion.identity);
                Debug.Log("Win trigger placed at: " + winTriggerPosition);
                return;
            }
        }

        Debug.LogWarning("No suitable grass area found for win trigger placement.");
    }

    void PlacePlayer()
    {
        if (playerPrefab == null || tilemap == null)
        {
            Debug.LogError("Player prefab or tilemap is not assigned.");
            return;
        }

        // Find the highest position in the tilemap to spawn the player
        playerSpawnPosition = FindHighestTilePosition();

        // Convert the tile position to world position and instantiate the player
        Vector3 worldPosition = tilemap.CellToWorld(playerSpawnPosition) + playerOffset; // Adjust position to center the player
        Instantiate(playerPrefab, worldPosition, Quaternion.identity);

        Debug.Log("Player spawned at: " + playerSpawnPosition);
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

    void MoveEnemyToGround()
    {
        Vector3 enemyPosition = transform.position; // Assuming enemy's position
        Vector3 groundPosition = FindGroundPosition(enemyPosition);
        // Move the enemy to groundPosition
        transform.position = groundPosition;
    }

    Vector3 FindGroundPosition(Vector3 position)
    {
        RaycastHit hit;
        if (Physics.Raycast(position, Vector3.down, out hit, Mathf.Infinity, groundLayerMask))
        {
            return hit.point;
        }
        return position; // Fallback to original position if no ground found
    }
}
