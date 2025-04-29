using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool isPaused = false;
    public GameObject pauseMenuUI;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        // Re-inable player movement
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.enabled = true;
            }
        }
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        // Disable player movement
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.enabled = false;
            }
        }
    }

    public void LoadMainMenu()
    {
        Debug.Log("Loading Main Menu...");
        Time.timeScale = 1f; // Reset time scale before loading a new scene
        SceneManager.LoadScene("MainMenuScene");
    }

    public void RestartGame()
    {
        Debug.Log("Restarting Game...");
        Time.timeScale = 1f; // Reset time scale before loading a new scene
        SceneManager.LoadScene("MainScene");
    }
}
