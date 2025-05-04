using UnityEngine;
using UnityEngine.AI;

public class npcAI : MonoBehaviour
{
    public Renderer model;
    public Animator anim;
    private Color modelColor;

    public NavMeshAgent agent;
    private Vector3 startingPos;

    public int roamPauseTime = 3;
    public float roamDist = 2f;

    private bool isTalking = false;
    private bool isPlayerInteracting;
    private Transform player;
    private Transform talkingTarget;
    private float roamTimer;

    [Header("Shop Settings")]
    public bool shopUnlocked = false;
    public Shop shop;
    public bool autoOpenShopAfterTurnIn = true;

    [Header("Quest Unlock Settings")]
    public QuestID requiredQuestToUnlock;
    public string requiredItemName;
    public int requiredItemAmount;

    private void Start()
    {
        modelColor = model.material.color;
        startingPos = transform.position;
        roamTimer = roamPauseTime;
    }

    private void Update()
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
        if (!isPlayerInteracting && !isTalking)
        {
            if (agent.remainingDistance <= 0.1f)
                roamTimer += Time.deltaTime;

            if (roamTimer >= roamPauseTime)
            {
                Roam();
            }

            float agentSpeed = agent.velocity.normalized.magnitude;
            anim.SetFloat("Speed", Mathf.Lerp(anim.GetFloat("Speed"), agentSpeed, Time.deltaTime * 5f));
        }
    }

    void Roam()
    {
        roamTimer = 0;
        Vector3 randomDirection = Random.insideUnitSphere * roamDist;
        randomDirection += startingPos;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, roamDist, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    void FacePlayer()
    {
        if (player == null) return;

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
        player = playerTransform;
        agent.ResetPath();
        agent.isStopped = true;
        anim.SetFloat("Speed", 0f);
    }

    public void EndDialogue()
    {
        isTalking = false;
        talkingTarget = null;
        player = null;
        agent.isStopped = false;
        roamTimer = 0f;

        QuestItemTurnIn turnIn = GetComponent<QuestItemTurnIn>();
        if (turnIn != null)
        {
            turnIn.TryTurnIn();
        }

        HandleQuestCompletion();

        if (shopUnlocked && autoOpenShopAfterTurnIn && shop != null)
        {
            shop.OpenShop();
        }
    }

    public void StopInteraction()
    {
        isPlayerInteracting = false;
        player = null;
        roamTimer = 0;
    }

    void HandleQuestCompletion()
    {
        if (shopUnlocked) return; 

        if (requiredQuestToUnlock != QuestID.None && QuestManager.instance.IsQuestCompleted(requiredQuestToUnlock))
        {
            if (!string.IsNullOrEmpty(requiredItemName) && requiredItemAmount > 0)
            {
                if (playerInven.Instance.HasItem(requiredItemName, requiredItemAmount))
                {
                    playerInven.Instance.RemoveItem(requiredItemName, requiredItemAmount);
                    shopUnlocked = true;
        //            Debug.Log("Shop unlocked by item turn-in!");
                }
            }
            else
            {
                shopUnlocked = true;
   //             Debug.Log("Shop unlocked by quest completion!");
            }
        }
    }
}
