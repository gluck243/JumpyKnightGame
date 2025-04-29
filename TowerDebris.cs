using UnityEngine;

public class TowerDebris : MonoBehaviour
{
    public float fallSpeed = 2f;
    public float massiveDamage = 100f;
    public float despawnDelay = 0.5f;
    public GameObject shockwaveRightPrefab; 
    public GameObject shockwaveLeftPrefab; 
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0.2f; // Adjusted gravity for slow falling
        }

        // Ignore collisions with all objects tagged as "ClawPlatform"
        Collider2D debrisCollider = GetComponent<Collider2D>();
        GameObject[] clawPlatforms = GameObject.FindGameObjectsWithTag("ClawPlatform");
        foreach (GameObject clawPlatform in clawPlatforms)
        {
            Collider2D clawPlatformCollider = clawPlatform.GetComponent<Collider2D>();
            if (clawPlatformCollider != null)
            {
                Physics2D.IgnoreCollision(debrisCollider, clawPlatformCollider);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.PlayerTakeDamage(massiveDamage);
                Debug.Log("Player hit by debris! Took " + massiveDamage + " damage.");
            }
            TriggerShockwave();
            Destroy(gameObject, despawnDelay);
        }
        else if (collision.gameObject.CompareTag("Floor"))
        {
            TriggerShockwave();
            Destroy(gameObject, despawnDelay);
        }
    }

    void TriggerShockwave()
    {
        if (shockwaveRightPrefab != null && shockwaveLeftPrefab != null)
        {
            // Instantiate the right shockwave
            GameObject rightShockwave = Instantiate(shockwaveRightPrefab, transform.position, Quaternion.identity);
            Shockwave rightShockwaveScript = rightShockwave.GetComponent<Shockwave>();
            if (rightShockwaveScript != null)
            {
                rightShockwaveScript.SetDirection(1); // Set direction to right
            }

            // Instantiate the left shockwave
            GameObject leftShockwave = Instantiate(shockwaveLeftPrefab, transform.position, Quaternion.identity);
            Shockwave leftShockwaveScript = leftShockwave.GetComponent<Shockwave>();
            if (leftShockwaveScript != null)
            {
                leftShockwaveScript.SetDirection(-1); // Set direction to left
            }
        }
        else
        {
            Debug.LogError("Right or Left Shockwave Prefab not assigned in the Inspector!");
        }
    }
}
