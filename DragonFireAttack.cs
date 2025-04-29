using UnityEngine;

public class DragonFireAttack : MonoBehaviour
{
    public float delayBeforeFire = 2f; // Adjust as needed
    private ParticleSystem fireBreathPS;
    private Animator sparkAnimator;
    [SerializeField] private GameObject fireBreath;
    private bool isFireActive = false;
    private PebbleSpawner pebbleSpawner;

    void Start()
    {
        fireBreathPS = GetComponent<ParticleSystem>();
        sparkAnimator = transform.Find("SparkAnimationObj").GetComponent<Animator>(); // Assuming the Animator is a child of this GameObject
        pebbleSpawner = GetComponent<PebbleSpawner>(); // Assuming PebbleSpawner is on the same GameObject

        if (fireBreathPS == null)
        {
            Debug.LogError("Particle System not found on this GameObject.");
            enabled = false;
        }
        if (sparkAnimator == null)
        {
            Debug.LogError("SparkEffect Animator not found on child GameObject.");
            enabled = false;
        }
        if (pebbleSpawner == null)
        {
            Debug.LogError("PebbleSpawner script not found on this GameObject.");
            enabled = false;
        }

        // Start the attack sequence
        Invoke("StartFireAttack", delayBeforeFire);
        sparkAnimator.Play("SparkAnimation"); // Trigger the spark animation
        fireBreathPS.Stop(); // Ensure fire is off initially
        if (pebbleSpawner != null)
        {
            pebbleSpawner.enabled = false; // Disable spawning initially
        }
    }

    void StartFireAttack()
    {
        fireBreathPS.Play(); // Start the fire
         isFireActive = true;
        if (pebbleSpawner != null)
        {
            pebbleSpawner.enabled = true; // Enable spawning when fire starts
        }
    }

    public void StopFireAttack()
    {
        if (fireBreathPS != null)
        {
            fireBreathPS.Stop(); // Stop emitting new particles
            fireBreathPS.Clear(); // Remove any existing particles
            var emission = fireBreathPS.emission;
            emission.enabled = false; // Ensure no more particles are emitted
        }
        isFireActive = false;
        sparkAnimator.gameObject.SetActive(false); // Disable the spark animation
        if (pebbleSpawner != null)
        {
            pebbleSpawner.enabled = false; // Stop spawning when fire stops
        }
        if (fireBreath != null)
        {
            fireBreath.SetActive(false); // Disable the fire breath object
        }
        else
        {
            Debug.LogWarning("Fire Breath GameObject not assigned in the Inspector.");
        }
    }
    public bool IsFireActive()
    {
        return isFireActive; // Return the current state of fire
    }
}