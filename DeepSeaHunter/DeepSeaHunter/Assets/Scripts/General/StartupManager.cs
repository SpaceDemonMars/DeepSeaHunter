using UnityEngine;
using UnityEngine.SceneManagement;

public class StartupManager : MonoBehaviour
{
    private bool waitingForInput = true;

    void Update()
    {
        if (waitingForInput && Input.anyKeyDown)
        {
            waitingForInput = false;
            LoadIntroScene();
        }
    }

    void LoadIntroScene()
    {
        SceneManager.LoadScene("IntroScene");
    }
}
