using UnityEngine;

[CreateAssetMenu]
public class equipStats : ScriptableObject
{
    [Range(0, 3)] public int rank;

    [Range(0, 3)] public int gearMod;
}
