using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyBoss : MonoBehaviour, IDamage
{
    [Header("General")]
    [SerializeField] string bossName;
    [SerializeField] int HP;
    public Renderer model;
    [SerializeField] Animator anim;
    [SerializeField] bool hasUniqueMaterial, hasArmor;
    public Material flashDamage;
    int HPOrig;
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
    bool inAttack, attackLocksMotion;
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
    [SerializeField] TrailRenderer melee1Trail1;
    [SerializeField] TrailRenderer melee1Trail2;
    [SerializeField] TrailRenderer melee1Trail3;
    float melee1Timer;
    bool useAltDestination;

    [Header("Melee - 2")]
    [SerializeField] int melee2Cooldown;
    [SerializeField] Collider melee2Col;
    [SerializeField] int numAttacks;
    [SerializeField] float timeBetweenAttacks;
    [SerializeField] TrailRenderer melee2Trail1;
    [SerializeField] TrailRenderer melee2Trail2;
    [SerializeField] TrailRenderer melee2Trail3;
    float melee2Timer;
    bool inMelee2;
    int attackCounter;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.instance.updateGameGoal(bossName, false);
        HPOrig = HP;
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
        playerDir = GameManager.instance.player.transform.position - transform.position;
        if (playerInRange && !attackLocksMotion)
        {
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
            GameManager.instance.bossBarText.text = bossName;
            if (!hasArmor)
                GameManager.instance.bossArmorBar.fillAmount = 0; //no armor bar
            GameManager.instance.bossHP.SetActive(true);
            updateBossUI();
        }
    }

    public void updateBossUI()
    {
        GameManager.instance.bossHPBar.fillAmount = (float)HP / HPOrig;
    }
    public void takeDamage(int damage)
    {
        HP -= damage;
        if (hasUniqueMaterial)
            StartCoroutine(flashMat());
        else
            StartCoroutine(flashRed());
        agent.SetDestination(GameManager.instance.player.transform.position);

        updateBossUI();

        if (HP <= 0)
        {
            DropOnDeath dropScript = GetComponent<DropOnDeath>();
            if (dropScript != null)
            {
                dropScript.Drop();
            }

            GameManager.instance.updateGameGoal(bossName, true);
            GameManager.instance.bossHP.SetActive(false);
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
        agent.stoppingDistance = 0;
        if (useAltDestination)
        {
            Vector3 attackDest = GameManager.instance.player.transform.position + playerDir;
            NavMeshHit hit;
            NavMesh.SamplePosition(attackDest, out hit, 2, 1);
            agent.SetDestination(hit.position);
        }
        attackLocksMotion = inAttack;
        anim.SetTrigger("Melee 1");
    }

    public void eventMelee1Start()
    {
        melee1Col.enabled = inAttack;
        if (melee1Trail1 != null)
            melee1Trail1.enabled = inAttack;
        if (melee1Trail2 != null)
            melee1Trail2.enabled = inAttack;
        if (melee1Trail3 != null)
            melee1Trail3.enabled = inAttack;
    }
    public void eventMelee1End()
    {
        inAttack = false;
        attackLocksMotion = inAttack;
        eventMelee1Start(); //this just toggles stuff lol
        agent.stoppingDistance = baseStoppingDist;
        globalAttackTimer = 0;
        melee1Timer = 0;
    }

    //Melee 2
    void melee2()
    {
        globalAttackTimer = 0;
        melee1Timer = 0;
        melee2Timer = 0;
        inAttack = true;
        agent.stoppingDistance = 0;
        attackLocksMotion = inAttack;
        anim.SetTrigger("Melee 2");

    }

    public void eventMelee2()
    {
        inMelee2 = !inMelee2;
        melee2Toggles();
        if (inMelee2) //in attack anim
        {
            if (useAltDestination)
            {
                Vector3 attackDest = GameManager.instance.player.transform.position + playerDir;
                NavMeshHit hit;
                NavMesh.SamplePosition(attackDest, out hit, 2, 1);
                agent.SetDestination(hit.position);
            }
            else
                agent.SetDestination(GameManager.instance.player.transform.position);

            attackCounter++;
        }
        else if (!inMelee2 && attackCounter < numAttacks) //out of anim, but has more attacks
        {
            StartCoroutine(holdBetweenAttacks());
        }
        else //out of anim, out of attacks
        {
            globalAttackTimer = 0;
            melee1Timer = 0;
            melee2Timer = 0;
            attackCounter = 0;
            inAttack = false;
            agent.stoppingDistance = baseStoppingDist;
            attackLocksMotion = inAttack;
        }
    }

    IEnumerator holdBetweenAttacks()
    {
        float timer = 0;
        while (timer < timeBetweenAttacks)
        {
            faceTarget();
            yield return new WaitForSeconds(.1f);
            timer += Time.deltaTime;
        }
        anim.SetTrigger("Melee 2");
    }

    void melee2Toggles()
    {
        melee2Col.isTrigger = inMelee2;
        if (melee2Trail1 != null)
            melee2Trail1.enabled = inMelee2;
        if (melee2Trail2 != null)
            melee2Trail2.enabled = inMelee2;
        if (melee2Trail3 != null)
            melee2Trail3.enabled = inMelee2;
    }
}
