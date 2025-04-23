using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using System;
using System.Reflection;

public class playerInven : MonoBehaviour
{
    public int currencyFish;
    public int currencyScrap;

    List<Item> items;
    List<int> qty;

    public static playerInven Instance;

    private void Start()
    {
        items = new List<Item>();
        qty = new List<int>();
    }

    public void addItem(Item item)
    {
        if (item != null)
        {
            if (items.Count < 0) //inventory empty
            {
                items.Add(item);
                qty.Add(item.quantity);
            }
            else 
            {
                //find index to insert
                int index = 0;
                for (; index < items.Count; index++)
                {
                    if (items[index].hasSameID(item))
                    {
                        qty[index] += item.quantity; //increase quantity
                        GameManager.instance.loadInventory();
                        if (displayingThisItem(item))
                            GameManager.instance.itemInfoQty.text = "x" + qty[index].ToString();
                        return;
                    }
                    if (items[index].itemId > item.itemId) break; ////item not in inventory, insert @ index found
                }

                items.Insert(index, item); //insert item
                qty.Insert(index, item.quantity);
            }
            GameManager.instance.loadInventory();
        }
    }
    public int getItemQuantity(string itemName)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].itemName == itemName)
                return qty[i];
        }
        return 0;
    }
    public bool HasItem(string itemName, int requiredAmount)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].itemName == itemName && qty[i] >= requiredAmount)
            {
                return true;
            }
        }
        return false;
    }

    public void RemoveItem(string itemName, int amount)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].itemName == itemName)
            {
                qty[i] -= amount;
                if (qty[i] <= 0)
                {
                    items.RemoveAt(i);
                    qty.RemoveAt(i);
                    GameManager.instance.itemInfo.SetActive(false);
                }
                else if (displayingThisItem(items[i]))
                    GameManager.instance.itemInfoQty.text = "x" + qty[i].ToString();
                GameManager.instance.loadInventory();
                return;
            }
        }
    }
    public void AddItem(string itemName, int amount)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].itemName == itemName)
            {
                qty[i] += amount;
                GameManager.instance.loadInventory();
                if (displayingThisItem(items[i]))
                    GameManager.instance.itemInfoQty.text = "x" + qty[i].ToString();
                return;
            }
        }

        Item newItem = new Item
        {
            itemName = itemName,
            quantity = amount,
            itemId = GenerateItemID(itemName)
        };
        items.Add(newItem);
        qty.Add(amount);
        GameManager.instance.loadInventory();
    }

    private int GenerateItemID(string itemName)
    {
        return itemName.GetHashCode(); 
    }

    public void removeItem(int index) //remove by index
    {
        if (index >= items.Count) return; //if index OOB exit
        items[index].useItem();
        qty[index]--;
        if (qty[index] <= 0) //out of item
        {
            items.RemoveAt(index);
            qty.RemoveAt(index);
            GameManager.instance.itemInfo.SetActive(false);
        }
        else if (displayingThisItem(items[index]))
            GameManager.instance.itemInfoQty.text = "x" + qty[index].ToString();
        GameManager.instance.loadInventory();
    }

    public int getInvenSize() { return items.Count; }
    public Item getItem(int index) { return (index < items.Count) ? items[index] : null; }
    public int getQty(int index) { return (index < qty.Count) ? qty[index] : 0; }

    bool displayingThisItem(Item currItem)
    {
        return GameManager.instance.itemInfoName.text == currItem.itemName;
    }

    public int getFish() { return currencyFish; }
    public void setFish(int fish) { currencyFish = fish; setFishText(); }
    public void addFish(int fish) { currencyFish += fish; setFishText(); }
    public int getScrap() { return currencyScrap; }
    public void setScrap(int scrap) { currencyFish = scrap; setScrapText(); }
    public void addScrap(int scrap) { currencyScrap += scrap; setScrapText(); }
    public void setFishText() { GameManager.instance.fishText.text = currencyFish.ToString(); }
    public void setScrapText() { GameManager.instance.scrapText.text = currencyScrap.ToString(); }
}
