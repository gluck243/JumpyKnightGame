using UnityEngine;

public class PlatformGenerator : MonoBehaviour
{
    [SerializeField] private GameObject ghostGuardPrefab; // Drag GhostGuard prefab here
    [SerializeField] private GameObject spiderPrefab;    // Drag Spider prefab here
    [SerializeField] private int ghostGuardsPerLevel = 2; // Number of monsters to spawn per level
    [SerializeField] private int spidersPerLevel = 2;    // Number of spiders to spawn per level
    [SerializeField] private float minimumSpiderSpacing = 4f; // Minimum spacing between spiders
    [SerializeField] private float minimumGuardEdgeDistance = 2f; // Minimum distance from platform edge for guards

    [SerializeField] private GameObject platformPrefab; // Drag platform prefab here
    [SerializeField] private GameObject wallPrefab; // Drag wall prefab here
    [SerializeField] private GameObject floorPrefab; // For the bottom floor
    [SerializeField] private GameObject backgroundPrefab; // Background prefab
    [SerializeField] private GameObject roofPrefab; // Roof prefab

    [SerializeField] private int numberOfPlatforms = 20; // Number of platforms to generate
    [SerializeField] private float verticalSpacing = 3f; // Vertical spacing between platforms
    [SerializeField] private float levelWidth = 18f; // Define the total width of the level
    [SerializeField] private LayerMask wallLayerMask; // Assign "Walls" layer here
    [SerializeField] private float wallBaseYOffset = 2f; // Offset to control wall's starting Y from PlatformGenerator Y
    
    private float highestPlatformY = 0f; // Variable to track the highest platform Y position
    private Transform leftWallBorder; // To store left wall transform for height adjustment
    private Transform rightWallBorder; // To store right wall transform for height adjustment
    private float levelFloorY; // Store the floor Y position
    [SerializeField] private int platformsPerLevel = 3; // Number of platforms per level

    [Header("Platform Count Distribution (Weighted Random)")] // Inspector Header
    [Range(0f, 1f)] public float probability_1Platform = 0.2f; // Probability of 1st platform level
    [Range(0f, 1f)] public float probability_2Platforms = 0.6f; // Probability of 2nd platform level

    [Header("Heart Container Spawning")] // Inspector Header
    [SerializeField] private GameObject heartContainerPrefab; // Drag heart prefab here
    [SerializeField] private int maxHeartContainerNumber = 3; // Maximum number of heart containers per level
    [Range(0f, 1f)] public float heartContainerSpawnProbability = 0.2f ; // Probability of spawning a heart container on a platform

    [Header("Top of the Tower")]
    public GameObject giantDoorPrefab; // Assign the Giant Door prefab here
    public string bossBattleSceneName = "BossBattle"; // Set boss battle scene name in Inspector

    void Start()
    {
        GenerateLevelBorders(); // Generate Walls and Floor first
        GeneratePlatforms(); // Then generate Platforms within the borders
        AdjustWallHeightsAndPosition(); // Adjust wall heights after platforms are generated
        SpawnGhostGuardsOnPlatforms(); // Spawn monsters on platforms
        SpawnSpidersOnWalls(); // Spawn spiders on walls
        GenerateTopOfTower(); // Generate the top of the tower
    }

    void GenerateLevelBorders() // Generates the side walls and floor
    {
        if (wallPrefab == null)
        {
            Debug.LogWarning("Wall Prefab is not assigned in PlatformGenerator!");
            return;
        }
        if (floorPrefab == null) // Optional floor prefab check
        {
            Debug.LogWarning("Floor Prefab is not assigned in PlatformGenerator! (Optional)");
        }

        // --- Generate Left Wall ---
        UnityEngine.Vector3 leftWallPosition = new UnityEngine.Vector3(-levelWidth / 2f - 0.5f - 50.5f, transform.position.y, 0); // Initial position
        UnityEngine.Vector3 leftWallScale = new UnityEngine.Vector3(75f, 10f, 1f); // Initial height
        GameObject leftWall = Instantiate(wallPrefab, leftWallPosition, UnityEngine.Quaternion.identity); // Create wall
        leftWall.transform.localScale = leftWallScale; // Set initial height
        leftWall.transform.parent = transform; // Set parent to PlatformGenerator
        SetWallLayer(leftWall); // Set "Walls" layer on wall
        leftWallBorder = leftWall.transform; // Store reference for later height adjustment

        // --- Generate Right Wall ---
        UnityEngine.Vector3 rightWallPosition = new UnityEngine.Vector3(levelWidth / 2f + 0.5f + 50.5f, transform.position.y, 0); // Initial position
        UnityEngine.Vector3 rightWallScale = new UnityEngine.Vector3(75f, 10f, 1f); // Initial height
        GameObject rightWall = Instantiate(wallPrefab, rightWallPosition, UnityEngine.Quaternion.identity);
        rightWall.transform.localScale = rightWallScale;
        rightWall.transform.parent = transform;
        SetWallLayer(rightWall);
        rightWallBorder = rightWall.transform;

        // Store references to walls for later height adjustment
        leftWallBorder = leftWall.transform;
        rightWallBorder = rightWall.transform;

        // Generate Floor
        float floorYPosition = transform.position.y - wallBaseYOffset; // Floor Y position - using wallBaseYOffset
        GameObject floor = null;
        if (floorPrefab != null) // Check if floorPrefab is assigned
        {
            UnityEngine.Vector3 floorPosition = new UnityEngine.Vector3(transform.position.x, floorYPosition, 0); // Floor position
            UnityEngine.Vector3 floorScale = new UnityEngine.Vector3(levelWidth + 2f, 1f, 1f); // Floor scale
            floor = Instantiate(floorPrefab, floorPosition, UnityEngine.Quaternion.identity); // Create floor
            floor.transform.localScale = floorScale; // Set floor scale
            floor.transform.parent = transform; // Set parent to PlatformGenerator
            SetPlatformLayer(floor); // Set "Ground" layer on floor
        }
        levelFloorY = floorYPosition; // Store floor Y position for wall positioning

        // Generate Background
        if (backgroundPrefab != null) // Assuming I have a background prefab
        {
            Vector3 backgroundPosition = new Vector3(transform.position.x, transform.position.y + (numberOfPlatforms * verticalSpacing) / 2f, 10f); // Position behind
            Vector3 backgroundScale = new Vector3(levelWidth, numberOfPlatforms * verticalSpacing * 1.5f, 1f); // Scale to cover tower
            GameObject background = Instantiate(backgroundPrefab, backgroundPosition, Quaternion.identity);
            background.transform.localScale = backgroundScale;
            background.transform.parent = transform;
        }
        else
        {
            Debug.LogWarning("Background Prefab not assigned!");
        }
    }

    void GeneratePlatforms()
    {
        UnityEngine.Vector2 spawnPosition = transform.position; // Start at PlatformGenerator position
        highestPlatformY = transform.position.y; // Initialize highestPlatformY with starting Y
        int maxPlatformsPerLevel = this.platformsPerLevel; // Maximum platforms per level

        float minPlatformSpacing = 4f; // Minimum desired spacing between platforms
        float availableWidthForPlatforms = levelWidth - (maxPlatformsPerLevel * minPlatformSpacing); // Use maxPlatformsPerLevel here for initial width check

        int currentHealthContainerCount = 0; // Track current heart container count for this level

        if (availableWidthForPlatforms < 0)
        {
            Debug.LogWarning("Level Width is too small to fit max " + maxPlatformsPerLevel + " platforms with " + minPlatformSpacing + "f spacing. Reducing max platforms per level to fit.");
            maxPlatformsPerLevel = Mathf.Max(1, Mathf.FloorToInt(levelWidth / minPlatformSpacing)); // Reduce maxPlatformsPerLevel
            availableWidthForPlatforms = Mathf.Max(0, levelWidth - (maxPlatformsPerLevel * minPlatformSpacing)); // Recalculate
        }

        for (int i = 0; i < numberOfPlatforms; i++) // Outer loop: Vertical Levels
        {
            // Calculating overall level progress, based on the outer loop index
            float overallLevelProgress = (float)i / (numberOfPlatforms - 1);
            float dynamicProbability = Mathf.Pow(overallLevelProgress, 2.7f) * 0.6f;

            int platformsPerLevel;
            if (maxPlatformsPerLevel <= 1) // If maxPlatformsPerLevel is 1 or less, just use 1 platform
            {
                platformsPerLevel = 1;
            }
            else
            {
                float randomValue = Random.value; // Random value between 0 and 1

                if (randomValue < probability_1Platform) // Check probability for 1 platform
                {
                    platformsPerLevel = 1;
                }
                else if (randomValue < probability_1Platform + probability_2Platforms) // Check probability for 2 platforms
                {
                    platformsPerLevel = 2;
                }
                else // Remaining probability is for 3 platforms (or maxPlatformsPerLevel if higher)
                {
                    platformsPerLevel = maxPlatformsPerLevel;
                }
            }

            // Debug.Log($"<color=yellow>Level Platforms: {platformsPerLevel}</color>"); // Debug log for platforms per level

            float availableWidthForPlatformsForLevel = levelWidth - (platformsPerLevel * minPlatformSpacing); // Recalculate available width with actual platformsPerLevel
            if (availableWidthForPlatformsForLevel < 0) availableWidthForPlatformsForLevel = 0; // Ensure non-negative

            if (platformsPerLevel <= 0) continue; // Skip level if no platforms to generate

            float totalPlatformWidth = levelWidth - availableWidthForPlatformsForLevel; // Calculate total width occupied by platforms including spacing
            float platformUnitWidth = totalPlatformWidth / platformsPerLevel; // Average width per platform unit (platform + spacing)

            // Generate platforms for this level
            spawnPosition.x = transform.position.x; // Reset X for each level start
            float currentLevelWidth = levelWidth; // Default level width

            if (platformsPerLevel == 2)
            {
                currentLevelWidth = levelWidth * 0.75f; // Reduce level width for 2 platforms
            }
            else
            {
                currentLevelWidth = levelWidth; // Use full level width for other platform counts
            }

            for (int j = 0; j < platformsPerLevel; j++)
            {
                // Calculate platform horizontal position
                float platformNormalizedPosition = (float)j / (platformsPerLevel - 1); // 0 to 1 normalized position within level

                if (platformsPerLevel == 1) // Special case: single platform in center
                {
                    spawnPosition.x = Random.Range(-levelWidth / 2f + 5f, levelWidth / 2f - 5f); // Random X within level bounds, edited, was 2.5f
                }
                else
                {
                    // Distribute platforms evenly across *currentLevelWidth*, centered
                    float basePlatformXPos = (-currentLevelWidth / 2f) + (currentLevelWidth * platformNormalizedPosition); // Evenly distributed position across *currentLevelWidth*
                    float randomXOffset = Random.Range(-platformUnitWidth / 2f, platformUnitWidth / 2f); // Increased randomness range
                    spawnPosition.x = basePlatformXPos + randomXOffset;
                    spawnPosition.x = Mathf.Clamp(spawnPosition.x, -currentLevelWidth / 2f + 2.5f, currentLevelWidth / 2f - 2.5f); // Clamp within *currentLevelWidth* bounds
                }

                SpawnPlatform(spawnPosition); // Spawn platform at spawnPosition
                
                if (currentHealthContainerCount < maxHeartContainerNumber)
                {
                    if (Random.value < dynamicProbability)
                    {
                        currentHealthContainerCount++;
                        UnityEngine.Vector3 heartSpawnPosition = new UnityEngine.Vector3(spawnPosition.x, spawnPosition.y + 0.4f, 0f);
                        if (heartContainerPrefab != null)
                        {
                            Instantiate(heartContainerPrefab, heartSpawnPosition, UnityEngine.Quaternion.identity);
                        }
                        else
                        {
                            Debug.LogWarning("Heart Container Prefab not assigned, skipping heart container spawn.");
                        }
                    }
                }
            }

            // Update highestPlatformY if the new platform is higher
            if (spawnPosition.y > highestPlatformY)
            {
                highestPlatformY = spawnPosition.y;
            }

            // Move spawnPosition up by verticalSpacing
            spawnPosition.y += verticalSpacing;
        }
        Debug.Log("<color=green>PlatformGenerator.GeneratePlatforms() COMPLETED</color>");
    }

    GameObject SpawnPlatform(UnityEngine.Vector2 position) // Spawns a platform at the given position
    {
        GameObject newPlatform = Instantiate(platformPrefab, position, UnityEngine.Quaternion.identity);
        newPlatform.transform.parent = transform;
        SetPlatformLayer(newPlatform);
        return newPlatform; // Return the instantiated platform GameObject so its scale can be modified
    }

    void AdjustWallHeightsAndPosition() // Adjusts wall heights and Y position based on highest platform
    {
        if (leftWallBorder != null && rightWallBorder != null)
        {
            float targetWallHeight = (highestPlatformY - transform.position.y) + verticalSpacing + 10f; // Calculate target height

            // Adjust Left Wall Height
            UnityEngine.Vector3 leftWallScale = leftWallBorder.localScale; // Get current scale
            leftWallScale.y = targetWallHeight;                     // Modify only the Y scale (height)
            leftWallBorder.localScale = leftWallScale;             // Set the modified scale back

            // Adjust Right Wall Height
            UnityEngine.Vector3 rightWallScale = rightWallBorder.localScale;
            rightWallScale.y = targetWallHeight;
            rightWallBorder.localScale = rightWallScale;

            // Adjust Wall Y Position to start at Floor
            float targetWallYPosition = levelFloorY + targetWallHeight / 2f; // Calculate Y position for wall center, starting from floor
            UnityEngine.Vector3 leftWallPosition = leftWallBorder.position;
            leftWallPosition.y = targetWallYPosition;
            leftWallBorder.position = leftWallPosition;

            UnityEngine.Vector3 rightWallPosition = rightWallBorder.position;
            rightWallPosition.y = targetWallYPosition;
            rightWallBorder.position = rightWallPosition;
        }
    }

    void SpawnGhostGuardsOnPlatforms() 
    {
        // Get all generated platforms
        GameObject[] platforms = GetPlatforms();
        if (platforms.Length == 0 || platforms == null)
        {
            Debug.LogWarning("No platforms found to spawn Ghost Guards on!");
            return; // Exit if no platforms exist
        }

        // Determine number of Ghost Guards to spawn for this level
        int numberOfGhostGuardsToSpawn = ghostGuardsPerLevel; // Use the serialized 'ghostGuardsPerLevel' variable

        // Loop to spawn Ghost Guards
        for (int i = 0; i < numberOfGhostGuardsToSpawn; i++)
        {
            // Randomly choose a platform to spawn on
            Transform selectedPlatform = platforms[Random.Range(0, platforms.Length)].transform;

            if (selectedPlatform == null || ghostGuardPrefab == null) // Safety checks
            {
                Debug.LogWarning("Platform or Ghost Guard Prefab not assigned, skipping Ghost Guard spawn.");
                continue; // Skip to next guard
            }

            // Calculate random X spawn position on platform
            SpriteRenderer platformRenderer = selectedPlatform.GetComponent<SpriteRenderer>();
            if (platformRenderer == null)
            {
                Debug.LogWarning("Platform SpriteRenderer missing, cannot calculate spawn position.");
                continue; // Skip if platform renderer is missing
            }
            Bounds platformBounds = platformRenderer.bounds;

            // X position calculation
            float platformXPosition;
            if ((platformBounds.max.x - platformBounds.min.x) <= (2 * minimumGuardEdgeDistance)) // Platform too narrow for edge distance?
            {
                platformXPosition = platformBounds.center.x; // Spawn at center if platform is too narrow
            }
            else
            {
                platformXPosition = Random.Range(platformBounds.min.x + minimumGuardEdgeDistance, platformBounds.max.x - minimumGuardEdgeDistance); // Random X within inset bounds
            }

            // Calculate Y spawn position
            float platformYPosition = platformBounds.max.y + 0.5f; // Slightly above platform top

            // Calculate Guard spawn position
            UnityEngine.Vector3 spawnPosition = new UnityEngine.Vector3(platformXPosition, platformYPosition, 0f);

            // Instantiate Ghost Guard prefab
            GameObject newGhostGuard = Instantiate(ghostGuardPrefab, spawnPosition, UnityEngine.Quaternion.identity);
            newGhostGuard.transform.parent = transform;

            // Debug.Log($"<color=teal>  Spawned Ghost Guard at position: {spawnPosition} on platform: {selectedPlatform.name}</color>");
        }
    }

    void SpawnSpidersOnWalls()
    {
        // Get references to the Left and Right Walls
        if (leftWallBorder == null || rightWallBorder == null)
        {
            Debug.LogWarning("Wall borders not initialized, cannot spawn Spiders.");
            return; // Exit if walls are not available
        }

        // Determine number of Spiders to spawn
        int numberOfSpidersToSpawn = spidersPerLevel;

        // Define wallWidth (match GenerateLevelBorders)
        float wallWidth = 1f; // For some reason works with 1f, even though the width is xf

        // Calculate Expected Wall X Positions
        float expectedLeftWallXPosition = transform.position.x - levelWidth / 2f - wallWidth / 2f;
        float expectedRightWallXPosition = transform.position.x + levelWidth / 2f + wallWidth / 2f;

        // Get list of existing Spiders for spacing check
        SpiderAI[] existingSpiders = FindObjectsByType<SpiderAI>(FindObjectsSortMode.None);

        // Loop to spawn Spiders
        for (int i = 0; i < numberOfSpidersToSpawn; i++)
        {
            // Randomly choose Left or Right wall to spawn on
            Transform selectedWallBorder = null;
            float wallXPosition = 0f;
            float spiderZRotation = 0f;

            bool isLeftWall = false;

            if (Random.Range(0f, 1f) < 0.5f) // Left Wall
            {
                selectedWallBorder = leftWallBorder;
                wallXPosition = expectedLeftWallXPosition;
                spiderZRotation = 270f;
                isLeftWall = true;
                // Debug.Log($"<color=blue>Spawning Spider on LEFT Wall - Initial Rotation: 270f</color>");
            }
            else // Right Wall
            {
                selectedWallBorder = rightWallBorder;
                wallXPosition = expectedRightWallXPosition;
                spiderZRotation = 90f;
                isLeftWall = false;
                // Debug.Log($"<color=blue>Spawning Spider on RIGHT Wall - Initial Rotation: 90f</color>");
            }

            if (selectedWallBorder == null || spiderPrefab == null) // Safety checks
            {
                Debug.LogWarning("Wall border or Spider Prefab not assigned, skipping Spider spawn.");
                continue; // Skip to next monster
            }

            // Calculate random Y spawn position along the Wall height
            Bounds wallBounds = selectedWallBorder.GetComponent<SpriteRenderer>().bounds;
            float randomWallYPosition = Random.Range(wallBounds.min.y + 1f, wallBounds.max.y - 5f); // Random Y within wall height (inset slightly)

            // Calculate Spider spawn position
            UnityEngine.Vector3 spawnPosition = new UnityEngine.Vector3(wallXPosition, randomWallYPosition, 0f);

            // Spider Spacing Check
            bool positionValid = true;
            foreach (SpiderAI existingSpider in existingSpiders)
            {
                if (UnityEngine.Vector2.Distance(spawnPosition, existingSpider.transform.position) < minimumSpiderSpacing)
                {
                    positionValid = false;
                    Debug.Log($"<color=orange>  Spider spawn position REJECTED due to spacing conflict with existing spider. Trying new Y position...</color>");
                    randomWallYPosition = Random.Range(wallBounds.min.y + 1f, wallBounds.max.y - 1f); // Try a new random Y
                    spawnPosition.y = randomWallYPosition; // Update spawnPosition with new Y
                    break; // Exit the foreach loop and re-check with potentially new position
                }
            }

            if (!positionValid) // If position is still invalid after trying a new Y
            {
                Debug.LogWarning("<color=red>  Could not find valid Spider spawn position after spacing checks. Skipping spawn for this spider.</color>");
                continue; // Skip to next spider
            }

            // Instantiate Spider prefab with Rotation
            GameObject newSpider = Instantiate(spiderPrefab, spawnPosition, UnityEngine.Quaternion.Euler(0f, 0f, spiderZRotation));
            newSpider.transform.parent = transform;
            SpiderAI spiderAIScript = newSpider.GetComponent<SpiderAI>();
            if (spiderAIScript != null)
            {
                spiderAIScript.isLeftWallSpider = isLeftWall;
            }

            // Debug.Log($"<color=teal>  Spawned Spider at position: {spawnPosition} on wall: {selectedWallBorder.name}, Rotation: {spiderZRotation}</color>");
        }
    }

    GameObject[] GetPlatforms() // Helper function to get all platform GameObjects
    {
        return GameObject.FindGameObjectsWithTag("Platform"); // Assumes platforms are tagged with "Platform"
    }

    void SetWallLayer(GameObject wallObject) // Sets "Walls" layer on wall GameObject
    {
        SpriteRenderer wallRenderer = wallObject.GetComponentInChildren<SpriteRenderer>();
        if (wallRenderer != null)
        {
            wallRenderer.gameObject.layer = LayerMask.NameToLayer("Walls");
        }
        else
        {
            Debug.LogWarning("No SpriteRenderer found on Wall Prefab to assign 'Walls' layer!");
        }
    }

    void SetPlatformLayer(GameObject platformObject) // Reusable function for setting "Ground" layer on platforms
    {
         SpriteRenderer platformRenderer = platformObject.GetComponentInChildren<SpriteRenderer>();
        if (platformRenderer != null)
        {
            platformRenderer.gameObject.layer = LayerMask.NameToLayer("Ground"); // Assuming platforms use "Ground" layer
        }
        else
        {
            Debug.LogWarning("No SpriteRenderer found on Platform Prefab to assign 'Ground' layer!");
        }
    }

    void GenerateTopOfTower()
    {
        float topYPosition = transform.position.y + (numberOfPlatforms - 1) * verticalSpacing + 6f; // Calculate top Y position
        float roomWidth = levelWidth; // Calculate room width as 80% of level width
        float wallHeight = 3f; // Set wall height

        // Floor
        Vector3 floorPosition = new Vector3(transform.position.x, topYPosition - wallHeight - 0.5f, 0f); // Position floor below walls
        Vector3 floorScale = new Vector3(roomWidth, 1f, 1f); // Scale floor to room width
        GameObject floor = Instantiate(floorPrefab, floorPosition, Quaternion.identity, transform); // Create floor
        floor.transform.localScale = floorScale; // Set floor scale
        SetPlatformLayer(floor); // Set "Ground" layer on floor

        // Roof
        if (roofPrefab != null)
        {
            Vector3 roofPosition = new Vector3(transform.position.x, topYPosition + 10.44f, 0f); // Position roof at top
            Vector3 roofScale = new Vector3(roomWidth + 10f, 20f, 1f); // Scale roof to room width
            GameObject roof = Instantiate(roofPrefab, roofPosition, Quaternion.identity, transform); // Create roof
            roof.transform.localScale = roofScale; // Set roof scale
            SetPlatformLayer(roof); // Set "Ground" layer on roof
        }
        else
        {
            Debug.LogWarning("Roof Prefab not assigned!");
        }

        // Giant Door
        if (giantDoorPrefab != null)
        {
            Vector3 doorPosition = new Vector3(transform.position.x, 1.955f + topYPosition - wallHeight / 2f - 0.5f, 0f); // Position door below walls
            GameObject door = Instantiate(giantDoorPrefab, doorPosition, Quaternion.identity, transform); // Create door
        }
        else
        {
            Debug.LogWarning("Giant Door Prefab not assigned!");
        }
    }
}
