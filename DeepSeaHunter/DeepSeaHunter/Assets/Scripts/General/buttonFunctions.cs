using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class buttonFunctions : MonoBehaviour
{
    [SerializeField] buttonSettingsSAVE saveSettings;
    [SerializeField] Slider music;
    [SerializeField] Slider player;
    [SerializeField] Slider fx;
    [SerializeField] GameObject defaultsButton;
    [SerializeField] AudioClip testFxClip;
    [SerializeField] AudioClip testplayerClip;

    private void Start()
    {
        applySavedSettings();
        loadPlayerPrefsVolumes();
    }
    private void loadPlayerPrefsVolumes()
    {
        if (PlayerPrefs.HasKey("MusicVolume"))
            AudioManager.Instance.SetMusicVolume(PlayerPrefs.GetFloat("MusicVolume"));
        if (PlayerPrefs.HasKey("PlayerVolume"))
            AudioManager.Instance.SetPlayerVolume(PlayerPrefs.GetFloat("PlayerVolume"));
        if (PlayerPrefs.HasKey("FxVolume"))
            AudioManager.Instance.SetFxVolume(PlayerPrefs.GetFloat("FxVolume"));
    }
    public void save() { if (GameManager.instance != null) GameManager.instance.Save(); }
    public void load() { if (GameManager.instance != null) GameManager.instance.Load(); }
    public void resume() { if (GameManager.instance != null) { GameManager.instance.stateUnpause(); AudioManager.Instance.PlaySFX(null); } }
    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        if (GameManager.instance != null)
        {
            AudioManager.Instance.PlaySFX(null);
            GameManager.instance.stateUnpause();
        }
    }
    public void quit()
    {
        if (GameManager.instance != null) AudioManager.Instance.PlaySFX(null);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    public void respawn()
    {
        if (GameManager.instance != null)
        {
            AudioManager.Instance.PlaySFX(null);
            GameManager.instance.playerScript.spawnPlayer();
            GameManager.instance.dialogHiddenUI.SetActive(true);
            GameManager.instance.resetLevelBoss();
            GameManager.instance.stateUnpause();
        }
    }
    public void invenItemClick(Button button)
    {
        if (GameManager.instance != null)
        {
            AudioManager.Instance.PlaySFX(null);
            GameManager.instance.playerScript.inven.removeItem(button.GetComponent<inventoryButtons>().index);
        }
    }
    public void settings() { if (GameManager.instance != null) { GameManager.instance.openTabSettings(true); AudioManager.Instance.PlaySFX(null); } }
    public void inventory() { if (GameManager.instance != null) { GameManager.instance.openTabInventory(true); AudioManager.Instance.PlaySFX(null); } }
    public void equipment() { if (GameManager.instance != null) { GameManager.instance.openTabEquipment(true); AudioManager.Instance.PlaySFX(null); } }
    public void journal() { if (GameManager.instance != null) { GameManager.instance.openTabJournal(true); AudioManager.Instance.PlaySFX(null); } }
    public void sliderMusicVolume(float val)
    {
        if (saveSettings != null) saveSettings.musicVol = val;
        AudioManager.Instance.SetMusicVolume(val);
        if (defaultsButton != null) defaultsButton.SetActive(!saveSettings.checkDefaults());
        if (testFxClip != null)
            AudioManager.Instance.PlaySFX(testFxClip);
    }
    public void sliderPlayerVolume(float val)
    {
        if (saveSettings != null) saveSettings.playerVol = val;
        AudioManager.Instance.SetPlayerVolume(val);
        if (defaultsButton != null) defaultsButton.SetActive(!saveSettings.checkDefaults());
        if (testplayerClip != null)
            AudioManager.Instance.PlaySFX(testplayerClip);
    }
    public void sliderFxVolume(float val)
    {
        if (saveSettings != null) saveSettings.fxVol = val;
        AudioManager.Instance.SetFxVolume(val);
        if (defaultsButton != null) defaultsButton.SetActive(!saveSettings.checkDefaults());
        if (testFxClip != null)
        AudioManager.Instance.PlaySFX(testFxClip);
    }
    public void restoreDefaults()
    {
        if (saveSettings != null)
        {
            saveSettings.restoreDefaults();
            applySavedSettings();
        }
    }
    void applySavedSettings()
    {
        if (saveSettings == null) return;

        if (music != null)
            music.value = saveSettings.musicVol;
        AudioManager.Instance.SetMusicVolume(saveSettings.musicVol);

        if (player != null)
            player.value = saveSettings.playerVol;
        AudioManager.Instance.SetPlayerVolume(saveSettings.playerVol);

        if (fx != null)
            fx.value = saveSettings.fxVol;
        AudioManager.Instance.SetFxVolume(saveSettings.fxVol);

        if (defaultsButton != null)
            defaultsButton.SetActive(!saveSettings.checkDefaults());
    }
}
