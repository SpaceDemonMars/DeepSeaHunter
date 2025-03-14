using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamage
{
    public int HP;
    public Renderer model;
    public Animator anim;

    public NavMeshAgent agent;

    public GameObject bullet;
    public Transform shootPos;

    public float shootRate;
    public int faceTargetSpeed;
    public int animTranSpeed;

    protected float shootTimer;

    protected Color modelColor;

    protected Vector3 playerDir;

    protected bool playerInRange;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        GameManager.instance.updateGameGoal(1);
        modelColor = model.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        setAnimLocomotion();
        shootTimer += Time.deltaTime;
        if (playerInRange)
        {
            playerDir = GameManager.instance.player.transform.position - transform.position;
            agent.SetDestination(GameManager.instance.player.transform.position);

            if (shootTimer >= shootRate)
                shoot();
            if (agent.remainingDistance <= agent.stoppingDistance)
                faceTarget();
        }
    }

    protected void setAnimLocomotion()
    {
        float agentSpeed = agent.velocity.normalized.magnitude;
        float animSpeed = anim.GetFloat("Speed");
        anim.SetFloat("Speed", Mathf.Lerp(animSpeed, agentSpeed, Time.deltaTime * animTranSpeed));
    }

    protected void faceTarget()
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

    public virtual void takeDamage(int damage)
    {
        HP -= damage;
        StartCoroutine(flashRed());
        agent.SetDestination(GameManager.instance.player.transform.position);

        if (HP <= 0)
        {
            GameManager .instance.updateGameGoal(-1);
            Destroy(gameObject);
        }
    }

    protected IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(.1f);
        model.material.color = modelColor;
    }

    protected virtual void shoot()
    {
        shootTimer = 0;
        Instantiate(bullet, shootPos.position, transform.rotation);
    }
    //
}
