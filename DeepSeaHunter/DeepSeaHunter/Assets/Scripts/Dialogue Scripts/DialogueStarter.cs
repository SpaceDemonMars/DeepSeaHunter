using UnityEngine;
using TMPro;
using System.Collections.Generic;

[System.Serializable]
public class DialogueCondition
{
    public QuestID requiredQuest;
    public Dialogue dialogueToUse;
}

public class DialogueStarter : MonoBehaviour
{
    public Dialogue defaultDialogue;
    public List<DialogueCondition> dialogueConditions = new List<DialogueCondition>();

    private TMP_Text talkPrompt;
    private DialogueManager dialogueManager;
    public Clue clueinDialogue;

    private bool isPlayerInRange = false;
    private bool waitingForInput = false;

    private float dialogueCooldown = 0.7f;
    private float lastDialogueTime = -10f;


    private void Start()
    {
        dialogueManager = GameManager.instance.dialogueManager;

        if (GameManager.instance.interactPrompt != null)
        {
            talkPrompt = GameManager.instance.interactPrompt;
        }
    }

    private void Update()
    {
        if (isPlayerInRange && !DialogueManager.instance.IsTalking() && Time.time - lastDialogueTime > dialogueCooldown)
        {
            if (talkPrompt != null && !talkPrompt.gameObject.activeSelf && waitingForInput)
            {
                talkPrompt.gameObject.SetActive(true);
            }

            if (Input.GetButtonDown("Interact"))
            {
                lastDialogueTime = Time.time;

                if (talkPrompt != null)
                    talkPrompt.gameObject.SetActive(false);

                dialogueManager.currentDialogueStarter = this;

                Dialogue dialogueToStart = GetAppropriateDialogue();
                dialogueManager.StartConvo(dialogueToStart);

                npcAI npc = GetComponent<npcAI>();
                if (npc != null)
                {
                    GameObject playerObj = GameObject.FindWithTag("Player");
                    if (playerObj != null)
                    {
                        npc.StartDialogue(playerObj.transform);
                    }
                }
            }
        }
    }

    public void SetDialogueCooldownTime()
    {
        lastDialogueTime = Time.time;
    }

    private Dialogue GetAppropriateDialogue()
    {
        foreach (var condition in dialogueConditions)
        {
            if (QuestManager.instance.IsQuestCompleted(condition.requiredQuest))
            {
                return condition.dialogueToUse;
            }
        }
        return defaultDialogue;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            waitingForInput = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            waitingForInput = false;
            if (talkPrompt != null)
                talkPrompt.gameObject.SetActive(false);
        }
    }
}
