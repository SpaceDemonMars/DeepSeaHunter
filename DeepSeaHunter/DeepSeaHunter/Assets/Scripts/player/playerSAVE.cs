using UnityEngine;

[CreateAssetMenu]
public class playerSAVE : ScriptableObject
{
    public Transform playerPosition;

    //from playerController
    public int playerHP;
    public float playerSpeed;
    public float playerDashStr;
    public float playerJumpStr;
    public bool playerLightOn;
    public meleeStats playerMelee;
    public rangedStats playerRanged;
}
