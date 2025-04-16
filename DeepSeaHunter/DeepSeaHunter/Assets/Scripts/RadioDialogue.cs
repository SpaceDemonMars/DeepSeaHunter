using UnityEngine;

public class RadioDialogue : MonoBehaviour
{
    public Dialogue radioDialogue;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!DialogueManager.Instance.IsTalking())
            {
                DialogueManager.Instance.StartConvo(radioDialogue);
            }
        }
    }
}