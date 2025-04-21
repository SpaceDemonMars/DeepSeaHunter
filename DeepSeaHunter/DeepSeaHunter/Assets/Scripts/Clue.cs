using UnityEngine;
using TMPro;
using System.Collections;

public class Clue : MonoBehaviour, IInteractable
{
    public int clueID;
    public string clueName;
    [TextArea] public string clueDescription;

    [Header("UI Popup")]
    public TMP_Text cluePopupText;
    public float popupDuration = 2f;

    private bool pickedUp = false;

    private void Start()
    {
        if (cluePopupText == null && JournalManager.instance != null)
        {
            cluePopupText = JournalManager.instance.cluePopupText;
        }
    }

    public void Pickup()
    {
        if (pickedUp) return;
        pickedUp = true;

        JournalManager.instance.DiscoverClue(this); 

        Destroy(gameObject);
    }

    public void Interact()
    {
        Pickup();
    }
}
