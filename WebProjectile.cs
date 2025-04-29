using UnityEngine;

public class WebProjectile : MonoBehaviour
{
    public float damageAmount = 5f; // Damage dealt on contact with player
    public float slowDownFactor = 0.5f; // Slowdown factor on contact with player
    public bool isReflected = false; // Track if the projectile is reflected
    public SpiderAI shooterSpider; // Public variable to store reference to the spider that shot this projectile set by SpiderAI.cs
    public float reflectedProjectileSpeed = 25f; // Speed of the reflected projectile
    public float reflectedWebDamageToSpider = 100f; // Damage of reflected web to spider
    public bool useHoming = true; // Enable/Disable Homing Behavior 
    public float homingAngularSpeed = 360f; // Angular speed for homing - degrees per second
    public Color reflectedColor = Color.yellow; // Color of reflected projectile - Visual Feedback
    private Rigidbody2D rb; // Get Rigidbody in Start for efficiency

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (!isReflected) // Only damage player if not reflected
            {
                // Debug.Log("<color=cyan>Web Projectile HIT Player: " + collision.gameObject.name + " - NOT REFLECTED</color>");
                PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.PlayerTakeDamage(damageAmount);
                    Debug.Log("Player took " + damageAmount + " web damage.");
                }
                else
                {
                    Debug.LogWarning("Player object is missing Health component");
                }
                PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
                if (playerMovement != null)
                {
                    playerMovement.ApplySlowdown(slowDownFactor);
                    Debug.Log("Player slowed down by web.");
                }
                else
                {
                    Debug.LogWarning("Player object is missing PlayerMovement component");
                }
                Destroy(gameObject); // Destroy if not reflected
            }
            else // Is Reflected - Player Collision
            {
                Debug.Log("<color=red>REFLECTED Web Projectile hit Player</color>");
                Destroy(gameObject);
                return;
            }
        }
        else if (other.CompareTag("Enemy") && isReflected) // Check for Enemy hit IF reflected
        {
            // Debug.Log("<color=green>Reflected Web Projectile hit Enemy (Spider)!</color>");
            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.EnemyTakeDamage(reflectedWebDamageToSpider); // Use reflected damage value - Instant Kill
                if (enemyHealth.GetCurrentEnemyHealth() <= 0)
                {
                   // Debug.Log("<color=red>Spider destroyed by reflected web!</color>");
                }
            }
            Destroy(gameObject); // Destroy after hitting enemy (if reflected)
        }
        else if (other.CompareTag("Floor") || other.CompareTag("Wall") || other.CompareTag("Platform"))
        {
            Debug.Log("<color=grey>Web Projectile hit Floor, Wall, or Platform - Destroying</color>");
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // Basic projectile self-destruct after some time
        Destroy(gameObject, 5f);

        // Homing behavior
        if (isReflected && useHoming && shooterSpider != null) // Homing logic only if reflected, homing enabled, and spider reference is valid
        {
            Vector2 targetDirection = (shooterSpider.transform.position - transform.position).normalized; // Direction to spider
            Vector2 currentDirection = rb.linearVelocity.normalized; // Current projectile direction

            float angleDifference = Vector2.SignedAngle(currentDirection, targetDirection); // Angle between current and target direction

            float rotationThisFrame = homingAngularSpeed * Time.deltaTime; // Max rotation this frame based on angular speed

            if (Mathf.Abs(angleDifference) > Mathf.Epsilon) // Only rotate if there's a significant angle difference
            {
                float clampedAngle = Mathf.Clamp(angleDifference, -rotationThisFrame, rotationThisFrame);
                Quaternion rotation = Quaternion.AngleAxis(clampedAngle, Vector3.forward); // Create rotation Quaternion
                rb.linearVelocity = rotation * rb.linearVelocity; // Apply rotation to velocity to steer towards target
            }
        }
    }

    public void ReflectProjectile()
    {
        Debug.Log("<color=yellow>Web Projectile REFLECTED by Player!</color>");
        isReflected = true;
        GetComponent<SpriteRenderer>().color = reflectedColor; // Visual Feedback - Change color on reflection
        if (shooterSpider != null)
        {
            UnityEngine.Vector2 directionToSpider = (shooterSpider.transform.position - transform.position).normalized; // Direction to spider
            rb.linearVelocity = directionToSpider * reflectedProjectileSpeed;
        }
        else
        {
            Debug.LogWarning("<color=red>Reflected Web - NO Shooter Spider Reference! - Reversing Velocity Instead (Fallback)</color>"); // Warning
            GetComponent<Rigidbody2D>().linearVelocity = -GetComponent<Rigidbody2D>().linearVelocity; // Fallback: Reverse velocity if no spider reference
        }
    }
}
