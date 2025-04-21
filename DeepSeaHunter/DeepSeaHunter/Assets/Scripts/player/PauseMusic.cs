using UnityEngine;

public class PauseMusic : MonoBehaviour
{
    public AudioSource aud;
    [Range(0, 1)][SerializeField] float radioVol; //WHEN SETTINGS MADE, REMOVE SERIALIZE FIELD, FINISH SETVOLUME FUNC
    [SerializeField] AudioClip[] menuPlaylist;
    int listIndex;

    bool radioOn;

    void Start()
    {
        listIndex = Random.Range(0, menuPlaylist.Length);
        SetRadioVol(radioVol);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.isPaused) 
        {
            if (!radioOn) radioOn = true;
            playRadio();
        }
        else if (radioOn) radioOn = false;
    }

    void playRadio()
    {
        if (!aud.isPlaying) //if not playing (current clip ended)
        { 
            aud.clip = getNextClip();
            aud.Play();
        }
    }

    AudioClip getNextClip()
    {
        if (menuPlaylist.Length > 0)
        {
            //if current index is last for current playlist ==> reset
            if (listIndex >= menuPlaylist.Length - 1) listIndex = 0; 
            else listIndex++; //else increment index
            return menuPlaylist[listIndex]; //returns clip at [currPlaylist][index]
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
    public float GetRadioVol() { return radioVol; } 

}
