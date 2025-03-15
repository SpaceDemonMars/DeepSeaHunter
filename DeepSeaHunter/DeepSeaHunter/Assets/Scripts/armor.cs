using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class armor : MonoBehaviour, IDamage
{
    [SerializeField] EnemyArmored parent;
    [SerializeField] Material flashDamage;
    [SerializeField] Material damaged;
    [SerializeField] Material parentDamaged;
    [SerializeField] int HP;

    Material parentMaterial;

    void Start()
    {
        parentMaterial = parent.model.material;
    }

    public void takeDamage(int damage)
    {
        HP -= damage;
        StartCoroutine(flashWhite());
        parent.agent.SetDestination(GameManager.instance.player.transform.position);

        if (HP <= 0)
        {
            parent.model.material = damaged;
            parent.flashDamage = parentDamaged;
            Destroy(gameObject);
        }
    }

    IEnumerator flashWhite()
    {
        parent.model.material = flashDamage;
        yield return new WaitForSeconds(.1f);
        parent.model.material = parentMaterial;
    }
}
