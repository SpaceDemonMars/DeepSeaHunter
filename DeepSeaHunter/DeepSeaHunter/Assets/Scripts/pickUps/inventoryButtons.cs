using TMPro;
using UnityEngine;

public class inventoryButtons : MonoBehaviour
{
    public int index;
    public Item item;
    [SerializeField] TMP_Text itemName;

    public void setText() { itemName.text = item.name; }
}
