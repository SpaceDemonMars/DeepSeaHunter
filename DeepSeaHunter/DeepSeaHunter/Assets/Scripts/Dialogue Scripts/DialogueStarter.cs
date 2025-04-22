using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class DialogueStarter : MonoBehaviour
{
    public Dialogue defaultDialogue;
    public List<DialogueCondition> dialogueConditions = new List<DialogueCondition>();
    TMP_Text talkPrompt;
    DialogueManager dialogueManager;
    public Clue clueinDialogue;

    private bool isPlayerInRange = false;
    private bool hasTriggered = false;
    private bool waitingForInput = false;

    private void Start()
    {
        talkPrompt = GameManager.instance.interactPrompt;
        dialogueManager = GameManager.instance.dialogueManager;
    }

    void Update()
    {
        if (isPlayerInRange && !hasTriggered)
        {
            if (!talkPrompt.gameObject.activeSelf && waitingForInput)
            {
                talkPrompt.gameObject.SetActive(true);
            }

            if (Input.GetButtonDown("Interact"))
            {
                hasTriggered = true;
                talkPrompt.gameObject.SetActive(false);
                dialogueManager.currentDialogueStarter = this;

                Dialogue selectedDialogue = defaultDialogue;

                for (int i = dialogueConditions.Count - 1; i >= 0; i--)
                {
                    if (QuestManager.instance.IsQuestCompleted(dialogueConditions[i].requiredQuest))
                    {
                        selectedDialogue = dialogueConditions[i].dialogueToUse;
                        break;
                    }
                }

                dialogueManager.StartConvo(selectedDialogue);

                FriendlyNPC npc = GetComponent<FriendlyNPC>();
                if (npc != null)
                {
                    GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                    if (playerObj != null)
                    {
                        npc.StartDialogue(playerObj.transform);
                    }
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            waitingForInput = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            talkPrompt.gameObject.SetActive(false);
            waitingForInput = false;
        }
    }
}
