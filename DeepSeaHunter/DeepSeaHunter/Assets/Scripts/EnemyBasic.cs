using UnityEngine.AI;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class EnemyBasic : MonoBehaviour, IDamage
{
    [SerializeField] int HP;
    [SerializeField] Renderer model;
    [SerializeField] Animator anim;

    [SerializeField] NavMeshAgent agent;

    [SerializeField] int faceTargetSpeed;
    [SerializeField] int animTranSpeed;

    [SerializeField] int attackRate;
    [SerializeField] float attackSpeed; //speed enemy moves at player while attacking

    [SerializeField] GameObject attackCollider;

    float attackTimer;
    bool isAttacking;
    float stoppingDistance;
    float agentSpeed;

    Color modelColor;

    Vector3 playerDir;

    bool playerInRange;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.instance.updateGameGoal(1);
        modelColor = model.material.color;
        stoppingDistance = agent.stoppingDistance;
        agentSpeed = agent.speed;
    }

    // Update is called once per frame
    void Update()
    {
        //setAnimLocomotion();
        attackTimer += Time.deltaTime;
        if (playerInRange)
        {
            if (isAttacking == false)
            {
                playerDir = GameManager.instance.player.transform.position - transform.position;
                agent.SetDestination(GameManager.instance.player.transform.position);

                if (attackTimer >= attackRate)
                {
                    Attack();
                }

                if (agent.remainingDistance <= agent.stoppingDistance)
                    faceTarget();
            }
        }
        if (isAttacking && agent.remainingDistance <= 0.5) //enemy isAttacking && and reached destination, outside to prevent player exitting range, locking enemy into attack
        {
            Debug.Log("agent.isStopped");
            endAttack();
        }
    }

    void Attack()
    {
        isAttacking = true;
        agent.stoppingDistance = 0;
        agent.speed = attackSpeed;
        attackCollider.GetComponent<Collider>().enabled = true; //turns on attack collider, so enemy can damage player
        agent.SetDestination(GameManager.instance.player.transform.position + (playerDir/2)); //set enemy to go a little past player
        //enemy should attack player location at time attack was called (this creates player dodge window)
    }
    void endAttack()
    {
        Debug.Log("End Attack Called");
        attackTimer = 0; //only reset attack timer here, so it cant reset during enemy attack
        isAttacking = false;
        agent.stoppingDistance = stoppingDistance;
        agent.speed = agentSpeed;
        attackCollider.GetComponent<Collider>().enabled = false; 
        //reset all values back to normal
    }

    void setAnimLocomotion()
    {
        float agentSpeed = agent.velocity.normalized.magnitude;
        float animSpeed = anim.GetFloat("Speed");
        anim.SetFloat("Speed", Mathf.Lerp(animSpeed, agentSpeed, Time.deltaTime * animTranSpeed));
    }

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, transform.position.y, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    public void takeDamage(int damage)
    {
        HP -= damage;
        attackTimer += attackRate / 2; //reduces attack cooldown when taking damage
        StartCoroutine(flashRed());
        agent.SetDestination(GameManager.instance.player.transform.position);

        if (HP <= 0)
        {
            GameManager.instance.updateGameGoal(-1);
            Destroy(gameObject);
        }
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(.1f);
        model.material.color = modelColor;
    }

    //
}

