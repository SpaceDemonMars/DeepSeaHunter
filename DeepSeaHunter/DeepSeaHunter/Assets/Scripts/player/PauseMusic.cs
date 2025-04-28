using UnityEngine;

public class PauseMusic : MonoBehaviour
{
    public AudioSource aud;
    [SerializeField] AudioClip[] menuPlaylist;
    int listIndex;
    bool radioOn;
    void Start()
    {
        listIndex = Random.Range(0, menuPlaylist.Length);
        radioOn = true;
        SetRadioVol(AudioManager.Instance != null ? AudioManager.Instance.musicVolume : 1f);
    }
    void Update()
    {
        playRadio();
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
        if (menuPlaylist.Length > 0)
        {
            if (listIndex >= menuPlaylist.Length - 1) listIndex = 0;
            else listIndex++;
            return menuPlaylist[listIndex];
        }
        else return null;
    }
    public void togglePauseMusic(bool on)
    {
        radioOn = on;
        SetRadioVol(AudioManager.Instance != null ? AudioManager.Instance.musicVolume : 1f);
    }
    public void SetRadioVol(float volume)
    {
        if (aud == null) return;
        if (radioOn) aud.volume = volume;
        else aud.volume = 0;
    }
    public float GetRadioVol()
    {
        return aud != null ? aud.volume : 0f;
    }
}
