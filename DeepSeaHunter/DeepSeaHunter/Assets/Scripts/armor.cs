using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class armor : MonoBehaviour, IDamage
{
    [SerializeField] EnemyArmored parent;
    [SerializeField] EnemyBoss boss;
    [SerializeField] Material flashDamage;
    [SerializeField] Material damaged;
    [SerializeField] Material parentDamaged;
    [SerializeField] int HP;

    Material parentMaterial;

    void Start()
    {
        if (boss != null)
            parentMaterial = boss.model.material;
        else
            parentMaterial = parent.model.material;
    }

    public void takeDamage(int damage)
    {
        HP -= damage;
        StartCoroutine(flashWhite());
        if (boss != null)
        {
            boss.agent.SetDestination(GameManager.instance.player.transform.position);

            if (HP <= 0)
            {
                boss.model.material = damaged;
                boss.flashDamage = parentDamaged;
                Destroy(gameObject);
            }
        }
        else
        {
            parent.agent.SetDestination(GameManager.instance.player.transform.position);

            if (HP <= 0)
            {
                parent.model.material = damaged;
                parent.flashDamage = parentDamaged;
                Destroy(gameObject);
            }
        }
    }

    IEnumerator flashWhite()
    {
        if (boss != null)
        {
            boss.model.material = flashDamage;
            yield return new WaitForSeconds(.1f);
            boss.model.material = parentMaterial;
        }
        else
        {
            parent.model.material = flashDamage;
            yield return new WaitForSeconds(.1f);
            parent.model.material = parentMaterial;
        }
    }
}
