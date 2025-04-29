using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

public class DragonPhaseTwo : MonoBehaviour
{
    public float delayBeforePhaseTwo = 2f; 
    public float debrisAttackDuration = 10f;
    public Sprite broughtDownHeadSprite;
    public float headLowerAmount = 1.24f; // How much to lower the head
    public float headLowerSpeed = 2f; // Speed of lowering 
    private SpriteRenderer dragonHeadRenderer;
    private Vector3 initialHeadPosition;
    private bool isLoweringHead = false;
    public GameObject platformLowerLeft;
    public GameObject platformLowerRight;
    public GameObject platformUpperLeft;
    public GameObject platformUpperRight;
    public GameObject clawPlatformLeft;
    public GameObject clawPlatformRight;
    public GameObject debrisPrefab;
    public Transform[] walls;
    public GameObject ceiling;
    // public string platformTag = "Platform"; // Tag for regular platforms
    public float debrisSpawnRate = 8f;
    private bool phaseTwoStarted = false;
    private float phaseTwoStartTime;
    private Vector3 originalTowerPosition;
    public float shakeMagnitude = 0.1f; // Adjust the intensity of the shake
    public float shakeSpeed = 20f;    // Adjust the speed of the shake
    private int clawsDefeated = 0; // Number of claws defeated
    public bool headLowered = false; // To prevent head lowering multiple times
    public GameObject VictoryScreenPanel; // Reference to the victory screen panel
    void Start()
    {
        if (VictoryScreenPanel != null)
        {
            VictoryScreenPanel.SetActive(false); // Ensure the victory screen is hidden at the start
        }

        if (clawPlatformLeft != null) clawPlatformLeft.SetActive(false); // Deactivate the left claw platform
        if (clawPlatformRight != null) clawPlatformRight.SetActive(false); // Deactivate the right claw platform
        
        walls = new Transform[2]; // Create a new array for walls
        walls[0] = GameObject.Find("Wall").transform;
        walls[1] = GameObject.Find("Wall (1)").transform;

        GameObject tower = GameObject.FindGameObjectWithTag("Tower");
        if (tower != null)
        {
            originalTowerPosition = tower.transform.position;
        }
        else
        {
            Debug.LogError("Tower GameObject not found with tag 'Tower'.");
        }

        dragonHeadRenderer = GetComponent<SpriteRenderer>(); // Assuming the SpriteRenderer is on the Dragon GameObject itself
        if (dragonHeadRenderer == null)
        {
            Debug.LogError("Dragon SpriteRenderer not found on this GameObject!");
        }
        initialHeadPosition = transform.position; // Store the initial head position
    }

    public void StartPhaseTwo()
    {
        if (!phaseTwoStarted)
        {
            phaseTwoStarted = true;
            phaseTwoStartTime = Time.time;
            Debug.Log("Phase Two Started!");
            Invoke("StartDebrisAttack", delayBeforePhaseTwo);
        }
    }

    void StartDebrisAttack()
    {
        Debug.Log("Debris Attack Started!");

        InvokeRepeating("ShakeTower", 0f, 1f / shakeSpeed); // Start shaking the tower

        if (platformLowerLeft != null) platformLowerLeft.SetActive(false);
        if (platformLowerRight != null) platformLowerRight.SetActive(false);
        if (platformUpperLeft != null) platformUpperLeft.SetActive(false);
        if (platformUpperRight != null) platformUpperRight.SetActive(false);

        if (debrisPrefab != null)
        {
            InvokeRepeating("SpawnDebris", 0, 1f / debrisSpawnRate);
            Invoke("ShowClaws", debrisAttackDuration);
        }
        else
        {
            Debug.LogError("Debris Prefab not set!");
        }
    }

    void ShakeTower()
    {
        if (phaseTwoStarted && Time.time < phaseTwoStartTime + delayBeforePhaseTwo + 2f) // Shake for 2 seconds after the phase starts
        {
            GameObject tower = GameObject.FindGameObjectWithTag("Tower");
            if (tower != null)
            {
                float offsetX = Random.Range(-shakeMagnitude, shakeMagnitude); // Random offset on X
                float offsetY = Random.Range(-shakeMagnitude, shakeMagnitude);  // Random offset on Y
                tower.transform.position = originalTowerPosition + new Vector3(offsetX, offsetY, 0f); // Apply the offset
            }
        }
        else
        {
            CancelInvoke("ShakeTower");
            GameObject tower = GameObject.FindGameObjectWithTag("Tower");
            if (tower != null)
            {
                tower.transform.position = originalTowerPosition; // Reset position
            }
        }
    }

    void SpawnDebris()
    {   
        float spawnX = Random.Range(walls[0].position.x + 0.5f, walls[1].position.x - 0.5f); // Randomly pick a spawn position between the walls
        float spawnY = ceiling.transform.position.y - 2f; // Set the spawn position to the ceiling
        Vector3 spawnPosition = new Vector3(spawnX, spawnY, 0f);
        Instantiate(debrisPrefab, spawnPosition, Quaternion.Euler(0, 0, Random.Range(0, 360))); // Instantiate a new debris
    }

    void ShowClaws()
    {
        Debug.Log("Claws Shown!");

        if (clawPlatformLeft != null) clawPlatformLeft.SetActive(true);
        if (clawPlatformRight != null) clawPlatformRight.SetActive(true);
    }

    public void ClawDefeated(GameObject claw)
    {
        clawsDefeated++;
        Debug.Log("Claws Defeated: " + clawsDefeated);

        if (clawsDefeated >= 2 && !headLowered)
        {
            headLowered = true;
            Debug.Log("Both claws defeated!");
            // Disable claw platforms
            if (clawPlatformLeft != null) clawPlatformLeft.SetActive(false);
            if (clawPlatformRight != null) clawPlatformRight.SetActive(false);

            // Enable original lower platforms
            // if (platformLowerLeft != null) platformLowerLeft.SetActive(true);
            // if (platformLowerRight != null) platformLowerRight.SetActive(true);

            CancelInvoke("SpawnDebris"); // Stop spawning debris

            // Swap to the "brought down" head sprite
            if (dragonHeadRenderer != null && broughtDownHeadSprite != null)
            {
                dragonHeadRenderer.sprite = broughtDownHeadSprite;
                Debug.Log("Dragon's head brought down!");
                // Lower the head position
                Vector3 targetPosition = initialHeadPosition + Vector3.down * headLowerAmount;
                StartCoroutine(MoveObject(transform, targetPosition, headLowerSpeed));
                isLoweringHead = true;
            }
            else
            {
                Debug.LogError("Dragon SpriteRenderer or Brought Down Head Sprite not assigned!");
            }
        }
    }

    public void EndBattle()
    {
        Debug.Log("<color=green>Dragon defeated! Battle finished!</color>");
        gameObject.SetActive(false); // Disable the dragon GameObject
        if (VictoryScreenPanel != null)
        {
            VictoryScreenPanel.SetActive(true); // Show the victory screen
            Time.timeScale = 0f; // Pause the game
        }
        else
        {
            Debug.LogError("VictoryScreenPanel not assigned!");
        }
    }

    private System.Collections.IEnumerator MoveObject(Transform targetTransform, Vector3 endPosition, float speed) // Smoothly move the head down
    {
        float sqrRemainingDistance = (targetTransform.position - endPosition).sqrMagnitude; // Calculate the remaining distance
        while (sqrRemainingDistance > float.Epsilon) // While the remaining distance is greater than a very small number
        {
            Vector3 newPosition = Vector3.MoveTowards(targetTransform.position, endPosition, speed * Time.deltaTime); // Calculate the new position
            targetTransform.position = newPosition; // Move the object
            sqrRemainingDistance = (targetTransform.position - endPosition).sqrMagnitude; // Recalculate the remaining distance
            yield return null; // Wait for the next frame
        }
    }
}
