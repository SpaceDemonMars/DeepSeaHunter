using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class generalSAVE
{
    public playerSAVE pSave;

    public invenSAVE iSave;

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
