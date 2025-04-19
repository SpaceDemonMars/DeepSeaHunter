using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelect : MonoBehaviour
{
    public void LoadDemo()
    {
        SceneManager.LoadScene("Demo");
    }

    public void LoadLeanne()
    {
        SceneManager.LoadScene("Leanne");
    }

    public void LoadPaige()
    {
        SceneManager.LoadScene("Paige");
    }
}