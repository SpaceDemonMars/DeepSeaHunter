using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    public AudioSource musicSource;
    public AudioClip introMusic;
//    public AudioClip demoSceneMusic;
    public AudioClip creditsMusic;
    public AudioClip goodEndingMusic;
    public AudioClip neutralEndingMusic;
    public AudioClip badEndingMusic;

    [Range(0f, 1f)] public float musicVolume = 1.0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;

        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            musicVolume = PlayerPrefs.GetFloat("MusicVolume");
        }

        ApplyVolume();
    }
        else
        {
            Destroy(gameObject);
}
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "Startup":
            case "IntroScene":
            case "MainMenu":
            case "LoadingScene":
                PlayMusic(introMusic);
                break;
 //           case "Demo":
   //             PlayMusic(demoSceneMusic);
     //           break;
            case "Credits":
                PlayMusic(creditsMusic);
                break;
            case "Good Ending":
                PlayMusic(goodEndingMusic);
                break;
            case "Neutral Ending":
                PlayMusic(neutralEndingMusic);
                break;
            case "Bad Ending":
                PlayMusic(badEndingMusic);
                break;
            default:
                StopMusic();
                break;
        }
    }
    public void StopMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;

        if (musicSource.clip != clip)
        {
            musicSource.clip = clip;
            musicSource.Play();
        }
    }
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        ApplyVolume();

        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.Save();
    }

    private void ApplyVolume()
    {
        if (musicSource != null)
            musicSource.volume = musicVolume;
    }
    public void ToggleBackgroundMusic(bool play)
    {
        if (musicSource == null) return;

        if (play)
        {
            if (!musicSource.isPlaying)
                musicSource.Play();
        }
        else
        {
            if (musicSource.isPlaying)
                musicSource.Pause();
        }
    }
}
