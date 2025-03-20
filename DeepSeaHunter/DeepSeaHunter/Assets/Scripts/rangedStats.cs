using UnityEngine;

[CreateAssetMenu
    ]
public class rangedStats : ScriptableObject
{
    public GameObject model;
    [Range(1, 10)] public int shootDamage;
    [Range(5, 100)] public int shootDist;
    [Range(0.1f, 2)] public float shootRate;
    [HideInInspector] public int ammoCur;
    [Range(5, 50)] public int ammoMax;

    public ParticleSystem hitEffect;
    public AudioClip[] shootSound;
    [Range(0, 1)] public float shootVol;

}
