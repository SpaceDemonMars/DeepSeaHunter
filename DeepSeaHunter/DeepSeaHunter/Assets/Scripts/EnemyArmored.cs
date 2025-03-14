using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class EnemyArmored : MonoBehaviour, IDamage
{
    [SerializeField] EnemyAI parent;
    [SerializeField] Material flashDamage;
    [SerializeField] Material damaged;
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

        if (HP <= 0)
        {
            parent.model.material = damaged;
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
