using UnityEngine;

public class PebbleMovement : MonoBehaviour
{
    private bool hasLanded = false;
    private bool canLand = false;
    private float landDelay = 1f;
    private bool hasExitedBounds = false; // Track if the pebble has exited the bounds
    private Bounds fireBounds; // Bounds of the particle system
    private PebbleSpawner spawner; // Reference to the PebbleSpawner

    public void Initialize(Bounds bounds)
    {
        fireBounds = bounds; // Set the bounds of the particle system
    }
    void Start()
    {
        // Allow the pebble to pass through initially
        Invoke("EnableLanding", landDelay); // Wait n seconds before allowing landing
    }

    void EnableLanding()
    {
        canLand = true;
    }

    void Update()
    {
        // Check if the pebble is outside the bounds
        if (!fireBounds.Contains(transform.position))
        {
            hasExitedBounds = true; // Mark that the pebble has exited the bounds
        }

        // If the pebble has exited and re-enters the bounds, destroy it
        if (hasExitedBounds && fireBounds.Contains(transform.position))
        {
            Debug.Log("Pebble re-entered the fire bounds and will be destroyed.");

            // Notify the spawner to reduce the pebble count
            if (spawner != null)
            {
                spawner.DecreasePebbleCount(transform.position.x);
            }

            Destroy(gameObject); // Destroy the pebble
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!hasLanded && canLand && collision.gameObject.CompareTag("Platform"))
        {
            hasLanded = true;
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Static;
            }
        }
    }
}
