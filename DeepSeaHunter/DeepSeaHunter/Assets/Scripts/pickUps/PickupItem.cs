using UnityEngine;

public class PickupItem : MonoBehaviour, IInteractable
{
    public string itemName;
    public int amount = 1;

    public void Interact()
    {
        Item newItem = new Item
        {
            itemName = itemName,
            quantity = amount,
            itemId = GenerateItemID(itemName)
        };

        playerInven.Instance.addItem(newItem);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Item newItem = new Item
            {
                itemName = itemName,
                quantity = amount,
                itemId = GenerateItemID(itemName)
            };

            playerInven.Instance.addItem(newItem);
            Destroy(gameObject);
        }
    }

    private int GenerateItemID(string itemName)
    {
        return itemName.GetHashCode();
    }
}
