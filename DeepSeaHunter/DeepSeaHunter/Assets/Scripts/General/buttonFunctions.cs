using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class buttonFunctions : MonoBehaviour
{
    public void resume() { GameManager.instance.stateUnpause(); GameManager.instance.playSFX(); }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameManager.instance.playSFX();
        GameManager.instance.stateUnpause();
    }

    public void quit()
    {
        GameManager.instance.playSFX();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    public void respawn()
    {
        GameManager.instance.playSFX();
        GameManager.instance.playerScript.spawnPlayer();
        GameManager.instance.dialogHiddenUI.SetActive(true); //show UI
        GameManager.instance.resetLevelBoss();
        GameManager.instance.stateUnpause();
        Debug.Log("Respawn button clicked, player moved to: " + GameManager.instance.playerSpawnPos.transform.position);
    }

    public void invenItemClick(Button button)
    {
        GameManager.instance.playSFX();
        GameManager.instance.playerScript.inven.removeItem(button.GetComponent<inventoryButtons>().index);
    }

    public void settings() { GameManager.instance.openTabSettings(true); GameManager.instance.playSFX(); }
    public void inventory() { GameManager.instance.openTabInventory(true); GameManager.instance.playSFX(); }
    public void equipment() { GameManager.instance.openTabEquipment(true); GameManager.instance.playSFX(); }
    public void journal() { GameManager.instance.openTabJournal(true); GameManager.instance.playSFX(); }

    public void sliderMusicVolume(float val) { GameManager.instance.setMusicVolume(val); GameManager.instance.playSFX(); }
    public void sliderPlayerVolume(float val) { GameManager.instance.setPlayerVolume(val); GameManager.instance.playSFX(); }
    public void sliderFxVolume(float val) { GameManager.instance.setFxVolume(val); GameManager.instance.playSFX(); }
}
