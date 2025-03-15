using UnityEngine.AI;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class EnemyBasic : EnemyAI
{
    [SerializeField] float attackSpeed; //speed enemy moves at player while attacking

    bool isAttacking;
    float stoppingDistance;
    float agentSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    override protected void Start()
    {
        base.Start();
        stoppingDistance = agent.stoppingDistance;
        agentSpeed = agent.speed;
    }

    // Update is called once per frame
    void Update()
    {
        setAnimLocomotion();
        shootTimer += Time.deltaTime;
        if (playerInRange)
        {
            if (isAttacking == false)
            {
                playerDir = GameManager.instance.player.transform.position - transform.position;
                agent.SetDestination(GameManager.instance.player.transform.position);

                if (agent.remainingDistance <= agent.stoppingDistance)
                { 
                    //figure out how to make enemy circle target
                    faceTarget(); 
                }

                if (shootTimer >= shootRate)
                {
                    Debug.Log("Start Attack");
                    shoot();
                }
            }
        }
        if (isAttacking && agent.remainingDistance <= agent.stoppingDistance) //enemy isAttacking && and reached destination, outside to prevent player exitting range, locking enemy into attack
        {
            Debug.Log("Call End Attack");
            endAttack();
        }
    }

    protected override void shoot()
    {
        isAttacking = true;
        agent.stoppingDistance = 0;
        agent.speed = attackSpeed;
        bullet.GetComponent<Collider>().enabled = true; //turns on attack collider, so enemy can damage player
        agent.SetDestination(GameManager.instance.player.transform.position + (playerDir/2)); //set enemy to go a little past player
        //enemy should attack player location at time attack was called (this creates player dodge window)
    }
    void endAttack()
    {
        Debug.Log("End Attack");
        shootTimer = 0; //only reset attack timer here, so it cant reset during enemy attack
        isAttacking = false;
        agent.stoppingDistance = stoppingDistance;
        agent.speed = agentSpeed;
        bullet.GetComponent<Collider>().enabled = false; 
        //reset all values back to normal
    }

    override public void takeDamage(int damage)
    {

        shootTimer += shootRate / 2; //reduces attack cooldown when taking damage
        base.takeDamage(damage);
    }
}

