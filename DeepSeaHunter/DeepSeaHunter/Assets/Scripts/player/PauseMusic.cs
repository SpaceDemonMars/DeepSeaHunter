using UnityEngine;

public class PauseMusic : MonoBehaviour
{
    public AudioSource aud;
    float radioVol = .5f;   //sorry for the lack of clarity, the previous comment
                            //meant LITERALLY to only remove the [SerializeField]
                            //not to remove this entirely
    [SerializeField] AudioClip[] menuPlaylist;
    int listIndex;
    bool radioOn;
    void Start()
    {
        listIndex = Random.Range(0, menuPlaylist.Length);
        radioOn = true;
        SetRadioVol(radioVol);
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
        SetRadioVol(radioVol);
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
}
