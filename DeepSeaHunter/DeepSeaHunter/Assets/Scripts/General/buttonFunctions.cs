using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class buttonFunctions : MonoBehaviour
{
    string prefsMusic = "MusicVolume";
    string prefsPlayer = "PlayerVolume";
    string prefsFx = "fxVolume";
    string prefsDefault = "isDefault";
    [SerializeField] Slider music;
    [SerializeField] Slider player;
    [SerializeField] Slider fx;
    [SerializeField] GameObject defaultsButton;

    private void Start()
    {
        applySavedSettings();
    }


    public void save() { GameManager.instance.Save(); }
    public void load() { GameManager.instance.Load(); }

    public void resume() { GameManager.instance.stateUnpause(); AudioManager.Instance.PlaySFX(); }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        AudioManager.Instance.PlaySFX();
        GameManager.instance.stateUnpause();
    }

    public void quit()
    {
        AudioManager.Instance.PlaySFX();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    public void respawn()
    {
        AudioManager.Instance.PlaySFX();
        GameManager.instance.playerScript.spawnPlayer();
        GameManager.instance.dialogHiddenUI.SetActive(true); //show UI
        GameManager.instance.resetLevelBoss();
        GameManager.instance.stateUnpause();
     //   Debug.Log("Respawn button clicked, player moved to: " + GameManager.instance.playerSpawnPos.transform.position);
    }

    public void invenItemClick(Button button)
    {
        AudioManager.Instance.PlaySFX();
        GameManager.instance.playerScript.inven.removeItem(button.GetComponent<inventoryButtons>().index);
    }

    public void settings() { GameManager.instance.openTabSettings(true); AudioManager.Instance.PlaySFX(); }
    public void inventory() { GameManager.instance.openTabInventory(true); AudioManager.Instance.PlaySFX(); }
    public void equipment() { GameManager.instance.openTabEquipment(true); AudioManager.Instance.PlaySFX(); }
    public void journal() { GameManager.instance.openTabJournal(true); AudioManager.Instance.PlaySFX(); }

    public void sliderMusicVolume(float val)
    {
        AudioManager.Instance.SetMusicVolume(val);
        defaultsButton.SetActive(PlayerPrefs.GetInt(prefsDefault) == 0);
        AudioManager.Instance.PlaySFX();
    }
    public void sliderPlayerVolume(float val)
    {
        AudioManager.Instance.SetPlayerVolume(val);
        defaultsButton.SetActive(PlayerPrefs.GetInt(prefsDefault) == 0);
        AudioManager.Instance.PlaySFX();
    }
    public void sliderFxVolume(float val)
    {
        AudioManager.Instance.SetFxVolume(val);
        defaultsButton.SetActive(PlayerPrefs.GetInt(prefsDefault) == 0);
        AudioManager.Instance.PlaySFX();
    }
    public void restoreDefaults()
    {
        PlayerPrefs.DeleteAll(); //clear existing
        PlayerPrefs.SetFloat(prefsMusic, .5f);
        PlayerPrefs.SetFloat(prefsPlayer, .5f);
        PlayerPrefs.SetFloat(prefsFx, .5f);
        PlayerPrefs.SetInt(prefsDefault, 1);

        applySavedSettings();
    }

    void applySavedSettings()
    {
        music.value = PlayerPrefs.GetFloat(prefsMusic);
        AudioManager.Instance.SetMusicVolume(PlayerPrefs.GetFloat(prefsMusic));
        player.value = PlayerPrefs.GetFloat(prefsMusic);
        AudioManager.Instance.SetPlayerVolume(PlayerPrefs.GetFloat(prefsPlayer));
        fx.value = PlayerPrefs.GetFloat(prefsMusic);
        AudioManager.Instance.SetFxVolume(PlayerPrefs.GetFloat(prefsFx));
        //prefsDefault == 0 -> settings are NOT Default -> button is ACTIVE (true)
        //prefsDefault == 1 -> settings are Default -> button is NOT Active (false)
        defaultsButton.SetActive(PlayerPrefs.GetInt(prefsDefault) == 0);
    }
}
