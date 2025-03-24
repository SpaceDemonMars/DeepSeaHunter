using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyBoss : MonoBehaviour, IDamage
{
    [Header("General")]
    [SerializeField] int HP;
    public Renderer model;
    [SerializeField] Animator anim;
    [SerializeField] bool hasUniqueMaterial;
    public Material flashDamage;
    Color modelColor;
    float baseMoveSpeed;
    float baseStoppingDist;
    Transform spawnPos;

    [Header("NavMesh")]
    public NavMeshAgent agent;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int animTranSpeed;
    Vector3 playerDir;

    [Header("General - Attacks")]
    [SerializeField] int globalAttackCooldown;
    [SerializeField] Transform shootPos;
    [SerializeField] bool hasSingleShot, hasMultiShot, hasBeam, hasMelee1, hasMelee2; 
    float globalAttackTimer;
    bool inAttack;
    bool playerInRange;

    [Header("Single Shot")]
    [SerializeField] int singleShotCooldown;
    [SerializeField] GameObject bulletSingle;
    float singleShotTimer;

    [Header("Multi Shot")]
    [SerializeField] int multiShotCooldown;
    [SerializeField] GameObject bulletMulti;
    [SerializeField] int numBullets;
    [SerializeField] float timeBetweenShots;
    float multiShotTimer;

    [Header("Beam")]
    [SerializeField] int beamCooldown;
    [SerializeField] GameObject beam;
    float beamTimer;

    [Header("Melee - 1")]
    [SerializeField] int melee1Cooldown;
    [SerializeField] Collider melee1Col;
    [SerializeField] TrailRenderer melee1TrailMain;
    [SerializeField] TrailRenderer melee1TrailSecond;
    [SerializeField] float melee1MoveSpeed;
    float melee1Timer;

    [Header("Melee - 2")]
    [SerializeField] int melee2Cooldown;
    [SerializeField] Collider melee2Col;
    [SerializeField] float melee2MoveSpeed;
    float melee2Timer;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.instance.updateGameGoal(name, false);
        modelColor = model.material.color;
        baseMoveSpeed = agent.speed;
        baseStoppingDist = agent.stoppingDistance;
        spawnPos = transform;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTimers();
        setAnimLocomotion();
        if (playerInRange)
        {
            playerDir = GameManager.instance.player.transform.position - transform.position;
            agent.SetDestination(GameManager.instance.player.transform.position);
            if (agent.remainingDistance <= agent.stoppingDistance)
                faceTarget();
            CombatController();
        }
    }

    void UpdateTimers()
    {
        globalAttackTimer += Time.deltaTime;
        singleShotTimer += Time.deltaTime;
        multiShotTimer += Time.deltaTime;
        beamTimer += Time.deltaTime;
        melee1Timer += Time.deltaTime;
        melee2Timer += Time.deltaTime;
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
    public void takeDamage(int damage)
    {
        HP -= damage;
        if (hasUniqueMaterial)
            StartCoroutine(flashMat());
        else
            StartCoroutine(flashRed());
        agent.SetDestination(GameManager.instance.player.transform.position);

        if (HP <= 0)
        {
            GameManager.instance.updateGameGoal(name, true);
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
    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(.1f);
        model.material.color = modelColor;
    }

    //COMBAT
    void CombatController()
    {
        if (!inAttack && globalAttackCooldown <= globalAttackTimer)
        { //global CD up && not in attack
            //boss should jump backwards before shooting
            if (hasSingleShot && singleShotCooldown <= singleShotTimer)
                singleShot();
            else if (hasMultiShot && multiShotCooldown <= multiShotTimer)
                multiShot();
            else if (hasBeam && beamCooldown <= beamTimer)
                shootBeam();
            else if (hasMelee1 && melee1Cooldown <= melee1Timer)
                melee1();
            else if (hasMelee2 && melee2Cooldown <= melee2Timer)
                melee2();
        }
    }

    //Single Shot
    void singleShot()
    {
        globalAttackTimer = 0;
        singleShotTimer = 0;
        Instantiate(bulletSingle, shootPos.position, transform.rotation);
    }

    //Multi Shot
    void multiShot()
    {
        StartCoroutine(shootMulti());
    }

    IEnumerator shootMulti()
    {
        inAttack = true;
        for (int i = 0; i < numBullets; i++)
        {
            Instantiate(bulletMulti, shootPos.position, transform.rotation);
            yield return new WaitForSeconds(timeBetweenShots);
        }
        globalAttackTimer = 0;
        singleShotTimer = 0;
        multiShotTimer = 0;
        inAttack = false;
    }

    //Beam
    void shootBeam()
    {
        globalAttackTimer = 0;
        singleShotTimer = 0;
        beamTimer = 0;
        Instantiate(beam, shootPos.position + (Vector3.forward * beam.transform.localScale.y), transform.rotation);
    }

    //Melee 1
    void melee1()
    {
        globalAttackTimer = 0;
        melee1Timer = 0;
        inAttack = true;
        agent.stoppingDistance = 1;
        anim.SetTrigger("Melee 1");
    }

    public void eventMelee1Start()
    {
        melee1Col.enabled = true;
        melee1TrailMain.enabled = true;
        melee1TrailSecond.enabled = true;
    }
    public void eventMelee1End()
    {
        melee1Col.enabled = false;
        melee1TrailMain.enabled = false;
        melee1TrailSecond.enabled = false;
        agent.stoppingDistance = baseStoppingDist;
        globalAttackTimer = 0;
        melee1Timer = 0;
        inAttack = false;

    }

    //Melee 2
    void melee2()
    {

    }
}
