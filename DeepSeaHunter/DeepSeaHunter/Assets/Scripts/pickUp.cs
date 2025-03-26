using UnityEngine;

public class pickUp : MonoBehaviour
{
    [SerializeField] meleeStats knife;
    [SerializeField] rangedStats harpoon;


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

            //ALWAYS
            Destroy(gameObject);
        }

    }
}
