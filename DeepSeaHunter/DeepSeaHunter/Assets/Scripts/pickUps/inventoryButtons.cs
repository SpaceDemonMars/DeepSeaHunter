using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class inventoryButtons : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int index;
    public Item item;
    [SerializeField] TMP_Text itemName;

    public void setText() { itemName.text = item.name; }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GameManager.instance.itemInfoName.text = item.itemName;
        GameManager.instance.itemInfoDesc.text = item.itemDescription;
        GameManager.instance.itemInfoQty.text = "x" + GameManager.instance.playerScript.inven.getQty(index).ToString();
        GameManager.instance.itemInfo.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GameManager.instance.itemInfo.SetActive(false);
    }
}
