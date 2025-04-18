using UnityEngine;

[System.Serializable]
public class DialogueEntry
{
    public string speakerName;
    public string line;
    public DialogueChoice[] choices;

    [System.Serializable]
    public class DialogueChoice
    {
        public string choiceText;
        public int nextLineIndex;
    }
}
