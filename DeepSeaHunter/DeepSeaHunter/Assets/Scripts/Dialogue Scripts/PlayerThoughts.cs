using UnityEngine;
using TMPro;

public class PlayerThoughts : MonoBehaviour
{
    public static PlayerThoughts Instance;

    public TextMeshProUGUI thoughtText;
    public GameObject thoughtPanel;
    public TextMeshProUGUI promptText;

    private bool isWaitingForDismiss = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Update()
    {
        if (isWaitingForDismiss && Input.GetButtonDown("Interact"))
            {
            HideThought();
        }
    }

    public void ShowThought(string text)
    {
        thoughtText.text = text;
        promptText.text = "Press [E] to continue";
        thoughtPanel.SetActive(true);
        isWaitingForDismiss = true;
    }

    private void HideThought()
    {
        isWaitingForDismiss = false;
        thoughtPanel.SetActive(false);
    }

    public bool IsShowingThought() => isWaitingForDismiss;
}
