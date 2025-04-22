using UnityEngine;

[System.Serializable]
public class DialogueEntry
{
    public string speakerName;
    public string line;
    public DialogueChoice[] choices;

    public bool completeQuest;
    public QuestID questToComplete;

    public bool startQuest;
    public QuestID questToStart;

    [System.Serializable]
    public class DialogueChoice
    {
        public string choiceText;
        public int nextLineIndex;
    }
}
