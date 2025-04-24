using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("LoadingScreen"); 
        SceneManager.LoadScene("Demo"); 
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit(); 
    }
}
