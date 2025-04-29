using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryScreen : MonoBehaviour
{
    public string mainMenuSceneName = "MainMenuScene"; // Name of the main menu scene
    public string gameSceneName = "MainScene"; // Name of the game scene
    
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
