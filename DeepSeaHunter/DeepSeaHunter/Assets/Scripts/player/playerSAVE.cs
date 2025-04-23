using UnityEditor;
using UnityEngine;

[CreateAssetMenu]
public class playerSAVE : ScriptableObject
{
    public Vector3 playerPosition;

    //from playerController
    public int playerHP;
    public bool playerLightOn;
    public meleeStats playerMelee;
    public rangedStats playerRanged;
}
