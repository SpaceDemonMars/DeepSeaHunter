using UnityEngine;

public class PickupItem : MonoBehaviour, IInteractable
{
    public string itemName;  
    public int amount = 1;

    public void Interact()
    {
        PlayerInventory.Instance.AddItem(itemName, amount);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInventory.Instance.AddItem(itemName, amount);
            Destroy(gameObject); 
        }
    }
}
