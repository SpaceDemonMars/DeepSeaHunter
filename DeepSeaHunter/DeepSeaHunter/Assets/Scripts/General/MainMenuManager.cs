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
        LoadSceneData.nextSceneToLoad = "Demo";
        SceneManager.LoadScene("LoadingScreen");
    }
    public void LoadGame()
    {
        if (SaveManager.instance.Load() != null)
        {
            LoadSceneData.nextSceneToLoad = "Demo"; 
            SceneManager.LoadScene("LoadingScreen");
        }
        else
        {
            Debug.Log("No save data found.Starting a new game instead.");
            NewGame(); 
        }
    }

    public void ReturnToPreviousScene()
    {
        SceneManager.LoadScene(LoadingManager.previousScene);
    }

    public void Credits()
    {
        LoadSceneData.previousScene = SceneManager.GetActiveScene().name;
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
        Debug.Log("Quitting game...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
