using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamage
{
    [Header("General")]
    public int HP;
    public Renderer model;
    public Animator anim;
    //hidden
    protected Color modelColor;

    [Header("Navmesh")]
    public NavMeshAgent agent;
    [SerializeField] Transform headPos;
    [SerializeField] int FOV;
    [SerializeField] int roamPauseTime;
    [SerializeField] int roamDist;
    public int faceTargetSpeed;
    public int animTranSpeed;
    //hidden
    float roamTimer;
    protected float stoppingDist;
    Vector3 startingPos;
    protected Vector3 playerDir;
    float angleToPlayer;

    protected bool playerInRange;

    [Header("Shooting")]
    public GameObject bullet;
    public Transform shootPos;
    public float shootRate;
    //hidden
    protected float shootTimer;
    protected bool isSharkAttacking; //this is just for shark, so i dont have to override functions

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        GameManager.instance.updateGameGoal(1);
        modelColor = model.material.color;
        stoppingDist = agent.stoppingDistance;
        startingPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        setAnimLocomotion();
        shootTimer += Time.deltaTime;
        if(agent.remainingDistance < 0.01f)
            roamTimer += Time.deltaTime;
        if (playerInRange && !canSeePlayer())
        {
            checkRoam();
        }
        else if (!playerInRange)
        {
            checkRoam();
        }
    }

    bool canSeePlayer()
    {
        playerDir = GameManager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);
        Debug.DrawRay(headPos.position, playerDir);
        RaycastHit hit;
        if(Physics.Raycast(headPos.position, playerDir, out hit))
        {
            if(hit.collider.CompareTag("Player") && angleToPlayer <= FOV)
            {
                anim.SetBool("isRoaming", false);
                if (!isSharkAttacking)
                { //this prevents shark jittering from changing stopping dist/dest every update
                    agent.stoppingDistance = stoppingDist;
                    agent.SetDestination(GameManager.instance.player.transform.position);
                }

                if (shootTimer >= shootRate)
                    shoot();
                if (agent.remainingDistance <= agent.stoppingDistance)
                    faceTarget();
                return true;
            }
        }
        agent.stoppingDistance = 0;
        return false;
    }

    void checkRoam()
    {
        if ((roamTimer > roamPauseTime && agent.remainingDistance < 0.01f) || GameManager.instance.playerScript.HP <= 0) 
        {
            roam();
        }
    }

    void roam()
    {
        roamTimer = 0;
        agent.stoppingDistance = 0;
        anim.SetBool("isRoaming", true);
        Vector3 ranPos = Random.insideUnitSphere * roamDist;
        ranPos += startingPos;
        NavMeshHit hit;
        NavMesh.SamplePosition(ranPos, out hit, roamDist, 1);
        agent.SetDestination(hit.position);
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
            agent.stoppingDistance = 0;
        }
    }

    public virtual void takeDamage(int damage)
    {
        HP -= damage;
        roamTimer = 0;
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

    protected virtual void shoot()
    {
        shootTimer = 0;
        Instantiate(bullet, shootPos.position, transform.rotation);
    }
    //
}
