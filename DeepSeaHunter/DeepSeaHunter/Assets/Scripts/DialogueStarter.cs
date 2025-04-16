using TMPro;
using UnityEngine;

public class DialogueStarter : MonoBehaviour
{
    public Dialogue dialogue;
    public TextMeshProUGUI talkPrompt;
    public DialogueManager dialogueManager;

    private bool isPlayerInRange = false;

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            dialogueManager.StartConvo(dialogue);
            talkPrompt.gameObject.SetActive(false);
        }
    }
        void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            talkPrompt.gameObject.SetActive(true);
        }
    }
        void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            talkPrompt.gameObject.SetActive(false);
        }
    }
}
