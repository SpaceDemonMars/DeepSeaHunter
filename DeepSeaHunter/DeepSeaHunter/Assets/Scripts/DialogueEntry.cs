using UnityEngine;

public class DialogueEntry : MonoBehaviour
{
    [SerializeField] public string line;
    [SerializeField] public DialogueChoice[] choices;

    public class DialogueChoice
    {
        [SerializeField] public string choiceText;
        public int nextLineIndex;
    }
}