using UnityEngine;

public class Radio : MonoBehaviour
{
    [SerializeField] AudioSource aud;
    [Range(0, 1)][SerializeField] float radioVol; //WHEN SETTINGS MADE, REMOVE SERIALIZE FIELD, FINISH SETVOLUME FUNC
    [SerializeField] AudioClip[] radioPlaylist;
    [SerializeField] AudioClip[] staticPlaylist;
    AudioClip[][] allRadioClips;
    int[] playlistIndex = { 0, 0 };

    [SerializeField] bool radioOn;
    public bool inStatic;
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
    }

    void toggleRadio()
    {
        if (Input.GetButtonDown("Radio"))
        {
            radioOn = !radioOn;
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
}
