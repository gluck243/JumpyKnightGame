using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed for left/right movement
    private float currentMoveSpeed; // Store the current move speed
    public float jumpForce = 10f; // Force of the jump
    // public float slamForce = 20f; // Force of the slam attack
    public float kickRange = 0.5f; // Range of the kick attack
    public float kickDamage = 5f; // Damage of the kick attack
    // public float slamDamage = 10f; // Damage of the slam attack
    private float horizontalInput; // Store horizontal input
    // private bool isSlamming = false; // Track if the player is currently slamming
    private float slowDownTimer = 0f; // Timer to track slowdown duration
    private float slowDownDuration = 0f; // Duration of the slowdown effect


    // private UnityEngine.Vector3 originalScale; // Store the original scale of the player
    // [SerializeField] private UnityEngine.Vector3 slammingScale = new UnityEngine.Vector3(1.2f, 0.6f, 1f); // Scale when slamming - squashed vertically, stretched horizontally


    [SerializeField] private Transform groundCheck; // Drag "GroundCheck" GameObject here in the Inspector
    [SerializeField] private float groundCheckRadius = 0.2f; // Radius of the ground check overlap
    [SerializeField] private LayerMask groundLayer; // "Ground" LayerMask to detect ground collision
    [SerializeField] private LayerMask kickLayerMask; // LayerMask for the kick raycast to ignore Player layer
    [SerializeField] private LayerMask webProjectileLayerMask; // LayerMask for the web projectile kickback
    [SerializeField] private Collider2D playerCollider; // Assign Player's Collider in Inspector for Web Kickback
    public float kickbackOverlapRadius = 0.6f; // Radius for overlap check
    [SerializeField] private Transform kickPoint; // Position from where kick originates

    private Rigidbody2D rb;

    // Pebble kick variables
    public float pebbleKickForce = 10f;
    public float pebbleKickRange = 0.6f;
    private int pebblesKicked = 0;
    public Transform dragonHeadTransform;
    private DragonFireAttack dragonFireAttackScript;
    private int pebblesHitDragon = 0;

    // DragonClaw variables
    public float clawsHealth = 1f; // Number of hits to defeat a claw
    private DragonPhaseTwo dragonPhaseTwo;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody 2D component attached to the Player
        // originalScale = transform.localScale; // Store the original scale of the player
        currentMoveSpeed = moveSpeed; // Set the current move speed to the default move speed
        
        GameObject dragonHead = GameObject.FindGameObjectWithTag("DragonHead");
        if (dragonHead != null)
        {
            dragonHeadTransform = dragonHead.transform;
        }
        else
        {
            Debug.LogError("DragonHead not found or not tagged with 'DragonHead'.");
        }

        GameObject fireBreathObject = GameObject.Find("FireBreath");
        if (fireBreathObject != null)
        {
            dragonFireAttackScript = fireBreathObject.GetComponent<DragonFireAttack>();
        }
        else
        {
            Debug.LogError("FireBreath GameObject not found in the scene.");
        }

        dragonPhaseTwo = Object.FindFirstObjectByType<DragonPhaseTwo>(); // Find the DragonPhaseTwo script in the scene
    }

    void Update()
    {

        if (slowDownTimer > 0f)
        {
            slowDownTimer -= Time.deltaTime; // Reduce the slowDownTimer
            if (slowDownTimer <= 0f)
            {
                currentMoveSpeed = moveSpeed; // Reset the move speed
            }
        }

        // Left/Right Movement
        horizontalInput = Input.GetAxisRaw("Horizontal"); // Get input from A/D keys or Left/Right arrow keys

        // Jump input
        if (Input.GetButtonDown("Jump") && IsGrounded()) // Check for Jump button (Spacebar by default) and if grounded and not slamming, edited, !isSlamming removed 
        {
            Jump(); // Call the Jump function
        }

        // Slam input
        /*if (Input.GetKeyDown(KeyCode.Space) && !IsGrounded() && !isSlamming) // Check for 'S' key and not grounded and not already slamming
        {
            StartSlam(); // Call a function to handle the slam
        }*/

        // Reset Slamming state when grounded
        /*if (IsGrounded() && isSlamming)
        {
            isSlamming = false;
            transform.localScale = originalScale; // Reset the player's scale

            // Check for enemies underneath the player when landing

            Collider2D[] collidersUnderneath = Physics2D.OverlapCircleAll(groundCheck.position, groundCheckRadius, ~0); // ~0 means "all layers"
            foreach (Collider2D collider in collidersUnderneath)
            {
                Health targetHealth = collider.GetComponent<Health>();
                if (targetHealth != null && collider.gameObject != gameObject)
                {
                    Debug.Log("Slam Landed on: " + collider.gameObject.name + ", applying slam damage.");
                    targetHealth.TakeDamage(slamDamage); // Apply slam damage
                }
            }
        }*/

        // Kick input
        if (Input.GetKeyDown(KeyCode.K)) // Check for 'K' key and not already slamming, edited, !isSlamming removed
        {
            Kick(); // Call a function to handle the kick
        }
    }

    void FixedUpdate()
    {
        MoveHorizontal(horizontalInput); // Move the player horizontally
        /*if (isSlamming) // Apply slam force in FixedUpdate while slamming
        {
            rb.linearVelocity = new UnityEngine.Vector2(rb.linearVelocity.x, -slamForce); // Apply downward slam force
        }*/
    }
    void MoveHorizontal(float input)
    {
        rb.linearVelocity = new UnityEngine.Vector2(input * currentMoveSpeed, rb.linearVelocity.y); // Set horizontal linearVelocity, keep vertical linearVelocity
    }

    void Jump()
    {
        // Jump logic
        rb.linearVelocity = new UnityEngine.Vector2(rb.linearVelocity.x, jumpForce); // Set vertical linearVelocity to jumpForce
    }

    /*void StartSlam()
    {
        isSlamming = true; // Set slamming state to true
        transform.localScale = slammingScale; // Set the player's scale to the slamming scale
        // Debug.Log("Slam Started!");
    }*/

    void Kick() // Handles melee kick, Web Projectile Kickback, Hot pebble kick and DragonHead kick
    {
        Debug.Log("Kick!");

        // Web projectile kickback logic
        // Perform Overlap Check for Web Projectiles
        Collider2D[] overlappingWebColliders = Physics2D.OverlapCircleAll(transform.position, kickbackOverlapRadius, webProjectileLayerMask); // Using OverlapCircleAll for simplicity and radius

        if (overlappingWebColliders != null && overlappingWebColliders.Length > 0) // Found overlapping web projectile(s)
        {
            Debug.Log("<color=green>Player Overlapping with Web Projectile(s) for Kickback</color>");

            bool webProjectileKickedBack = false; // Flag to track if at least one web projectile was kicked back in this Kick() call

            foreach (Collider2D webCollider in overlappingWebColliders) // Loop through ALL overlapping web projectiles
            {
                WebProjectile webProjectile = webCollider.GetComponent<WebProjectile>();
                if (webProjectile != null && !webProjectile.isReflected) // Found WebProjectile and not already reflected
                {
                    // Debug.Log("<color=blue>WEB KICKBACK - Reflecting Web Projectile!</color>");
                    webProjectile.ReflectProjectile(); // Call ReflectProjectile function on WebProjectile
                    webProjectileKickedBack = true; // Set flag to true as player kicked back at least one web
                }
            }

            if (webProjectileKickedBack) // If any web projectile was kicked back, return and do not perform melee kick
            {
                return;
            }
        }

        // Pebble kick logic
        if (dragonHeadTransform != null)
        {
            Collider2D[] hitPebbleColliders = Physics2D.OverlapCircleAll(transform.position, pebbleKickRange); // OverlapCircleAll to find all pebbles in range
            GameObject closestPebble = null; // Closest pebble to the player
            float closestDistance = Mathf.Infinity; // Closest distance to a pebble

            foreach (Collider2D collider in hitPebbleColliders) // Loop through all colliders in range
            {
                if (collider.CompareTag("HotPebble"))
                {
                    float distanceToPebble = UnityEngine.Vector2.Distance(transform.position, collider.transform.position); // Calculate distance to pebble
                    if (distanceToPebble < closestDistance) // Check if this pebble is closer than the previous closest pebble
                    {
                        closestPebble = collider.gameObject; // Set this pebble as the closest pebble
                        closestDistance = distanceToPebble; // Update the closest distance
                    }
                }
            }

            if (closestPebble != null) // If a pebble was found in range
            {
                Debug.Log("Player kicked a pebble!");
                Rigidbody2D pebbleRb = closestPebble.GetComponent<Rigidbody2D>(); // Get the pebble's Rigidbody2D component
                if (pebbleRb != null) // Check if the pebble's Rigidbody2D component is not null
                {
                    UnityEngine.Vector2 directionToDragon = (dragonHeadTransform.position - closestPebble.transform.position).normalized; // Calculate the direction to the dragon's head
                    pebbleRb.bodyType = RigidbodyType2D.Dynamic; // Make sure it can move
                    pebbleRb.linearVelocity = directionToDragon * pebbleKickForce; // Kick the pebble towards the dragon's head

                    PebbleKicked kickedPebbleScript = closestPebble.GetComponent<PebbleKicked>(); // Get the PebbleKicked script
                    if (kickedPebbleScript == null) // If the PebbleKicked script is not already attached
                    {
                        kickedPebbleScript = closestPebble.AddComponent<PebbleKicked>(); // Add the PebbleKicked script to the pebble
                    }
                    kickedPebbleScript.SetTarget(dragonHeadTransform); // Set the target for the pebble
                }
                return; // Prioritize pebble kick over melee kick if a pebble is nearby
            }
        }

        // Melee Kick Logic
        float kickDirection = 1f;
        if (horizontalInput < 0) kickDirection = -1f;
        else if (horizontalInput > 0) kickDirection = 1f;

        UnityEngine.Vector2 kickStartPosition = kickPoint.position;
        RaycastHit2D hitInfo = Physics2D.Raycast(kickStartPosition, UnityEngine.Vector2.right * kickDirection, kickRange, kickLayerMask);

        Debug.DrawRay(kickStartPosition, UnityEngine.Vector2.right * kickDirection * kickRange, Color.red, 0.1f);
        if (hitInfo.collider != null)
        {
            Debug.Log("Melee Kick hit: " + hitInfo.collider.gameObject.name);

            if (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Claw")) // Check if the hit object is on the "Claw" layer
            {
                clawsHealth--; // Reduce the claw's health
                Debug.Log("Kicked the " + hitInfo.collider.gameObject.name + "! Claw Health: " + clawsHealth); // Log the kick and claw health
                if (clawsHealth <= 0)
                {
                    Debug.Log(hitInfo.collider.gameObject.name + " defeated!");
                    // Find the DragonPhaseTwo script and notify it
                    DragonPhaseTwo dragonPhaseTwo = Object.FindFirstObjectByType<DragonPhaseTwo>();
                    if (dragonPhaseTwo != null)
                    {
                        dragonPhaseTwo.ClawDefeated(hitInfo.collider.gameObject);
                    }
                    hitInfo.collider.gameObject.SetActive(false); // Deactivate the claw
                }
            }
            else if (hitInfo.collider.CompareTag("DragonHead")) // Check if the hit object is the Dragon's Head
            {
                if (dragonPhaseTwo != null && dragonPhaseTwo.headLowered)
                {
                    Debug.Log("<color=yellow>Final Kick on Dragon's Head!</color>");
                    dragonPhaseTwo.EndBattle(); // Call a method in DragonPhaseTwo to handle the end of the battle
                }
                else if (dragonPhaseTwo != null && !dragonPhaseTwo.headLowered)
                {
                    Debug.Log("<color=orange>Hit Dragon's Head but it's not lowered yet!</color>");
                }
            }
            else
            {
                Debug.Log("Melee Kick hit a Claw but no Claw script found on: " + hitInfo.collider.gameObject.name);
            }

            // Check if the Giant Doors are hit
            if (hitInfo.collider.CompareTag("GiantDoors"))
            {
                GiantDoors doorScript = hitInfo.collider.GetComponent<GiantDoors>();
                if (doorScript != null)
                {
                    Debug.Log("<color=yellow>Player kicked the Giant Doors!</color>");
                    doorScript.KickDown(); // Call the KickDown function on the GiantDoor script
                }
                else
                {
                    Debug.Log("Melee Kick hit Giant Door but no GiantDoors script found.");
                }
            }

            // Handle hitting other objects
            EnemyHealth targetHealth = hitInfo.collider.GetComponent<EnemyHealth>();
            if (targetHealth != null)
            {
                targetHealth.EnemyTakeDamage(kickDamage);
            }
            else
            {
                Debug.Log("Melee Kick hit object but no Health component found on: " + hitInfo.collider.gameObject.name);
            }
        }
    }

    public void IncrementPebbleKickCount()
    {
        pebblesKicked++;
    }

    public void IncrementPebbleHitDragonCount()
    {
        pebblesHitDragon++; // Increment the pebblesHitDragon count
        if (pebblesHitDragon >= 3 && dragonFireAttackScript != null)
        {
            dragonFireAttackScript.StopFireAttack();

            // Find the Dragon and get the DragonPhaseTwo script
            GameObject dragonObject = GameObject.FindGameObjectWithTag("DragonHead"); 
            if (dragonObject != null)
            {
                DragonPhaseTwo phaseTwoScript = dragonObject.GetComponent<DragonPhaseTwo>();
                if (phaseTwoScript != null)
                {
                    phaseTwoScript.StartPhaseTwo();
                }
                else
                {
                    Debug.LogError("DragonPhaseTwo script not found on the DragonHeead GameObject.");
                }
            }
            else
            {
                Debug.LogError("Dragon GameObject not found with tag 'Dragon'.");
            }

            // Destroy all remaining hot pebbles
            GameObject[] remainingPebbles = GameObject.FindGameObjectsWithTag("HotPebble");
            foreach (GameObject pebble in remainingPebbles)
            {
                Destroy(pebble);
            }
        }
    }

    public int GetPebbleKickCount()
    {
        return pebblesKicked;
    }

    private bool IsGrounded()
    {
        // Using a CircleCast for slightly better ground detection than a single Raycast (more forgiving on edges)
        Collider2D hitCollider = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        return hitCollider != null; // If the OverlapCircle hits something in the GroundLayer, return true (grounded)
    }

    public void ApplySlowdown(float slowDownFactor)
    {
        currentMoveSpeed = moveSpeed * (1f - slowDownFactor); // Slow down the player's movement speed
        slowDownDuration = 1.5f; // Set the duration of the slowdown effect
        slowDownTimer = slowDownDuration; // Start the slowdown timer
        Debug.Log("<color=orange>Player Slowed Down! Current Speed: " + currentMoveSpeed + "</color>");
    }
}