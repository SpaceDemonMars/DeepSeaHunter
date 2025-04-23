using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class generalSAVE : ScriptableObject
{
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
