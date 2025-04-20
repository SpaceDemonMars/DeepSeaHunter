using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class JournalManager : MonoBehaviour
{
    public static JournalManager instance;

    [Header("Journal UI")]
    public GameObject journalPanel;
    public TMP_Text[] clueEntries; 

    [Header("Popup UI")]
    public TMP_Text cluePopupText;  
    public float popupDuration = 2f;

    private List<Clue> foundClues = new List<Clue>();
    private Coroutine popupCoroutine;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void DiscoverClue(Clue clue)
    {
        if (!foundClues.Contains(clue))
        {
            foundClues.Add(clue);

            clueEntries[clue.clueID].text = clue.clueName + "\n" + clue.clueDescription;
            clueEntries[clue.clueID].gameObject.SetActive(true);

            ShowCluePopup(clue.clueName);
        }
    }

    private void ShowCluePopup(string clueName)
    {
        if (cluePopupText == null)
        {
            Debug.LogWarning("Clue Popup Text is not assigned!");
            return;
        }

        if (popupCoroutine != null)
            StopCoroutine(popupCoroutine);

        cluePopupText.text = $"Found: {clueName}";
        cluePopupText.gameObject.SetActive(true);
        popupCoroutine = StartCoroutine(AutoHidePopup());
    }

    private IEnumerator AutoHidePopup()
    {
        yield return new WaitForSeconds(popupDuration);

        if (cluePopupText != null)
        {
            cluePopupText.gameObject.SetActive(false);
        }
    }

    public void ToggleJournal()
    {
        journalPanel.SetActive(!journalPanel.activeSelf);
    }

    public int GetClueCount()
    {
        return foundClues.Count;
    }

}
