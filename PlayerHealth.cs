using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 30f;
    private float currentHealth;
    private HealthUI healthUI;
    private bool initialUIUpdate = false;
    public GameObject deathScreenPanel;

    void Start()
    {
        currentHealth = maxHealth;
        healthUI = GameObject.Find("HealthHeartsPanel").GetComponent<HealthUI>();
        if (deathScreenPanel != null)
        {
            deathScreenPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("Death Screen Panel not assigned in PlayerHealth!");
        }
    }

    void Update()
    {
        if (!initialUIUpdate && healthUI != null)
        {
            healthUI.UpdateHealthDisplay(currentHealth);
            initialUIUpdate = true;
        }
    }

    public void PlayerTakeDamage(float damageAmount)
    {
        if (currentHealth <= 0) return;
        currentHealth -= damageAmount;
        UpdateHealthDisplay();
        if (currentHealth <= 0)
        {
            PlayerDie();
        }
    }

    public void PlayerRestoreHealth(float healthToRestore)
    {
        currentHealth += healthToRestore;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        UpdateHealthDisplay();
    }

    private void PlayerDie()
    {
        Debug.Log(gameObject.name + " Died!");
        if (deathScreenPanel != null)
        {
            deathScreenPanel.SetActive(true);
            Time.timeScale = 0f; // Pause the game
        }
        else
        {
            Debug.LogError("Death Screen Panel not assigned in PlayerHealth!");
        }
        Destroy(gameObject);
    }

    public float GetCurrentPlayerHealth()
    {
        return currentHealth;
    }

    void UpdateHealthDisplay()
    {
        if (healthUI != null)
        {
            healthUI.UpdateHealthDisplay((int)currentHealth);
        }
        else
        {
            Debug.LogWarning("HealthText UI element not assigned in the Inspector!");
        }
    }
}