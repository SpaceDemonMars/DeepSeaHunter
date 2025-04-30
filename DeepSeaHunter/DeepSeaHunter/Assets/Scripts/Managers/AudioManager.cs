using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //reworked for bugfixes
    public static AudioManager Instance;
    string prefsMusic = "MusicVolume";
    string prefsPlayer = "PlayerVolume";
    string prefsFx = "fxVolume";
    string prefsDefault = "isDefault";

    [Header("FX Audio")]
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip[] uiSFX;
    [SerializeField] float uiVol;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance !=  this) Destroy(gameObject);
    }
    public void SetMusicVolume(float volume) 
    {
        PlayerPrefs.SetFloat(prefsMusic, volume);
        PlayerPrefs.SetInt(prefsDefault, 0);
        if (GameManager.instance)
        {
            GameManager.instance.radioScript.SetRadioVol(volume);
            GameManager.instance.pauseMusic.SetRadioVol(volume);
        }
    }
    public void SetPlayerVolume(float volume)
    {
        PlayerPrefs.SetFloat(prefsPlayer, volume);
        PlayerPrefs.SetInt(prefsDefault, 0);
        if (GameManager.instance)
            GameManager.instance.playerScript.aud.volume = volume; 
    }
    public void SetFxVolume(float volume)
    {
        PlayerPrefs.SetFloat(prefsFx, volume);
        PlayerPrefs.SetInt(prefsDefault, 0);
        aud.volume = volume; 
    }

    public void PlaySFX(AudioClip clip = null)
    {
        if (clip != null)   //playing custom clip
            aud.PlayOneShot(clip, uiVol);
        else                //playing standard
            aud.PlayOneShot(uiSFX[Random.Range(0, uiSFX.Length)], uiVol);
    }
}

/* Notes for Leanne

    [Header("Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource voiceSource;
        We cant do this, because it requires dragging audio sources from OTHER game objects onto this one
        Think of it as kidnapping their components. 
        Kidnapping components is BAD because as we move between scenes we lose access to those components
        this creates null reference errors.


    Mathf.Clamp01()
        We don't need to call this, the Sliders are normalized by default
    

    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        ApplyVolumes();
    }
    private void ApplyVolumes()
    {
        if (musicSource != null)
            musicSource.volume = masterVolume * musicVolume;
        if (sfxSource != null)
            sfxSource.volume = masterVolume * fxVolume;
        if (voiceSource != null)
            voiceSource.volume = masterVolume * playerVolume;
    }
        We can add this back in later, but it was giving me trouble when testing so its out for now.
*/

/*public void SetMusicVolume(float volume)
{
    musicVolume = Mathf.Clamp01(volume);
    PlayerPrefs.SetFloat("MusicVolume", musicVolume);
    PlayerPrefs.Save();
    ApplyVolumes();
}

public void SetPlayerVolume(float volume)
{
    playerVolume = Mathf.Clamp01(volume);
    PlayerPrefs.SetFloat("PlayerVolume", playerVolume);
    PlayerPrefs.Save();
    ApplyVolumes();
}

public void SetFxVolume(float volume)
{
    fxVolume = Mathf.Clamp01(volume);
    PlayerPrefs.SetFloat("FxVolume", fxVolume);
    PlayerPrefs.Save();
    ApplyVolumes();
}*/