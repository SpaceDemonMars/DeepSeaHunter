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

    [Header("Interaction Prompt")]
    public string interactPromptText = "Press [E] to Pick Up";  // <-- NEW!!

    private bool pickedUp = false;
    private Coroutine popupCoroutine;

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

        if (JournalManager.instance != null) 
            JournalManager.instance.DiscoverClue(this);

        if (cluePopupText != null)
        {
            if (popupCoroutine != null)
                StopCoroutine(popupCoroutine);

            cluePopupText.text = $"Found: {clueName}";
            cluePopupText.gameObject.SetActive(true);

            popupCoroutine = StartCoroutine(HidePopupAfterDelay());
        }

        Destroy(gameObject);
    }

    public void Interact()
    {
        Pickup();
    }

    private IEnumerator HidePopupAfterDelay()
    {
        yield return new WaitForSeconds(popupDuration);

        if (cluePopupText != null)
        {
            cluePopupText.gameObject.SetActive(false);
        }
    }
}
