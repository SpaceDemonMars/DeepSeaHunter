using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator anim;

    [SerializeField] int HP;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int animTranSpeed;

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

        if (playerInRange)
        {
            playerDir = GameManager.instance.player.transform.position - transform.position;
            agent.SetDestination(GameManager.instance.player.transform.position);

            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                faceTarget();
            }
        }
    }
    void setAnimLocomotion()
    {
        float agentSpeed = (agent.velocity.normalized.magnitude);
        float animSpeedCur = anim.GetFloat("Speed");
        anim.SetFloat("Speed", Mathf.Lerp(animSpeedCur, agentSpeed, Time.deltaTime * animTranSpeed));
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
    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, transform.position.y, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);

    }

    public void takeDamage(int damage)
    {
        HP -= damage;
        StartCoroutine(flashRed());

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
}
