using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class buttonFunctions : MonoBehaviour
{
    public void resume() { GameManager.instance.stateUnpause(); }

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
        GameManager.instance.dialogHiddenUI.SetActive(true); //show UI
        GameManager.instance.resetLevelBoss();
        GameManager.instance.stateUnpause();
        Debug.Log("Respawn button clicked, player moved to: " + GameManager.instance.playerSpawnPos.transform.position);
    }

    public void invenItemClick(Button button)
    {
        GameManager.instance.playerScript.inven.removeItem(button.GetComponent<inventoryButtons>().index);
    }

    public void settings() { GameManager.instance.openTabSettings(true); }
    public void inventory() { GameManager.instance.openTabInventory(true); }
    public void equipment() { GameManager.instance.openTabEquipment(true); }

    public void sliderMusicVolume(float val) { GameManager.instance.setMusicVolume(val); }
    public void sliderPlayerVolume(float val) { GameManager.instance.setPlayerVolume(val); }
    public void sliderFxVolume(float val) { GameManager.instance.setFxVolume(val); }
}
