using UnityEngine;
using UnityEngine.UI;

public class Item : ScriptableObject
{
    public int itemId;
    public string itemName;
    public string itemDescription;
    public Image itemIcon;
    public virtual void useItem() {}
}
