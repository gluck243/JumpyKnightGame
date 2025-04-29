using UnityEngine;

public class GuardAI : MonoBehaviour
{
    public float patrolSpeed = 2f; // Speed of the guard's patrol
    public Transform groundDetection; // Transform to detect ground
    public float contactDamage = 10f; // Damage dealt on contact with player

    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private float turnCooldownDuration = 0.2f;

    private Rigidbody2D rb;
    private bool movingRight = true;
    private float lastTurnTime;

    private bool isFalling = true;
    public float fallingSpeed = 5f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lastTurnTime = -turnCooldownDuration;
        isFalling = true; // Start falling
    }

    void Update()
    {
        // Move horizontally
        if (movingRight)
        {
            rb.linearVelocity = new Vector2(patrolSpeed, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(-patrolSpeed, rb.linearVelocity.y);
        }

        // Ground/Edge Detection
        RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, 1f, groundLayerMask);

       if (groundInfo.collider == false) // If no ground detected at edge
        {
            // Debug.Log("<color=red>No ground DETECTED! Turning around...</color>"); // Highlight "No ground" messages in red

            //COOLDOWN CHECK:
            if (Time.time - lastTurnTime >= turnCooldownDuration) // Check if cooldown has expired
            {
                if (movingRight)
                {
                    // Debug.Log("Currently moving RIGHT, now turning LEFT");
                    transform.eulerAngles = new Vector3(0, -180, 0);
                    movingRight = false;
                }
                else
                {
                    // Debug.Log("Currently moving LEFT, now turning RIGHT");
                    transform.eulerAngles = new Vector3(0, 0, 0);
                    movingRight = true;
                }
                lastTurnTime = Time.time; // Update lastTurnTime to current time after turning
            }
            /* else
            {
                Debug.Log("Turnaround on cooldown, time since last turn: " + (Time.time - lastTurnTime).ToString("F2") + "s, cooldown duration: " + turnCooldownDuration + "s"); // Log when on cooldown
            } */
        }
    }

    void FixedUpdate()
    {
        if (isFalling)
        {
            rb.linearVelocity = new Vector2(0f, -fallingSpeed); // Apply downward velocity
            RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, 0.1f, groundLayerMask); // Raycast downwards

            if (groundInfo.collider != null) // Ground detected!
            {
                // Debug.Log("<color=green>Ghost Guard - Platform Detected, Fall Complete. Starting Patrol.</color>");
                isFalling = false; // Stop falling
                rb.linearVelocity = Vector2.zero; // Stop any remaining velocity
            }
            return; // Exit FixedUpdate early while falling, skipping patrol logic below
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the collision with the Player
        if (collision.gameObject.CompareTag("Player")) // If the object has the "Player" tag
        {
            Debug.Log(gameObject.name + " collided with player: " + collision.gameObject.name);

            // Get the health component of player
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.PlayerTakeDamage(contactDamage); // Deal damage to player
                Debug.Log("Player took damage!");
            }
            else
            {
                Debug.LogWarning("Player object does not have a Health component!");
            }
        }
    }
}