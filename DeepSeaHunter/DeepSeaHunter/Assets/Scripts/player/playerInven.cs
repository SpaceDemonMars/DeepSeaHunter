using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class playerInven : MonoBehaviour
{
    public int currencyFish;
    public int currencyScrap;

    List<Item> items;

    private void Start()
    {
        items = new List<Item>();
    }

    public void addItem(Item item)
    {
        if (item != null)
        {
            if (items.Count > 0) //inventory empty
            {
                items.Add(item);
            }
            else 
            {
                //find index to insert
                int index = 0;
                for (; index < items.Count; index++)
                {
                    if (items[index].itemId == item.itemId)
                    {
                        items[index].quantity += item.quantity; //increase quantity
                        break;
                    }
                    if (items[index].itemId > item.itemId) break; ////item not in inventory, insert @ index found
                }

                items.Insert(index, item); //insert item
            }
            GameManager.instance.loadInventory();
        }
    }

    public void removeItem(int index) //remove by index
    {
        if (index >= items.Count) return; //if index OOB exit
        items[index].useItem();
        items[index].quantity--;
        if (items[index].quantity <= 0) //out of item
        {
            items.RemoveAt(index);
            GameManager.instance.itemInfo.SetActive(false);
        }
        GameManager.instance.loadInventory();
    }

    public int getInvenSize() { return items.Count; }
    public Item getItem(int index) { return (index < items.Count) ? items[index] : null; }
    public int getQty(int index) { return (index < items[index].quantity) ? items[index].quantity : 0; }

    public int getFish() { return currencyFish; }
    public void setFish(int fish) { currencyFish = fish; setFishText(); }
    public void addFish(int fish) { currencyFish += fish; setFishText(); }
    public int getScrap() { return currencyScrap; }
    public void setScrap(int scrap) { currencyFish = scrap; setScrapText(); }
    public void addScrap(int scrap) { currencyScrap += scrap; setScrapText(); }
    public void setFishText() { GameManager.instance.fishText.text = currencyFish.ToString(); }
    public void setScrapText() { GameManager.instance.scrapText.text = currencyScrap.ToString(); }
}
