using UnityEngine;

public class Radio : MonoBehaviour
{
    public AudioSource aud;
    float radioVol = .5f;
    [SerializeField] AudioClip[] radioPlaylist;
    [SerializeField] AudioClip[] staticPlaylist;
    AudioClip[][] allRadioClips;
    int[] playlistIndex = { 0, 0 };
    bool radioOn;
    [SerializeField] bool inStatic;
    void Start()
    {
        allRadioClips = new AudioClip[2][];
        allRadioClips[0] = radioPlaylist;
        allRadioClips[1] = staticPlaylist;
        for (int i = 0; i < allRadioClips.Length - 1; i++)
            playlistIndex[i] = Random.Range(0, allRadioClips[i].Length);
        SetRadioVol(radioVol);
    }
    void Update()
    {
        if (GameManager.instance != null && !GameManager.instance.isPaused)
        {
            toggleRadio();
            playRadio();
        }
    }
    void playRadio()
    {
        if (!aud.isPlaying)
        {
            aud.clip = getNextClip();
            aud.Play();
        }
    }
    AudioClip getNextClip()
    {
        int playlist = inStatic ? 1 : 0;
        if ((inStatic && staticPlaylist.Length > 0) || (!inStatic && radioPlaylist.Length > 0))
        {
            if (playlistIndex[playlist] >= allRadioClips[playlist].Length - 1) playlistIndex[playlist] = 0;
            else playlistIndex[playlist]++;
            return allRadioClips[playlist][playlistIndex[playlist]];
        }
        else return null;
    }
    public void toggleRadio()
    {
        if (Input.GetButtonDown("Radio"))
        {
            setRadioOn(!radioOn);
        }
    }
    public void SetRadioVol(float volume)
    {
        radioVol = volume;
        if (radioOn) aud.volume = volume;
        else aud.volume = 0;
    }
    public float GetRadioVol()
    {
        return radioVol;
    }
    public bool getRadioOn()
    {
        return radioOn;
    }
    public void setRadioOn(bool on)
    {
        radioOn = on;
        SetRadioVol(radioVol);
        GameManager.instance.pauseMusic.togglePauseMusic(!radioOn);

        if (MusicManager.Instance != null)
            MusicManager.Instance.ToggleBackgroundMusic(!radioOn);
    }
    public bool getInStatic()
    {
        return inStatic;
    }
    public void setInStatic(bool value)
    {
        if (value != inStatic)
        {
            inStatic = value;
            aud.Stop();
            playRadio();
        }
    }
}