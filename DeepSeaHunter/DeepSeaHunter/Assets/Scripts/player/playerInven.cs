using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class playerInven : MonoBehaviour
{
    public int currencyFish;
    public int currencyScrap;

    public static playerInven Instance;

    private List<Item> items = new();
    private List<int> qty = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        items = new List<Item>();
        qty = new List<int>();
    }

    // Save/Load
    public invenSAVE saveInven()
    {
        invenSAVE save = new()
        {
            fish = currencyFish,
            scrap = currencyScrap,
            items = new List<ItemSAVE>()
        };

        for (int i = 0; i < items.Count; i++)
        {
            save.items.Add(convertToSave(items[i], qty[i]));
        }

        return save;
    }

    public void loadInven(invenSAVE data)
    {
        currencyFish = data.fish;
        currencyScrap = data.scrap;
        setFishText();
        setScrapText();

        clearInventory();

        foreach (ItemSAVE i in data.items)
        {
            addItem(convertFromSave(i));
        }

        GameManager.instance.loadInventory();
    }

    ItemSAVE convertToSave(Item item, int qty)
    {
        return new ItemSAVE
        {
            itemId = item.itemId,
            itemName = item.itemName,
            itemDescription = item.itemDescription,
            quantity = qty,
            fishValue = item.fishValue,
            scrapValue = item.scrapValue,
            hp = item.hp,
            o2 = item.o2,
            sanity = item.sanity
        };
    }

    Item convertFromSave(ItemSAVE i)
    {
        Item item = ScriptableObject.CreateInstance<Item>();
        item.itemId = i.itemId;
        item.itemName = i.itemName;
        item.itemDescription = i.itemDescription;
        item.quantity = i.quantity;
        item.fishValue = i.fishValue;
        item.scrapValue = i.scrapValue;
        item.hp = i.hp;
        item.o2 = i.o2;
        item.sanity = i.sanity;
        return item;
    }

    // Add Item (by object)
    public void addItem(Item item)
    {
        if (item == null) return;

        int index = items.FindIndex(i => i.itemId == item.itemId);
        if (index >= 0)
        {
            qty[index] += item.quantity;
            updateUI(index);
        }
        else
        {
            Item copy = ScriptableObject.Instantiate(item);
            items.Add(copy);
            qty.Add(copy.quantity);
        }

        GameManager.instance.loadInventory();
        GameManager.instance.ShowItemPopup($"+{item.quantity} {item.itemName}");

    }

    // Add item by name + amount (fallback logic)
    public void AddItem(string itemName, int amount)
    {
        int index = items.FindIndex(i => i.itemName == itemName);
        if (index >= 0)
        {
            qty[index] += amount;
            updateUI(index);
        }
        else
        {
            Item newItem = ScriptableObject.CreateInstance<Item>();
            newItem.itemName = itemName;
            newItem.quantity = amount;
            newItem.itemId = GenerateItemID(itemName);
            items.Add(newItem);
            qty.Add(amount);
        }
        GameManager.instance.loadInventory();
        GameManager.instance.ShowItemPopup($"+{amount} {itemName}");
    }

    public bool HasItem(string itemName, int requiredAmount)
    {
        int index = items.FindIndex(i => i.itemName == itemName);
        return (index >= 0 && qty[index] >= requiredAmount);
    }

    public int getItemQuantity(string itemName)
    {
        int index = items.FindIndex(i => i.itemName == itemName);
        return index >= 0 ? qty[index] : 0;
    }

    public void RemoveItem(string itemName, int amount)
    {
        int index = items.FindIndex(i => i.itemName == itemName);
        if (index < 0) return;

        qty[index] -= amount;

        if (qty[index] <= 0)
        {
            items.RemoveAt(index);
            qty.RemoveAt(index);
            GameManager.instance.itemInfo.SetActive(false);
        }
        else updateUI(index);

        GameManager.instance.loadInventory();
    }

    public void removeItem(int index)
    {
        if (index >= items.Count) return;
        useItem(items[index]);

        qty[index]--;
        if (qty[index] <= 0)
        {
            items.RemoveAt(index);
            qty.RemoveAt(index);
            GameManager.instance.itemInfo.SetActive(false);
        }
        else updateUI(index);

        GameManager.instance.loadInventory();
    }

    void useItem(Item item)
    {
        addFish(item.fishValue);
        addScrap(item.scrapValue);
        GameManager.instance.playerScript.takeDamage(item.hp * -1, 0);
        GameManager.instance.o2.modifyO2(item.o2);
        // todo: handle sanity logic
    }

    void updateUI(int index)
    {
        if (displayingThisItem(items[index]))
            GameManager.instance.itemInfoQty.text = "x" + qty[index].ToString();
    }

    public void clearInventory()
    {
        items.Clear();
        qty.Clear();
        GameManager.instance.loadInventory();
    }

    public int getInvenSize() => items.Count;
    public Item getItem(int index) => index < items.Count ? items[index] : null;
    public int getQty(int index) => index < qty.Count ? qty[index] : 0;

    bool displayingThisItem(Item currItem)
    {
        return GameManager.instance.itemInfoName.text == currItem.itemName;
    }

    // Currency management
    public int getFish() => currencyFish;
    public void setFish(int fish) { currencyFish = fish; setFishText(); }
    public void addFish(int fish) { currencyFish += fish; setFishText(); }

    public int getScrap() => currencyScrap;
    public void setScrap(int scrap) { currencyScrap = scrap; setScrapText(); }
    public void addScrap(int scrap) { currencyScrap += scrap; setScrapText(); }

    public void setFishText() => GameManager.instance.fishText.text = currencyFish.ToString();
    public void setScrapText() => GameManager.instance.scrapText.text = currencyScrap.ToString();

    public bool attemptFishTrade(int reqFish)
    {
        if (currencyFish >= reqFish)
        {
            currencyFish -= reqFish;
            setFishText();
            return true;
        }
        return false;
    }

    public bool attemptScrapTrade(int reqScrap)
    {
        if (currencyScrap >= reqScrap)
        {
            currencyScrap -= reqScrap;
            setScrapText();
            return true;
        }
        return false;
    }

    private int GenerateItemID(string itemName)
    {
        return itemName.GetHashCode();
    }

    public void DebugPrintInven()
    {
        if (items.Count > 0)
        {
            foreach (Item i in items)
            {
                Debug.Log(i.name);
            }
        }
    }
}
