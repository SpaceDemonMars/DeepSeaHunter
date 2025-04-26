using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public GameObject settingsPanel;
    public Button loadButton;

    private void Start()
    {
        CheckForSaveFile(); 
    }

    public void StartGame()
    {
        GameManager.loadFromSave = false;
        LoadSceneData.nextSceneToLoad = "Demo";
        SceneManager.LoadScene("LoadingScreen");
    }

    public void LoadGame()
    {
        if (!SaveManager.instance.SaveExists())
            return;

        GameManager.loadFromSave = true;
        LoadSceneData.nextSceneToLoad = "Demo";
        SceneManager.LoadScene("LoadingScreen");
    }

    public void EraseSave()
    {
        SaveManager.instance.DeleteSave();
        CheckForSaveFile(); 
    }

    private void CheckForSaveFile()
    {
        if (SaveManager.instance.SaveExists())
            loadButton.interactable = true;
        else
            loadButton.interactable = false;
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
