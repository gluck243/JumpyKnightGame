using UnityEngine;

public class PebbleKicked : MonoBehaviour
{
    private Transform targetTransform; // Target transform of the pebble
    public float speed = 5f; // Speed of the pebble
    
    public void SetTarget(Transform target)
    {
        targetTransform = target; // Set the target transform
    }

    void Update()
    {
        if (targetTransform != null) // Check if the target transform is not null
        {
            transform.position = Vector2.MoveTowards(transform.position, targetTransform.position, speed * Time.deltaTime); // Move the pebble towards the target
            if (Vector2.Distance(transform.position, targetTransform.position) < 0.5f) // Check if the pebble has reached the target
            {
                Debug.Log("Pebble hit the Dragon!");
                // Dragon damage logic here
                Destroy(gameObject); // Destroy the pebble

                GameObject player = GameObject.FindGameObjectWithTag("Player"); // Find the player
                if (player != null)
                {
                    PlayerMovement interactionScript = player.GetComponent<PlayerMovement>(); // Get the PlayerMovement script
                    if (interactionScript != null)
                    {
                        interactionScript.IncrementPebbleKickCount();
                        interactionScript.IncrementPebbleHitDragonCount();
                        Debug.Log("Pebble Kick Count: " + interactionScript.GetPebbleKickCount());
                    }
                }
                else
                {
                    Debug.LogWarning("Player GameObject not found!");
                }
            }
        }
    }
}
