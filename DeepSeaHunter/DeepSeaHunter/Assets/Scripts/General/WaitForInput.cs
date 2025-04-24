using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingWaitForInput : MonoBehaviour
{
    public string nextSceneName = "MainMenu";

    void Update()
    {
        if (Input.GetButtonDown("Interact"))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
