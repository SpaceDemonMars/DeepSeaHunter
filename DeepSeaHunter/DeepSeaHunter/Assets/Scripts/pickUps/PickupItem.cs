using UnityEngine;

public class PickupItem : MonoBehaviour, IInteractable
{
    [Header("Item Pickup Settings")]
    public string itemName;
    public int amount = 1;
    public Item itemPickup;

    [Header("Weapon Pickup (Optional)")]
    public meleeStats knife;
    public rangedStats harpoon;

    public void Interact()
    {
        TryPickup(GameManager.instance.playerScript.inven);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInven inven = other.GetComponent<playerInven>();
            if (inven != null)
            {
                TryPickup(inven);
            }
        }
    }

    private void TryPickup(playerInven inven)
    {
        // Handle weapon pickups
        IPickup pickupable = inven.GetComponent<IPickup>();
        if (pickupable != null)
        {
            if (knife != null)
                pickupable.getMeleeStats(knife);

            if (harpoon != null)
                pickupable.getRangedStats(harpoon);
        }

        // Handle item pickups
        if (itemPickup != null)
        {
            inven.addItem(itemPickup);
        }
        else if (!string.IsNullOrEmpty(itemName))
        {
            Item newItem = new Item
            {
                itemName = itemName,
                quantity = amount,
                itemId = GenerateItemID(itemName)
            };
            inven.addItem(newItem);
        }

        Destroy(gameObject);
    }

    private int GenerateItemID(string itemName)
    {
        return itemName.GetHashCode();
    }
}
