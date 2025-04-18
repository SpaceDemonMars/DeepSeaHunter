using System.Collections;
using System.Collections.Generic;
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

    [SerializeField]  private DialogueEntry[] entries;
    private int currentIndex = 0;

    void Awake()
    {
    }
    void Start()
    {
        if (dialogueBox == null)
            dialogueBox = GameObject.Find("DialogueBox"); // Find by name

        if (nameTxt == null)
            nameTxt = GameObject.Find("NameText")?.GetComponent<TextMeshProUGUI>();

        if (dialogueLine == null)
            dialogueLine = GameObject.Find("DialogueLine")?.GetComponent<TextMeshProUGUI>();

        if (choiceParent == null)
            choiceParent = GameObject.Find("ChoiceParent")?.transform;

        dialogueBox.SetActive(false);
    }
    public void StartConvo(Dialogue dialogue)
    {
        isTalking = true;
        dialogueBox.SetActive(true);

        entries = dialogue.lines;
        currentIndex = 0;

        DisplayEntry(entries[currentIndex]);
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

        if (!string.IsNullOrEmpty(entry.speakerName))
        {
            nameTxt.text = entry.speakerName;
        }
        else
        {
            nameTxt.text = "???"; 
        }

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
        if (dialogueBox != null)
        {
            dialogueBox.SetActive(false);
        }
        isTalking = false;
        entries = null;
        currentIndex = 0;

        if (nameTxt != null) nameTxt.text = "";
        if (dialogueLine != null) dialogueLine.text = "";
        if (choiceParent != null)
        {
            foreach (Transform child in choiceParent)
            {
                Destroy(child.gameObject);
            }
        }
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