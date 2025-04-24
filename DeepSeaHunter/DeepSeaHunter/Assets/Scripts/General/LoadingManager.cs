using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    public GameObject loadingScreen;
    public TMP_Text loadingText;
    public TMP_Text continuePrompt;
    public static string nextSceneToLoad = "Demo";

    private bool isSceneReady = false;

    void Start()
    {
        loadingScreen.SetActive(true);
        continuePrompt.gameObject.SetActive(false);
        StartCoroutine(LoadSceneAsync(nextSceneToLoad));
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
}
