using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public GameObject settingsPanel;

    private void Start()
    {
        if (string.IsNullOrEmpty(LoadingManager.previousScene))
        {
            LoadingManager.previousScene = "MainMenu";
        }
    }

    public void NewGame()
    {
        SaveManager.instance.DeleteSave();
        LoadingManager.LoadSceneWithTracking("Tutorial");
    }

    public void LoadGame()
    {
        if (SaveManager.instance.Load() != null)
        {
            LoadingManager.LoadSceneWithTracking("Tutorial");
        }
        else
        {
 //           Debug.Log("No save data. Starting new game.");
            NewGame();
        }
    }

    public void ReturnToPreviousScene()
    {
        if (!string.IsNullOrEmpty(LoadingManager.previousScene))
            SceneManager.LoadScene(LoadingManager.previousScene);
        else
            SceneManager.LoadScene("MainMenu");
    }

    public void Credits()
    {
        LoadingManager.previousScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("Credits");
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }

    public void QuitGame()
    {
 //       Debug.Log("Quitting game...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
