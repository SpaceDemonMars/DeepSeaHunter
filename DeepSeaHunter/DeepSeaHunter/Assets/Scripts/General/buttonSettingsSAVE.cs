using UnityEngine;

[CreateAssetMenu]
public class buttonSettingsSAVE : ScriptableObject
{
    public float musicVol;
    [SerializeField] float defaultMusic;
    public float playerVol;
    [SerializeField] float defaultPlayer;
    public float fxVol;
    [SerializeField] float defaultFx;
    public bool isDefaults;
    
    public void restoreDefaults()
    {
        musicVol = defaultMusic;
        playerVol = defaultPlayer; 
        fxVol = defaultFx;
        isDefaults = true;
    }

    public bool checkDefaults() 
    { 
        isDefaults = (musicVol == defaultMusic && playerVol == defaultPlayer && fxVol == defaultFx);
        return isDefaults;
    }
}
