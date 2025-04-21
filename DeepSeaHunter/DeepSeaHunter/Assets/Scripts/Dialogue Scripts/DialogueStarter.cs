using UnityEngine;
using TMPro;

public class DialogueStarter : MonoBehaviour
{
    public Dialogue dialogue;              
    TMP_Text talkPrompt;   //modified for a bug fix - paige   
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
                Debug.Log("Entered NPC Trigger");
                talkPrompt.gameObject.SetActive(true);
            }

            if (Input.GetButtonDown("Interact"))
            {
                hasTriggered = true;
                talkPrompt.gameObject.SetActive(false);
                dialogueManager.currentDialogueStarter = this;

                dialogueManager.StartConvo(dialogue);

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
