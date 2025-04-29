using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.MPE;
using UnityEngine;

public class PebbleSpawner : MonoBehaviour
{
    public GameObject pebblePrefab; // Pebble prefab to spawn
    public Transform[] platforms; // Platforms to spawn pebbles on
    private Bounds fireBounds; // Bounds of the fire
    private float nextSpawnTime; // Time to spawn the next pebble
    public float spawnRate = 0.5f; // Rate of spawning pebbles
    public float launchForceHorizontal = 3f; // Horizontal launch force of the pebble
    public float launchForceUpward = 5f; // Upward launch force of the pebble
    private float leftSidePebbles = 0; // Number of pebbles on the left side
    private float rightSidePebbles = 0; // Number of pebbles on the right side
    void Start()
    {
        ParticleSystem ps = GetComponent<ParticleSystem>(); // Get the ParticleSystem component
        if (ps != null && ps.shape.shapeType == ParticleSystemShapeType.Rectangle)
        {
            fireBounds = new Bounds(transform.position, ps.shape.scale); // Set the bounds to the scale of the rectangle
        }
        else
        {
            Debug.LogWarning("Pebble Spawner has to be attached to particle system!");
            enabled = false; // Disable the script if it's not attached to a particle system
        }

        platforms = new Transform[4];
        platforms[0] = GameObject.Find("Platform").transform;
        platforms[1] = GameObject.Find("Platform (1)").transform;
        platforms[2] = GameObject.Find("Platform (2)").transform;
        platforms[3] = GameObject.Find("Platform (3)").transform;
    }

    void Update()
    {
        if (Time.time > nextSpawnTime)
        {
            nextSpawnTime = Time.time + 1f / spawnRate; // Set the next spawn time
            SpawnPebble(); // Spawn a pebble
        }
    }

    void SpawnPebble()
    {
        if (pebblePrefab == null || platforms.Length != 4) return; // If the pebble prefab is null or platforms are not set, exit the function
        
        // Rabdomly pick pebble spawn position withtin the fire bounds
        float randomX = Random.Range(fireBounds.min.x, fireBounds.max.x);
        float randomZ = Random.Range(fireBounds.min.z, fireBounds.max.z);
        Vector3 spawnPosition = new Vector3(randomX, transform.position.y, randomZ); // Set the spawn position

        GameObject newPebble = Instantiate(pebblePrefab, spawnPosition, Quaternion.identity); // Instantiate a new pebble

        Rigidbody2D rb = newPebble.GetComponent<Rigidbody2D>(); // Get the pebble's Rigidbody2D component
        if (rb == null) return;

        rb.bodyType = RigidbodyType2D.Dynamic; // Set the body type to dynamic
        rb.gravityScale = 1; // Set the gravity scale to 0

        Transform targetPlatform = platforms[Random.Range(0, platforms.Length)]; // Pick a random platform
        Debug.Log("Target platform: " + targetPlatform.name);
        float platformSideThreshold = (platforms[0].position.x + platforms[1].position.x) / 2f; // Calculate the threshold for the platform sides

        int targetSide = targetPlatform.position.x < platformSideThreshold ? -1 : 1; // Determine the target side
        if ((targetSide == -1 && leftSidePebbles < 3) || (targetSide == 1 && rightSidePebbles < 3))
        {
            Collider2D platformCollider = targetPlatform.GetComponent<Collider2D>(); // Get the platform's collider
            if (platformCollider != null)
            {
                Bounds platformBounds = platformCollider.bounds; // Get the platform's bounds
                float targetY = platformBounds.max.y + 0.2f; // Land slightly above
                float targetX = Random.Range(platformBounds.min.x, platformBounds.max.x);
                Vector3 targetPosition = new Vector3(targetX, targetY, targetPlatform.position.z);

                Vector2 launchDirectionHorizontal = (targetPosition - spawnPosition).normalized;
                rb.linearVelocity = new Vector2(launchDirectionHorizontal.x * launchForceHorizontal, launchForceUpward);

                if (targetSide == -1) leftSidePebbles++;
                else rightSidePebbles++;

                newPebble.AddComponent<PebbleMovement>(); // Add the landing checker script
                
                // Add the PebbleCollisionChecker and initialize it with fireBounds
                PebbleMovement pebbleMovement = newPebble.AddComponent<PebbleMovement>();
                pebbleMovement.Initialize(fireBounds); // Pass the fireBounds to the PebbleMovement script
            }
        }
        else
        {
            Destroy(newPebble); // Destroy the pebble
        }
    }

    public void DecreasePebbleCount(float side)
        {
            if (side == -1) leftSidePebbles--;
            else rightSidePebbles--;
        }
}
