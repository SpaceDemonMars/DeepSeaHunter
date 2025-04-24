using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ItemSAVE
{
    public int itemId;
    public string itemName;
    public string itemDescription;
    public int quantity = 1; //this is the quantity being added to the player inventory
    //public Image itemIcon;
    [Header("Currency")]
    public int fishValue = 0;
    public int scrapValue = 0;
    [Header("Consumable")]
    public int hp = 0;
    public int o2 = 0;
    public int sanity = 0;
}
