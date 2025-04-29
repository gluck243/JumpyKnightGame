using UnityEngine;

public class SpiderAI : MonoBehaviour
{
    public GameObject WebProjectilePrefab;
    public float patrolSpeed = 2f;
    public float contactDamage = 10f;
    public float webProjectileSpeed = 5f;
    public float shootCooldown = 2f;
    public float shootRange = 5f;
    public bool isLeftWallSpider = false;
    private float shootTimer = 0f;
    private Rigidbody2D rb;
    // private bool movingRight = true;
    private bool isCrawlingUp = true;
    private float patrolTimer = 0f;
    public float patrolDuration = 3f;
    public Transform groundDetection;
    public Transform wallDetection;
    [SerializeField] private LayerMask playerLayerMask;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private LayerMask wallLayerMask;
    [SerializeField] private float turnCooldownDuration = 0.2f;
    private float lastTurnTime;
    // private bool isWallCrawling = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lastTurnTime = -turnCooldownDuration;
    }

    void Update()
    {
        shootTimer += Time.deltaTime;

        if (shootTimer >= shootCooldown)
        {
            Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, shootRange, playerLayerMask);
            if (playerCollider != null)
            {
                Debug.Log("<color=yellow>Player Detected! - Shooting Web</color>");
                GameObject webProjectile = Instantiate(WebProjectilePrefab, transform.position, Quaternion.identity);
                Vector2 playerDirection = (playerCollider.transform.position - transform.position).normalized;
                Rigidbody2D webRB = webProjectile.GetComponent<Rigidbody2D>();
                if (webRB != null)
                {
                    webRB.linearVelocity = playerDirection * webProjectileSpeed;
                }
                else
                {
                    Debug.LogWarning("WebProjectile prefab missing Rigidbody2D component");
                }
                shootTimer = 0f;

                // Pass Spider (this script) reference to WebProjectile
                WebProjectile webProjectileScript = webProjectile.GetComponent<WebProjectile>();
                if (webProjectileScript != null)
                { 
                    webProjectileScript.shooterSpider = this; // Pass SpiderAI script reference to the projectile
                    Debug.Log($"<color=magenta>Spider Passing SpiderAI Reference to Web Projectile: {this.gameObject.name}</color>");
                }
            }
        }

        if (isLeftWallSpider)
        {
            transform.rotation = UnityEngine.Quaternion.Euler(0f, 0f, 270f); // Force 270 rotation for Left Wall
        }
        else
        {
            transform.rotation = UnityEngine.Quaternion.Euler(0f, 0f, 90f);  // Force 90 rotation for Right Wall (and default)
        }
    }


    void FixedUpdate() // Conditional wallDetectionDirection based on isLeftWallSpider
    {
        patrolTimer += Time.deltaTime;

        Vector2 wallDetectionDirection; // Declare wallDetectionDirection

        if (isLeftWallSpider)
        {
            wallDetectionDirection = Vector2.left; // For left wall spiders, raycast to the LEFT
            // Debug.Log("<color=green>Left Wall Spider - Wall Detection Direction: Vector2.left</color>");
        }
        else
        {
            wallDetectionDirection = Vector2.right; // For right wall spiders (and default), raycast to the RIGHT
        }
        RaycastHit2D wallInfo = Physics2D.Raycast(wallDetection.position, wallDetectionDirection, 0.5f, wallLayerMask); // Raycast to detect walls


        if (wallInfo.collider == true) // Wall detected!
        {
            if (patrolTimer >= patrolDuration)
            {
                isCrawlingUp = !isCrawlingUp;
                patrolTimer = 0f;
            }

            if (isCrawlingUp)
            {
                rb.linearVelocity = new Vector2(0f, patrolSpeed);
            }
            else
            {
                rb.linearVelocity = new Vector2(0f, -patrolSpeed);
            }

        }
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) // If the spider collides with the player
        {
            Debug.Log(gameObject.name + " collided with player: " + collision.gameObject.name); 

            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>(); // Get the Health component of the player

            if (playerHealth != null) // If the player has a Health component
            {
                playerHealth.PlayerTakeDamage(contactDamage); // Deal damage to the player
                Debug.Log("Player took " + contactDamage + " damage. They have " + playerHealth.GetCurrentPlayerHealth() + " hp left" ); 
            }
            else
            {
                Debug.LogWarning("Player does not have a health component");
            }
        }
    }
}