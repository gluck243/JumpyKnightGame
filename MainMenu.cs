using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string sceneToLoad = "MainScene";

    public void Play()
    {
        Debug.Log("Play button clicked");
        SceneManager.LoadScene(sceneToLoad);
    }
    
    public void Quit()
    {
        Debug.Log("Quit button clicked");
        #if UNITY_WEBGL
        // For WebGL, try to close the current tab or window
        Application.OpenURL("about:blank"); // Open a blank page first (may help with some browsers)
        Application.ExternalEval("window.close();"); // Attempt to close the window
#else
        Application.Quit(); // For standalone builds
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // For quitting in the editor
#endif
#endif
    }
}
