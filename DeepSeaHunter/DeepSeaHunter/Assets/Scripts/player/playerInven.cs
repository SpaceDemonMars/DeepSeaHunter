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
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    //Save/Load
    public invenSAVE saveInven()
    {
        invenSAVE iSave = new()
        {
            fish = currencyFish,
            scrap = currencyScrap,
            items = new List<ItemSAVE>()
        };
        //save items list
        for (int i = 0; i < items.Count; i++)
            iSave.items.Add(convertToSave(items[i], qty[i]));
        
  //      Debug.Log("Success: Save (Inven)");
        return iSave;
    }
    public void loadInven(invenSAVE iSave)
    {
        currencyFish = iSave.fish;
        setFishText();
        currencyScrap = iSave.scrap;
        setScrapText();
        //load saved items
        clearInventory();
        foreach (ItemSAVE save in iSave.items)
            addItem(convertFromSave(save));

        GameManager.instance.loadInventory();
  //      Debug.Log("Success: Load (Inven)");
    }
    
    ItemSAVE convertToSave(Item item, int qty)
    {
        ItemSAVE save = new ItemSAVE();

        save.itemId = item.itemId;
        save.itemName = item.itemName;
        save.itemDescription = item.itemDescription;
        save.quantity = qty;
        //save.itemIcon = item.itemIcon;
        save.fishValue = item.fishValue;
        save.scrapValue = item.scrapValue;
        save.hp = item.hp;
        save.o2 = item.o2;
        save.sanity = item.sanity;

        return save;
    }
    Item convertFromSave(ItemSAVE item)
    {
        Item save = ScriptableObject.CreateInstance<Item>();

        save.itemId = item.itemId;
        save.itemName = item.itemName;
        save.itemDescription = item.itemDescription;
        save.quantity = item.quantity;
        //save.itemIcon = item.itemIcon;
        save.fishValue = item.fishValue;
        save.scrapValue = item.scrapValue;
        save.hp = item.hp;
        save.o2 = item.o2;
        save.sanity = item.sanity;

        return save;
    }

    //Add/Remove(Use)
    public void addItem(Item item)
    {
        if (item != null)
        {
            if (items.Count == 0) //inventory empty
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
                    if (items[index].itemId == item.itemId)
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
    public void removeItem(int index) //remove by index
    {
        if (index >= items.Count) return; //if index OOB exit
        useItem(items[index]);
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

    void useItem(Item item)
    {
        //currency
        addFish(item.fishValue);
        addScrap(item.scrapValue);
        //consumable
        GameManager.instance.playerScript.takeDamage(item.hp * -1, 0);
        GameManager.instance.o2.modifyO2(item.o2);
        //update sanity
    }
    void clearInventory()
    {
        items.Clear();
        qty.Clear();
        GameManager.instance.loadInventory();
    }

    //getters/setters
    public int getInvenSize() { return items.Count; }
    
    public Item getItem(int index) { return (index < items.Count) ? items[index] : null; }
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
    
    public int getQty(int index) { return (index < qty.Count) ? qty[index] : 0; }
    public int getItemQuantity(string itemName)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].itemName == itemName)
                return qty[i];
        }
        return 0;
    }
    
    bool displayingThisItem(Item currItem)
    {
        return GameManager.instance.itemInfoName.text == currItem.itemName;
    }

    public int getFish() { return currencyFish; }
    public void setFish(int fish) { currencyFish = fish; setFishText(); }
    public void addFish(int fish) { currencyFish += fish; setFishText(); }
    public int getScrap() { return currencyScrap; }
    public void setScrap(int scrap) { currencyScrap = scrap; setScrapText(); }
    public void addScrap(int scrap) { currencyScrap += scrap; setScrapText(); }
    public void setFishText() { GameManager.instance.fishText.text = currencyFish.ToString(); }
    public void setScrapText() { GameManager.instance.scrapText.text = currencyScrap.ToString(); }

    public bool attemptFishTrade(int reqFish)
    {
        if (currencyFish >= reqFish)
        {
            setFish(currencyFish - reqFish);
            return true;
        }
        return false;
    }
    public bool attemptScrapTrade(int reqScrap)
    {
        if (currencyScrap >= reqScrap)
        {
            setScrap(currencyScrap - reqScrap);
            return true;
        }
        return false;
    }

    private int GenerateItemID(string itemName) { return itemName.GetHashCode(); }
}
