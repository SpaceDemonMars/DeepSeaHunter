using UnityEngine.AI;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using System;

public class EnemyBasic : EnemyAI
{
    [SerializeField] Collider biteCollider;
    [SerializeField] CapsuleCollider takeDmgCollider;
    [SerializeField] float biteSpeed;
    [SerializeField] TrailRenderer body1Trail;
    [SerializeField] TrailRenderer body2Trail;
    [SerializeField] TrailRenderer tailTrail;

    float agentSpeed;

    override protected void Start()
    {
        agentSpeed = agent.speed;
        base.Start();
    }

    //anim events
    public void onBiteStart()
    {
        Debug.Log("Shark Bite");
        isSharkAttacking = true;
        body1Trail.enabled = isSharkAttacking;
        body2Trail.enabled = isSharkAttacking;
        tailTrail.enabled = isSharkAttacking;
        biteCollider.enabled = isSharkAttacking;
        takeDmgCollider.radius /= 2; //shrink hitbox
        agent.speed *= biteSpeed;
        agent.stoppingDistance = 0; 
        Vector3 attackDest = GameManager.instance.player.transform.position + (playerDir / 2);
        NavMeshHit hit;
        NavMesh.SamplePosition(attackDest, out hit, (stoppingDist / 2), 1);
        agent.SetDestination(hit.position);
    }

    public void onBiteEnd()
    {
        Debug.Log("Shark End");
        isSharkAttacking = false;
        body1Trail.enabled = isSharkAttacking;
        body2Trail.enabled = isSharkAttacking;
        tailTrail.enabled = isSharkAttacking;
        biteCollider.enabled = isSharkAttacking;
        takeDmgCollider.radius *= 2;
        agent.speed /= biteSpeed;
        agent.stoppingDistance = stoppingDist;
        agent.SetDestination(GameManager.instance.player.transform.position);
        shootTimer = 0;
    }

    //canSeePlayer calls shoot >> use shoot for attack + shootTimer
    protected override void shoot()
    {
        shootTimer = 0; //reset
        anim.SetTrigger("Bite");
    }

    override public void takeDamage(int damage)
    {
        shootTimer += shootRate / 2; //reduces attack cooldown when taking damage
        base.takeDamage(damage);
    }
}

/*
 * 
 *      Vector3 attackDest = GameManager.instance.player.transform.position + (playerDir/2);
        NavMeshHit hit;
        NavMesh.SamplePosition(ranPos, out hit, (stoppingDist/2), 1);
        agent.SetDestination(hit.position);
 * 
 * 
 */