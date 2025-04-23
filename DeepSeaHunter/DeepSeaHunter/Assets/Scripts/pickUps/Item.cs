using UnityEngine;
using UnityEngine.UI;

public class Item : ScriptableObject
{
    public int itemId;
    public string itemName;
    public string itemDescription;
    public int quantity = 1; //this is the quantity being added to the player inventory
    public Image itemIcon;
    public virtual void useItem() {}

    public bool hasSameID(Item other)
    {
        return itemId == other.itemId;
    }
}
