using UnityEngine;
using System.Collections;

public class damage : MonoBehaviour
{
    enum damageType {  moving, stationary, dot, entangling, poison }
    [SerializeField] damageType type;
    [SerializeField] Rigidbody rb;

    [Range(0, 10)][SerializeField] int dmgAmount;
    [Range(0, 10)][SerializeField] float dmgTime;
    [Range(10, 45)][SerializeField] int speed;
    [Range(0, 5)][SerializeField] int destroyTime;
    [Range(0, 10)][SerializeField] float entangleDuration;
    [Range(0, 5)][SerializeField] int slowFactor;

    [Range(1, 10)][SerializeField] int poisonDamage;
    [Range(1, 10)][SerializeField] float poisonDuration;
    [Range(0f, 2f)][SerializeField] float poisonInterval;

    bool isDamaging;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (type == damageType.moving)
        {
            rb.linearVelocity = transform.forward*speed;
            Destroy(gameObject, destroyTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;
        IDamage dmg = other.GetComponent<IDamage>();
        playerController player = other.GetComponent<playerController>();

        if (dmg != null)
        {
            if (type == damageType.stationary || type == damageType.moving)
            {
                dmg.takeDamage(dmgAmount);
                if (type == damageType.moving)
                    Destroy(gameObject);
            }
            else if (type == damageType.dot)
            {
                StartCoroutine(damageOther(dmg));
            }
            else if (type == damageType.poison && player != null)
            {
                StartCoroutine(poisonPlayer(player));
            }
        }
        if (player != null && type == damageType.entangling)
        {
            StartCoroutine(entanglePlayer(player));
        }
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
    private void OnTriggerExit(Collider other)
    {
        if (type == damageType.dot)
        {
            isDamaging = false;
        }
    }

    IEnumerator damageOther(IDamage d)
    {
        isDamaging = true;
        d.takeDamage(dmgAmount);
        yield return new WaitForSeconds(dmgTime);
        isDamaging = false;
    }
    private IEnumerator entanglePlayer(playerController player)
    {
        if (!player.isTangled)
        {
            player.isTangled = true;
            player.speed /= slowFactor;
            player.jumpStr /= slowFactor;
            player.dashStr /= slowFactor;
        }
        yield return new WaitForSeconds(entangleDuration);
        if (player.isTangled)
        {
            player.speed *= slowFactor;
            player.jumpStr *= slowFactor;
            player.dashStr *= slowFactor;
            player.isTangled = false;
        }
    }
    private IEnumerator poisonPlayer(playerController player)
    {
        float elapsedTime = 0f;

        while (elapsedTime < poisonDuration)
        {
            player.takeDamage(poisonDamage);
            yield return new WaitForSeconds(poisonInterval);
            elapsedTime += poisonInterval;
        }
    }
}
