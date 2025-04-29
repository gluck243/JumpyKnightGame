using UnityEngine;

public class Shockwave : MonoBehaviour
{
    public float speed = 1f;
    public float damage = 10f;
    // private Vector2 startPosition;
    public int direction; // 1 for right, -1 for left

    void Start()
    {
        // startPosition = transform.position;
    }

    void Update()
    {
        // Debug.Log("Shockwave Direction: " + direction + ", Position: " + transform.position);
        Vector3 movement = Vector3.right * speed * Time.deltaTime * direction;
        transform.Translate(movement);
        // Debug.Log("Shockwave Position After Translate: " + transform.position);
        // Flip sprite based on direction
    }

    public void SetDirection(int dir)
    {
        direction = dir;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.PlayerTakeDamage(damage);
                Debug.Log("Player hit by shockwave! Took " + damage + " damage.");
            }
        }
        if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
