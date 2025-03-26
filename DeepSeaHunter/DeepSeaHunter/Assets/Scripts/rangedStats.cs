using UnityEngine;

[CreateAssetMenu
    ]
public class rangedStats : ScriptableObject
{
    //public GameObject model;
    [Range(0, 3)] public int rank;

    //[Range(1, 10)] public int shootDamage;
    [Range(5, 300)] public int grappleDist;
    [Range(1, 60)] public int grappleSpeed;
    //[Range(0.1f, 2)] public float shootRate;
    /*[HideInInspector] public int ammoCur;
    [Range(5, 50)] public int ammoMax;*/

    //public ParticleSystem hitEffect;
    public AudioClip[] grappleSound;
    [Range(0, 1)] public float grappleVol;

}
