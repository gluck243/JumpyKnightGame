using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 5f;
    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void EnemyTakeDamage(float damageAmount)
    {
        if (currentHealth <= 0) return;
        // Debug.LogError("<color=red>TAKE DAMAGE CALLED! Damage Amount: " + damageAmount + ", Current Health Before Damage: " + currentHealth + ", Time: " + Time.time + ", Stack Trace: " + System.Environment.StackTrace + "</color>");
        currentHealth -= damageAmount;
        if (currentHealth <= 0)
        {
            EnemyDie();
        }
    }

    /* public void RestoreHealth(float healthToRestore)
    {
        currentHealth += healthToRestore;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }*/

    private void EnemyDie()
    {
        Debug.Log(gameObject.name + " Died!");
        Destroy(gameObject);
    }

    public float GetCurrentEnemyHealth()
    {
        return currentHealth;
    }
}
