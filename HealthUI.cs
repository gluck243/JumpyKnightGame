using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private float maxHealth = 30; // Matches the maxHealth in Health.cs
    public float healthPerHeart = 10; // Each heart represents 10 health
    public Sprite fullHeartSprite;
    public Sprite emptyHeartSprite;
    public List<UnityEngine.UI.Image> hearts;

    void Start()
    {
        /* hearts = new List<UnityEngine.UI.Image>();
        // Find all Image children of this GameObject (the HealthHeartsPanel)
        for (int i = 0; i < maxHealth; i++)
        {
            Transform heartTransform = transform.GetChild(i);
            if (heartTransform != null)
            {
                UnityEngine.UI.Image heartImage = heartTransform.GetComponent<UnityEngine.UI.Image>();
                if (heartImage != null)
                {
                    hearts.Add(heartImage);
                }
            }
        } */
        UpdateHealthDisplay((int)GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>().GetCurrentPlayerHealth()); // Initialize display with current health
    }

    public void UpdateHealthDisplay(float currentHealth)
    {
        // Debug.Log("Updating health display with current health: " + currentHealth);

        float numberOfFullHearts = currentHealth / healthPerHeart; // Calculate the number of full hearts
        float numberOfEmptyHearts = maxHealth / healthPerHeart; // Calculate the total number of hearts

        // Debug.Log("Number of Full Hearts: " + numberOfFullHearts);
        // Debug.Log("Number of Empty Hearts: " + numberOfEmptyHearts);

        // Ensure the hearts list has the correct number of Image components
        if (hearts.Count != numberOfEmptyHearts)
        {
            Debug.LogError("Hearts list size does not match maxHealth / healthPerHeart!");
            return;
        }

        for (int i = 0; i < hearts.Count; i++)
        {
            if (i < numberOfFullHearts)
            {
                hearts[i].sprite = fullHeartSprite;
            }
            else
            {
                hearts[i].sprite = emptyHeartSprite;
            }
        }
    } 
}
