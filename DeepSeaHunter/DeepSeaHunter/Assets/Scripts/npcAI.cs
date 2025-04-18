using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class FriendlyNPC : MonoBehaviour
{
    [Header("<----- General ----->")]
    public Renderer model;
    public Animator anim;
    private Color modelColor;

    [Header("<----- NavMesh ----->")]
    public NavMeshAgent agent;
    private Vector3 startingPos;

    [Header("<----- Roaming ----->")]
    public int roamPauseTime = 3;
    public float roamDist = 2f;

    private bool isTalking = false;
    private Transform talkingTarget;

    private float roamTimer;
    private bool isPlayerInteracting;

    private Transform player;

    void Start()
    {
        modelColor = model.material.color;
        startingPos = transform.position;
    }

    void Update()
    {
        if (isTalking && talkingTarget != null)
        {
            FacePlayer();
        }
        else
        {
            HandleRoaming();
        }
    }

    void HandleRoaming()
    {
        if (!isPlayerInteracting)
        {
            if (agent.remainingDistance <= 0.1f)
                roamTimer += Time.deltaTime;

            if (roamTimer >= roamPauseTime)
            {
                Roam();
            }

            float agentSpeed = agent.velocity.normalized.magnitude;
            float animSpeed = anim.GetFloat("Speed");
            anim.SetFloat("Speed", Mathf.Lerp(animSpeed, agentSpeed, Time.deltaTime * 5f));
        }
    }

    void Roam()
    {
        roamTimer = 0;
        Vector3 randomDirection = Random.insideUnitSphere * roamDist;
        randomDirection += startingPos;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, roamDist, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    void FacePlayer()
    {
        if (player == null)
            return;

        Vector3 lookDirection = player.position - transform.position;
        lookDirection.y = 0;
        if (lookDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    public void Interact(Transform playerTransform)
    {
        isPlayerInteracting = true;
        player = playerTransform;
        agent.ResetPath();
        anim.SetFloat("Speed", 0f);
    }

    public void StartDialogue(Transform playerTransform)
    {
        isTalking = true;
        talkingTarget = playerTransform;
        agent.ResetPath(); 
        anim.SetFloat("Speed", 0f); 
    }

    public void EndDialogue()
    {
        isTalking = false;
        talkingTarget = null;
        roamTimer = 0f; 
    }

    public void StopInteraction()
    {
        isPlayerInteracting = false;
        player = null;
        roamTimer = 0;
    }
}