using UnityEngine;

[CreateAssetMenu]
public class meleeStats : ScriptableObject
{
    public GameObject model;
    [Range(1, 10)] public int meleeDmg;
    [Range(1, 10)] public int meleeDist;
    [Range(0.1f, 2)] public float meleeRate;

    public ParticleSystem hitEffect;
    public AudioClip[] hitSound;
    [Range(0, 1)] public float hitVol;
}
