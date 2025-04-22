using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI nameTxt;
    public TextMeshProUGUI dialogueLine;
    public GameObject dialogueBox;
    private bool isTalking = false;
    public Transform choiceParent;
    public GameObject choiceButton;

    [SerializeField] private DialogueEntry[] entries;
    private int currentIndex = 0;

    public DialogueStarter currentDialogueStarter;
    public static DialogueManager instance;

    public void StartConvo(Dialogue dialogue)
    {
        isTalking = true;
        dialogueBox.SetActive(true);
        entries = dialogue.lines;
        currentIndex = 0;
        DisplayEntry(entries[currentIndex]);
        GameManager.instance.playerScript.enabled = false;

        if (currentDialogueStarter != null)
        {
            npcAI npc = currentDialogueStarter.GetComponent<npcAI>();
            if (npc != null)
                npc.StartDialogue(GameManager.instance.player.transform);
        }
    }

    public void DisplayNextEntry()
    {
        currentIndex++;
        if (currentIndex >= entries.Length)
        {
            EndDialogue();
            return;
        }

        DisplayEntry(entries[currentIndex]);
    }

    void DisplayEntry(DialogueEntry entry)
    {
        StopAllCoroutines();
        dialogueLine.text = "";

        if (entry.requiresQuest)
        {
            bool questCompleted = QuestManager.instance.IsQuestCompleted(entry.requiredQuestID);
            bool questActive = QuestManager.instance.activeQuests.Exists(q => q.questID == entry.requiredQuestID);

            if (entry.questMustBeCompleted && !questCompleted)
            {
                DisplayNextEntry();
                return;
            }
            if (!entry.questMustBeCompleted && !questActive)
            {
                DisplayNextEntry();
                return;
            }
        }

        if (entry.requiresItem)
        {
            if (!playerInven.Instance.HasItem(entry.requiredItemName, entry.requiredItemAmount))
            {
                DisplayNextEntry();
                return;
            }
        }

        StartCoroutine(TypeSentence(entry.line));

        nameTxt.text = !string.IsNullOrEmpty(entry.speakerName) ? entry.speakerName : "???";

        foreach (Transform child in choiceParent)
        {
            Destroy(child.gameObject);
        }

        if (entry.completeQuest)
        {
            Quest questToComplete = QuestManager.instance.activeQuests.Find(q => q.questID == entry.questToComplete);
            if (questToComplete != null)
            {
                for (int i = 0; i < questToComplete.objectives.Count; i++)
                {
                    if (questToComplete.objectives[i].status == ObjectiveStatus.Active)
                    {
                        QuestManager.instance.CompleteObjective(entry.questToComplete, i);
                        break;
                    }
                }
            }
        }

        if (entry.startQuest)
        {
            Quest questToStart = QuestManager.instance.allQuests.Find(q => q.questID == entry.questToStart);
            if (questToStart != null)
            {
                QuestManager.instance.StartQuest(questToStart);
            }
        }

        if (entry.choices != null && entry.choices.Length > 0)
        {
            foreach (DialogueEntry.DialogueChoice choice in entry.choices)
            {
                GameObject buttonObj = Instantiate(choiceButton, choiceParent);
                TMP_Text buttonText = buttonObj.GetComponentInChildren<TMP_Text>();
                buttonText.text = choice.choiceText;

                buttonObj.GetComponent<Button>().onClick.AddListener(() =>
                {
                    currentIndex = choice.nextLineIndex;
                    DisplayEntry(entries[currentIndex]);
                });
            }
        }
    }

    IEnumerator TypeSentence(string lines)
    {
        dialogueLine.text = "";
        foreach (char letter in lines.ToCharArray())
        {
            dialogueLine.text += letter;
            yield return new WaitForSeconds(0.02f);
        }
    }

    void EndDialogue()
    {
        dialogueBox.SetActive(false);
        isTalking = false;
        entries = null;
        currentIndex = 0;

        nameTxt.text = "";
        dialogueLine.text = "";

        foreach (Transform child in choiceParent)
        {
            Destroy(child.gameObject);
        }

        if (currentDialogueStarter != null)
        {
            if (currentDialogueStarter.clueinDialogue != null)
            {
                JournalManager.instance.DiscoverClue(currentDialogueStarter.clueinDialogue);
            }

            npcAI npc = currentDialogueStarter.GetComponent<npcAI>();
            if (npc != null)
            {
                npc.EndDialogue();
            }

            currentDialogueStarter = null;
        }

        GameManager.instance.playerScript.enabled = true;
    }

    public bool IsTalking() => isTalking;

    void Update()
    {
        if (isTalking && Input.GetButtonDown("Interact") && choiceParent.childCount == 0)
        {
            DisplayNextEntry();
        }
    }
}
