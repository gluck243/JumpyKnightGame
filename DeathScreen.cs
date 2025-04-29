using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class DeathScreen : MonoBehaviour
{
    public string gameSceneName = "MainScene";
    public string mainMenuSceneName = "MainMenuScene";
    public TextMeshProUGUI gameOverText; // "GameOverText" TextMeshPro element is dreagged here in the Inspector
    public string[] gameOverMessages = new string[3]; // Array to hold three messages

    void OnEnable()
    {
        if (gameOverText != null && gameOverMessages.Length > 0)
        {
            int randomIndex = Random.Range(0, gameOverMessages.Length); // Get a random index
            gameOverText.text = gameOverMessages[randomIndex]; // Set the text to a random message
        }
        else
        {
            Debug.LogError("GameOverText TextMeshPro component or GameOver Messages array not set up correctly on the DeathScreenPanel!");
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // Reset time scale before loading a new scene
        SceneManager.LoadScene(gameSceneName);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f; // Reset time scale before loading a new scene
        SceneManager.LoadScene(mainMenuSceneName);
    }
}