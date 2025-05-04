using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI nameTxt;
    public TextMeshProUGUI dialogueLine;
    public GameObject dialogueBox;
    public Transform choiceParent;
    public GameObject choiceButton;

    [SerializeField] private DialogueEntry[] entries;
    private DialogueEntry lastSpokenEntry;
    private int currentIndex = 0;

    private bool isTalking = false;
    private bool isReadyToClose = false;

    public DialogueStarter currentDialogueStarter;
    public static DialogueManager instance;
    public bool IsTalking() => isTalking;

    private void Awake() => instance = this;

    void Update()
    {
        if (!isTalking) return;

        if (Input.GetButtonDown("Interact") && choiceParent.childCount == 0)
        {
            if (isReadyToClose)
                EndDialogue();
            else
                DisplayNextEntry();
        }
    }

    public void StartConvo(Dialogue dialogue)
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        isTalking = true;
        isReadyToClose = false;

        dialogueBox.SetActive(true);
        entries = dialogue.lines;
        currentIndex = 0;

        CleanupChoices();

        var npcTurnIn = currentDialogueStarter?.GetComponent<QuestItemTurnIn>();
        npcTurnIn?.TryTurnIn();

        StartCoroutine(DelayedFirstEntry());

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
            isReadyToClose = true;
            return;
        }

        DisplayEntry(entries[currentIndex]);
    }

    void DisplayEntry(DialogueEntry entry)
    {
        lastSpokenEntry = entry;
        StopAllCoroutines();
        dialogueLine.text = "";
        nameTxt.text = string.IsNullOrEmpty(entry.speakerName) ? "???" : entry.speakerName;
        StartCoroutine(TypeSentence(entry.line));

        CleanupChoices();

        // Item turn-in
        if (entry.attemptTurnIn)
        {
            var npc = currentDialogueStarter?.GetComponent<QuestItemTurnIn>();
            npc?.TryTurnIn();
        }

        // Complete quest
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

        // Start quest
        if (entry.startQuest)
        {
            Quest questToStart = QuestManager.instance.allQuests.Find(q => q.questID == entry.questToStart);
            if (questToStart != null)
            {
                QuestManager.instance.StartQuest(questToStart);
            }
        }

        // Dialogue choices
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
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        isTalking = false;
        isReadyToClose = false;
        dialogueBox.SetActive(false);

        CleanupChoices();

        if (currentDialogueStarter != null)
        {
            if (currentDialogueStarter.clueinDialogue != null)
                JournalManager.instance.DiscoverClue(currentDialogueStarter.clueinDialogue);

            npcAI npc = currentDialogueStarter.GetComponent<npcAI>();
            npc?.EndDialogue();

            currentDialogueStarter.SetDialogueCooldownTime();
            currentDialogueStarter = null;
        }

        // Handle scene change
        if (lastSpokenEntry != null && lastSpokenEntry.changeSceneAfter && !string.IsNullOrEmpty(lastSpokenEntry.sceneToLoad))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(lastSpokenEntry.sceneToLoad);
            return;
        }


        nameTxt.text = "";
        dialogueLine.text = "";
        entries = null;
        currentIndex = 0;

        GameManager.instance.playerScript.enabled = true;
    }

    void CleanupChoices()
    {
        foreach (Transform child in choiceParent)
            Destroy(child.gameObject);
    }

    IEnumerator DelayedFirstEntry()
    {
        yield return null;
        DisplayEntry(entries[currentIndex]);
    }
}
