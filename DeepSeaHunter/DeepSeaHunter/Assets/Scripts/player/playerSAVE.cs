using System;
using UnityEditor;
using UnityEngine;

[Serializable]
public class playerSAVE
{
    //playerPosition
    public float playerPositionX;
    public float playerPositionY;
    public float playerPositionZ;

    //from playerController
    public int playerHP;
    public bool playerLightOn;
    //public meleeStats playerMelee;
    //public rangedStats playerRanged;
}
