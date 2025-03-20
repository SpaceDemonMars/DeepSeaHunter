using UnityEngine;

public class meleePickUp : MonoBehaviour
{
    [SerializeField] meleeStats mweapon;
    private void OnTriggerEnter(Collider other)
    {
        IPickup pickupable = other.GetComponent<IPickup>();
        if (pickupable != null)
        {
            if (mweapon != null)
            {
                pickupable.getMeleeStats(mweapon);
            }
            Destroy(gameObject);
        }
    }
}


