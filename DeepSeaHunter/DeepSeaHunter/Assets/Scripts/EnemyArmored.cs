using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyArmored : EnemyAI
{
    public Material flashDamage; //public so armor.cs can access

    [SerializeField] int numBullets;
    [SerializeField] float timeBetweenShots;

    override public void takeDamage(int damage)
    {
        HP -= damage;
        StartCoroutine(flashMat());
        agent.SetDestination(GameManager.instance.player.transform.position);

        if (HP <= 0)
        {
            GameManager.instance.updateGameGoal(-1);
            Destroy(gameObject);
        }
    }

    IEnumerator flashMat()
    {
        Material mat = model.material;
        model.material = flashDamage;
        yield return new WaitForSeconds(.1f);
        model.material = mat;
    }

    override protected void shoot()
    {
        StartCoroutine(shootMultiple());
        shootTimer = 0;
    }

    IEnumerator shootMultiple()
    {
        for (int i = 0; i < numBullets; i++)
        {
            Instantiate(bullet, shootPos.position, transform.rotation);
            yield return new WaitForSeconds(timeBetweenShots);
        }
    }
}
