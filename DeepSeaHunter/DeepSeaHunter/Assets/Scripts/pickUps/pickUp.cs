using UnityEngine;

public class pickUp : MonoBehaviour
{
    [SerializeField] Item itemPickup;
    [SerializeField] meleeStats knife;
    [SerializeField] rangedStats harpoon;
    [SerializeField] equipStats gear;


    private void OnTriggerEnter(Collider other)
    {
        IPickup pickupable = other.GetComponent<IPickup>();
        if (pickupable != null)
        {
            //health
            //do nothing, damage script should handle

            //knife
            if (knife != null)
            {
                pickupable.getMeleeStats(knife);
            }

            //ranged
            if (harpoon != null)
            {
                pickupable.getRangedStats(harpoon);
            }

            //gear
            if (gear != null)
                pickupable.getEquipStats(gear);

            //ALWAYS
            Destroy(gameObject);
        }
        playerInven inven = other.GetComponent<playerInven>();
        if (inven != null)
        {
            inven.addItem(itemPickup);
        }
    }
}