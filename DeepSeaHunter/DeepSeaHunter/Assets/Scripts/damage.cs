using UnityEngine;
using System.Collections;

public class damage : MonoBehaviour
{
    enum damageType {  moving, stationary, dot }
    [SerializeField] damageType type;
    [SerializeField] Rigidbody rb;

    [Range(1, 10)][SerializeField] int dmgAmount;
    [Range(0.25f, 1)][SerializeField] float dmgTime;
    [Range(10, 45)][SerializeField] int speed;
    [Range(1, 5)][SerializeField] int destroyTime;

    bool isDamaging;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (type == damageType.moving)
        {
            rb.linearVelocity = transform.forward;
            Destroy(gameObject, dmgTime);
        }
    }

    IEnumerator damageOther(IDamage d)
    {
        isDamaging = true;
        d.takeDamage(dmgAmount);
        yield return new WaitForSeconds(dmgTime);
        isDamaging = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;
        IDamage dmg = other.GetComponent<IDamage>();
        if (dmg !=  null && type != damageType.dot) 
            dmg.takeDamage(dmgAmount);
        if (type == damageType.moving)
            Destroy(gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.isTrigger)
            return;
        IDamage dmg = other.GetComponent<IDamage>();
        if (dmg != null && type == damageType.dot)
        {
            if (!isDamaging)
                StartCoroutine(damageOther(dmg));
        }
    }
}
