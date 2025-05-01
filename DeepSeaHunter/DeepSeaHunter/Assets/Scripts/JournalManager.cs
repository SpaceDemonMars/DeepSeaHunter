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

    private List<int> foundClueIDs;
    private Coroutine popupCoroutine;

    private void Awake()
    {
        instance = this;
        foundClueIDs = new List<int>();
    }

    private void Start()
    {
        foundClueIDs = new List<int>();

        if (GameManager.instance != null)
        {
            List<int> savedClueIDs = GameManager.instance.GetFoundClueIDs();
            foreach (int id in savedClueIDs)
            {
                AddClueEntry(id);
            }
        }
    }


    public void DiscoverClue(Clue clue)
    {
        if (!foundClueIDs.Contains(clue.clueID))
        {
            foundClueIDs.Add(clue.clueID);
            AddClueEntry(clue.clueID, clue.clueName, clue.clueDescription);
            ShowCluePopup(clue.clueName);
        }
    }

    public bool HasFoundClue(int clueID)
    {
        return foundClueIDs.Contains(clueID);
    }

    private void AddClueEntry(int clueID, string clueName = "", string clueDescription = "")
    {
        if (clueEntries.Length > clueID)
        {
            if (string.IsNullOrEmpty(clueName)) clueName = $"Clue {clueID}";
            if (string.IsNullOrEmpty(clueDescription)) clueDescription = "(Description not loaded)";

            clueEntries[clueID].text = clueName + "\n" + clueDescription;
            clueEntries[clueID].gameObject.SetActive(true);
        }
    }

    private void ShowCluePopup(string clueName)
    {
        if (cluePopupText == null)
        {
    //        Debug.LogWarning("Clue Popup Text is not assigned!");
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
        return foundClueIDs.Count;
    }

}
