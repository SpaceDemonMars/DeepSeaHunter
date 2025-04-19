using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class playerInven : MonoBehaviour
{
    public int currencyFish;
    public int currencyScrap;

    List<Item> items;
    List<int> quantity; //when adding/inserting ALWAYS start @ 1

    private void Start()
    {
        items = new List<Item>();
        quantity = new List<int>();
        Debug.Log(items.Count);
    }

    public void addItem(Item item)
    {
        if (item == null) return;
        if (items.Count > 0) //inventory empty
        {
            items.Add(item);
            quantity.Add(1);
        }
        else if (items.Contains(item)) //if item is in inventory
        {
            quantity[items.IndexOf(item)]++; //increase quantity
        }
        else //item not in inventory
        {
            //find index to insert
            int index = 0;
            for (; index < items.Count; index++)
            {
                if (items[index].itemId > item.itemId) break; //insert index found, TEST THIS!!!!
            }

            items.Insert(index, item); //insert item
            quantity.Insert(index, 1); //insert tracker at same index
        }
        GameManager.instance.loadInventory();
    }

    public void removeItem(int index) //remove by index
    {
        if (index >= items.Count) return; //if index OOB exit
        items[index].useItem();
        quantity[index]--;
        if (quantity[index] <= 0) //out of item
        {
            items.RemoveAt(index);
            quantity.RemoveAt(index);
        }
        GameManager.instance.loadInventory();
    }

    public int getInvenSize() { return quantity.Count; }
    public Item getItem(int index) { return (index < items.Count) ? items[index] : null; }

    public int getFish() { return currencyFish; }
    public void setFish(int fish) { currencyFish = fish; setFishText(); }
    public void addFish(int fish) { currencyFish += fish; setFishText(); }
    public int getScrap() { return currencyScrap; }
    public void setScrap(int scrap) { currencyFish = scrap; setScrapText(); }
    public void addScrap(int scrap) { currencyFish += scrap; setScrapText(); }
    public void setFishText() { GameManager.instance.fishText.text = currencyFish.ToString(); }
    public void setScrapText() { GameManager.instance.scrapText.text = currencyScrap.ToString(); }
}
