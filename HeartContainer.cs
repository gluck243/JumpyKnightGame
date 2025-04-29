using UnityEngine;

public class HeartContainer : MonoBehaviour
{
    public float healthToRefill = 10f;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.PlayerRestoreHealth(healthToRefill); // Restore the player's health
                Destroy(gameObject); // Destroy the heart container after the player collects it
            }
            else
            {
                Debug.LogWarning("Player missing Health component");
            }
        }
    }
}
