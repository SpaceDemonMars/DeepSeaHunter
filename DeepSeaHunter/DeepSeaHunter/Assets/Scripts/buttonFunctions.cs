using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
    public void resume()
    {
        GameManager.instance.stateUnpause();
    }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameManager.instance.stateUnpause();
    }

    public void loadLevel(int index)
    {
        SceneManager.LoadScene(index);
        GameManager.instance.stateUnpause();   
    }

    public void quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    public void respawn()
    {
        GameManager.instance.playerScript.spawnPlayer();
        GameManager.instance.resetLevelBoss();
        GameManager.instance.stateUnpause();
        Debug.Log("Respawn button clicked, player moved to: " + GameManager.instance.playerSpawnPos.transform.position);
    }

}
