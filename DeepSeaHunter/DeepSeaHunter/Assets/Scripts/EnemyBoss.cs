using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyBoss : MonoBehaviour
{
    [Header("General")]
    [SerializeField] int HP;
    [SerializeField] string Name;
    [SerializeField] Renderer model;
    [SerializeField] Animator anim;
    [SerializeField] bool hasUniqueMaterial;
    public Material flashDamage;
    Color modelColor;
    float baseMoveSpeed;
    float baseStoppingDist;
    Transform spawnPos;

    [Header("NavMesh")]
    [SerializeField] NavMeshAgent agent;
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
    [SerializeField] float beamDuration;
    float beamTimer;

    [Header("Melee - 1")]
    [SerializeField] int melee1ShotCooldown;
    [SerializeField] Collider melee1Col;
    [SerializeField] float melee1MoveSpeed;
    float melee1Timer;

    [Header("Melee - 2")]
    [SerializeField] int melee2ShotCooldown;
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
            CombatController();
    }

    void UpdateTimers()
    {
        globalAttackTimer = Time.deltaTime;
        singleShotTimer = Time.deltaTime;
        multiShotTimer = Time.deltaTime;
        beamTimer = Time.deltaTime;
        melee1Timer = Time.deltaTime;
        melee2Timer = Time.deltaTime;
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
    public virtual void takeDamage(int damage)
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

        }
    }

    //Single Shot


    //Multi Shot


    //Beam


    //Melee 1


    //Melee 2
}
