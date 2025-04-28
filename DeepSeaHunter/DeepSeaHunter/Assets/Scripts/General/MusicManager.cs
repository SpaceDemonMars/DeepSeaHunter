using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    public AudioSource musicSource;
    public AudioClip introMusic;
    public AudioClip demoSceneMusic;
    public AudioClip creditsMusic;
    public AudioClip goodEndingMusic;
    public AudioClip neutralEndingMusic;
    public AudioClip badEndingMusic;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
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
            case "Demo":
                PlayMusic(demoSceneMusic);
                break;
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
                break;
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
}
