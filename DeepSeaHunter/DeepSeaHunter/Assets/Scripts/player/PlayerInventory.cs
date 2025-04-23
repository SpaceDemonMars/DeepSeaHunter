/*using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public Dictionary<string, int> items = new Dictionary<string, int>();

    public void AddItem(string itemName, int amount)
    {
        if (!items.ContainsKey(itemName))
        {
            items[itemName] = 0;
        }
        items[itemName] += amount;
        Debug.Log($"Added {amount} {itemName}(s). Total: {items[itemName]}");
    }

    public bool HasItem(string itemName, int amount)
    {
        return items.ContainsKey(itemName) && items[itemName] >= amount;
    }

    public void RemoveItem(string itemName, int amount)
    {
        if (HasItem(itemName, amount))
        {
            items[itemName] -= amount;
            if (items[itemName] <= 0)
            {
                items.Remove(itemName);
            }
            Debug.Log($"Removed {amount} {itemName}(s). Remaining: {items.GetValueOrDefault(itemName, 0)}");
        }
    }
}*/
