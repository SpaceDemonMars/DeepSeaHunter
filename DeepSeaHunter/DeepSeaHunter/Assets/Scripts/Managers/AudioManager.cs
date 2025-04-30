using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //reworked for bugfixes
    public static AudioManager Instance;

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
    }

    

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
            sfxSource.PlayOneShot(clip, fxVolume * masterVolume);
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