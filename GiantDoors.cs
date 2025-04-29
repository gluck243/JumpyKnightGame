using UnityEngine;

public class GiantDoors : MonoBehaviour
{
    public string BossSceneName = "BossScene";
    public Sprite openDoorSprite;
    private SpriteRenderer doorSpriteRenderer;
    private bool doorOpened = false;
    void Start()
    {
        doorSpriteRenderer = GetComponent<SpriteRenderer>(); // Get the SpriteRenderer component
        if (doorSpriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer not found on this GameObject!");
            enabled = false; // Disable this script
        }
    }

    public void KickDown()
    {
        if (!doorOpened)
        {
            doorOpened = true;
            Debug.Log("Door opened!");
            // Change sprite to open door state
            if ((openDoorSprite != null) && (doorSpriteRenderer != null))
            {
                doorSpriteRenderer.sprite = openDoorSprite;
            }
            else
            {
                Debug.LogWarning("Open Door Sprite or SpriteRenderer not assigned in the Inspector.");
            }
            Invoke("LoadBossScene", 1.5f); // Load the boss scene after 1.5 seconds
        }
    }

    void LoadBossScene()
    {
        Debug.Log("Loading Boss Scene: " + BossSceneName);
        // Load the boss scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(BossSceneName);
    }
}
