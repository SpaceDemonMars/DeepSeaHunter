using UnityEngine;

[System.Serializable]
public class DialogueEntry
{
    [TextArea(2, 5)] public string line;
    public DialogueChoice[] choices;

    [System.Serializable]
    public class DialogueChoice
    {
        public string choiceText;
        public int nextLineIndex;
    }
}
