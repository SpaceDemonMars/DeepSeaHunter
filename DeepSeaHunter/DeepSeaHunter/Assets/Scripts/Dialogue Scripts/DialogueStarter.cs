using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueStarter : MonoBehaviour
{
    public Dialogue dialogue;
    public TextMeshProUGUI talkPrompt;
    public DialogueManager dialogueManager;
    public string playerThoughtonIntro;

    private bool isPlayerInRange = false;
    private bool hasTriggered = false;
    private bool waitingForInput = false;

    void Update()
    {
        if (isPlayerInRange && !hasTriggered)
        {
            if (!waitingForInput)
            {
                talkPrompt.gameObject.SetActive(true); // Show prompt immediately
                waitingForInput = true;
            }

            if (Input.GetButtonDown("Interact"))
            {
                hasTriggered = true;
                talkPrompt.gameObject.SetActive(false);

                if (!string.IsNullOrEmpty(playerThoughtonIntro))
                {
                    StartCoroutine(ThoughtThenDialogue());
                }
                else
                {
                    dialogueManager.StartConvo(dialogue);
                }
            }
        }
    }

    IEnumerator ThoughtThenDialogue()
    {
        yield return null; // Allow frame to complete

        PlayerThoughts.Instance.ShowThought(playerThoughtonIntro);

        while (PlayerThoughts.Instance.IsShowingThought())
        {
            yield return null;
        }

        dialogueManager.StartConvo(dialogue);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            waitingForInput = false;
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
