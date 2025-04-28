using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    public GameObject loadingScene;
    public TMP_Text loadingText;
    public TMP_Text continuePrompt;
    public static string nextSceneToLoad = "Demo";
    public static string previousScene = "";

    private bool isSceneReady = false; 

    void Start()
    {
        loadingScene.SetActive(true);
        continuePrompt.gameObject.SetActive(false);

        if (SceneManager.GetActiveScene().name == "IntroScene")
        {
            loadingText.text = "Press Any Key to Continue";
            isSceneReady = true; 
        }
        else
        {
            StartCoroutine(LoadSceneAsync(nextSceneToLoad));
        }
    }

    void Update()
    {
        if (isSceneReady && Input.anyKeyDown)
        {
            if (SceneManager.GetActiveScene().name == "IntroScene")
            {
                SceneManager.LoadScene("MainMenu");
            }
            else
            {
                SceneManager.LoadScene(nextSceneToLoad);
            }
        }
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName);
        loadOperation.allowSceneActivation = false;

        while (loadOperation.progress < 0.9f)
        {
            loadingText.text = "Loading... " + (loadOperation.progress * 100f).ToString("F0") + "%";
            yield return null;
        }

        loadingText.text = "Loading Complete";
        continuePrompt.gameObject.SetActive(true);

        isSceneReady = true; 

        yield return new WaitUntil(() => Input.anyKeyDown);

        loadOperation.allowSceneActivation = true;
    }

    public static void LoadSceneWithTracking(string newSceneName)
    {
        previousScene = SceneManager.GetActiveScene().name;
        nextSceneToLoad = newSceneName;
        SceneManager.LoadScene("LoadingScene");
    }
}
