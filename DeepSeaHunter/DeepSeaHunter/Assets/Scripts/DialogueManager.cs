using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    public TextMeshProUGUI nameTxt;
    public TextMeshProUGUI dialogueLine;
    public GameObject dialogueBox;

    private Queue<string> lineQueue;
    private bool isTalking = false;

    public Transform choiceParent;
    public GameObject choiceButton;

    private DialogueEntry[] entries;
    private int currentIndex = 0;


    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else 
        { 
            Destroy(gameObject);
        }
    }
    void Start()
    {
        lineQueue = new Queue<string>();
        dialogueBox.SetActive(false);

    }
    public void StartConvo(Dialogue dialogue)
    {
        isTalking = true;
        dialogueBox.SetActive(true);
        nameTxt.text = dialogue.npcName;

        entries = dialogue.lines;
        currentIndex = 0;

        DisplayEntry(entries[currentIndex]);
        lineQueue.Clear();

        foreach (DialogueEntry entry in dialogue.lines)
        {
            lineQueue.Enqueue(entry.line);
        }

        DisplayNextEntry();
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
        StartCoroutine(TypeSentence(entry.line));

        foreach (Transform child in choiceParent)
        {
            Destroy(child.gameObject);
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
    }

    public bool IsTalking() => isTalking;

}