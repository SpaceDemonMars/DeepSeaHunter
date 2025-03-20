using UnityEngine;

public class rangedPickUp : MonoBehaviour
{
    [SerializeField] rangedStats rweapon;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (rweapon != null)
        {
            rweapon.ammoCur = rweapon.ammoMax;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        IPickup pickupable = other.GetComponent<IPickup>();
        if (pickupable != null)
        {
            if (rweapon != null)
            {
                pickupable.getRangedStats(rweapon);
            }
            Destroy(gameObject);
        }
    }
}