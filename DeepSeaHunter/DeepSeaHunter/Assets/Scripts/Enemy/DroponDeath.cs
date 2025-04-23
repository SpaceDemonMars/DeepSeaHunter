using UnityEngine;

public class DropOnDeath : MonoBehaviour
{
    public GameObject pickupPrefab;    
    public Transform dropPoint;        
    public string itemNameOverride;   

    private bool hasDropped = false;  

    public void Drop()
    {
        if (hasDropped) return;
        hasDropped = true;

        if (pickupPrefab != null)
        {
            Vector3 spawnPosition = transform.position;
            if (dropPoint != null)
            {
                spawnPosition = dropPoint.position;
            }

            GameObject spawnedPickup = Instantiate(pickupPrefab, spawnPosition, Quaternion.identity);

            PickupItem pickupScript = spawnedPickup.GetComponent<PickupItem>();
            if (pickupScript != null)
            {
                pickupScript.itemName = string.IsNullOrEmpty(itemNameOverride) ? gameObject.name : itemNameOverride;
            }
        }
    }
}
