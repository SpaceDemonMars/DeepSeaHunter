using UnityEngine;

public class RadioDialogue : MonoBehaviour
{
    public Dialogue radioDialogue;

    void Update()
    {
        if (Input.GetButtonDown("Radio"))
        {
            if (!DialogueManager.Instance.IsTalking())
            {
                DialogueManager.Instance.StartConvo(radioDialogue);
            }
        }
    }
}