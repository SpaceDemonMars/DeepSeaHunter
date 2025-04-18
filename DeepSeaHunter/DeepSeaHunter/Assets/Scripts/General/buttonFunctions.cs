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

    public void settings()
    {
        GameManager.instance.openTabSettings(true);
    }
    public void inventory()
    {
        GameManager.instance.openTabInventory(true);
    }
    public void equipment()
    {
        GameManager.instance.openTabEquipment(true);
    }
}
