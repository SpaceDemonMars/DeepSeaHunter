using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu]
public class generalSAVE : ScriptableObject
{
    public playerSAVE pSave;

    public invenSAVE iSave;

    public buttonSettingsSAVE bSave;

    //from oxygen
    public int o2;
    public bool inO2Zone;

    //from Radio
    public bool radioOn;
    public bool inStatic;

    //from temperature
    public int gearBase;
    public int zoneMod;
}
