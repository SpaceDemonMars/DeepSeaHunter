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

    public bool requiresItem;         
    public string requiredItemName;   
    public int requiredItemAmount;    

    public bool requiresQuest;        
    public QuestID requiredQuestID;   
    public bool questMustBeCompleted; 


    [System.Serializable]
    public class DialogueChoice
    {
        public string choiceText;
        public int nextLineIndex;
    }
}
