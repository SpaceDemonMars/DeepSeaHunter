using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Volumes")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float playerVolume = 1f;
    [Range(0f, 1f)] public float fxVolume = 1f;

    [Header("Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource voiceSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (PlayerPrefs.HasKey("MusicVolume"))
                musicVolume = PlayerPrefs.GetFloat("MusicVolume");
            if (PlayerPrefs.HasKey("PlayerVolume"))
                playerVolume = PlayerPrefs.GetFloat("PlayerVolume");
            if (PlayerPrefs.HasKey("FxVolume"))
                fxVolume = PlayerPrefs.GetFloat("FxVolume");

            ApplyVolumes();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        ApplyVolumes();
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

    private void ApplyVolumes()
    {
        if (musicSource != null)
            musicSource.volume = masterVolume * musicVolume;
        if (sfxSource != null)
            sfxSource.volume = masterVolume * fxVolume;
        if (voiceSource != null)
            voiceSource.volume = masterVolume * playerVolume;
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
            sfxSource.PlayOneShot(clip, fxVolume * masterVolume);
    }
}
