using System.Collections;
using UnityEngine;

public static class DialogueChain
{
    public static IEnumerator ThoughtonIntro(string thoughtText, Dialogue npcDialogue)
    {
        PlayerThoughts.Instance.ShowThought(thoughtText);

        while (PlayerThoughts.Instance.IsShowingThought())
        {
            yield return null;
        }

        DialogueManager.Instance.StartConvo(npcDialogue);
    }
}
