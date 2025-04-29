using UnityEngine;

public class FireDamage : MonoBehaviour
{
    public float damagePerSecond = 10f;
    private float damageTimer = 0f;

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            damageTimer += Time.deltaTime;
            if (damageTimer >= 1f)
            {
                damageTimer -= 1f;
                PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.PlayerTakeDamage(damagePerSecond);
                    Debug.Log("Player took " + damagePerSecond + " fire damage.");
                }
                else
                {
                    Debug.LogWarning("Health component not found on Player GameObject.");
                }
            }
        }
    }
}
