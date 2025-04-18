using UnityEngine;

public class Radio : MonoBehaviour
{
    [SerializeField] AudioSource aud;
    [Range(0, 1)][SerializeField] float radioVol; //WHEN SETTINGS MADE, REMOVE SERIALIZE FIELD, FINISH SETVOLUME FUNC
    [SerializeField] AudioClip[] radioPlaylist;
    [SerializeField] AudioClip[] staticPlaylist;
    [SerializeField] AudioClip[] onOffClick;
    [Range(0, 1)][SerializeField] float clickVol;
    AudioClip[][] allRadioClips;
    int[] playlistIndex = { 0, 0 };

    bool radioOn;
    [SerializeField] bool inStatic; //in a zone with static

    void Start()
    {
        allRadioClips = new AudioClip[2][];
        allRadioClips[0] = radioPlaylist;
        allRadioClips[1] = staticPlaylist;
        SetRadioVol();
    }

    // Update is called once per frame
    void Update()
    {
        toggleRadio();
        playRadio();
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
        int playlist = (inStatic) ? 1 : 0;
        if ((inStatic && staticPlaylist.Length > 0) || //playing static && playlist populated
            (!inStatic && radioPlaylist.Length > 0)) //or playing radio && playlist populated
        {
            //if current index is last for current playlist ==> reset
            if (playlistIndex[playlist] >= allRadioClips[playlist].Length - 1) playlistIndex[playlist] = 0; 
            else playlistIndex[playlist]++; //else increment index
            return allRadioClips[playlist][playlistIndex[playlist]]; //returns clip at [currPlaylist][index]
        }
        else return null;
    }

    void toggleRadio()
    {
        if (Input.GetButtonDown("Radio"))
        {
            radioOn = !radioOn;
            aud.PlayOneShot(onOffClick[Random.Range(0, onOffClick.Length)], clickVol);
            SetRadioVol();
        }
    }

    void SetRadioVol()
    {
        //radioVol = radio volume from menu
        if (radioOn)
            aud.volume = radioVol; 
        else aud.volume = 0;
    }

    bool getInStatic() { return inStatic; }
    public void setInStatic(bool value) 
    {
        inStatic = value;
        aud.Stop(); //stops current clip so that correct clip will play
        playRadio(); //starts new clip 
    }
}
