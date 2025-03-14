using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] int HP;
    public Renderer model;
    [SerializeField] Animator anim;

    [SerializeField] NavMeshAgent agent;

    [SerializeField] GameObject bullet;
    [SerializeField] Transform shootPos;

    [SerializeField] float shootRate;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int animTranSpeed;

    float shootTimer;

    Color modelColor;

    Vector3 playerDir;

    bool playerInRange;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
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
        StartCoroutine(flashRed());
        agent.SetDestination(GameManager.instance.player.transform.position);

        if (HP <= 0)
        {
            GameManager .instance.updateGameGoal(-1);
            Destroy(gameObject);
        }
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(.1f);
        model.material.color = modelColor;
    }

    void shoot()
    {
        shootTimer = 0;
        Instantiate(bullet, shootPos.position, transform.rotation);
    }
    //
}
