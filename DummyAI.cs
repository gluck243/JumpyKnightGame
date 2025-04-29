using UnityEngine;

public class DummyAI : MonoBehaviour
{
    public float contactDamage = 10f;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("TestEnemy Collision with Player detected!");
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.PlayerTakeDamage(contactDamage);
            }
        }
    }
}