using UnityEngine;
using TMPro;

public class PlayerInteract : MonoBehaviour
{
    //pushing a fix and leaving you notes :3 (delete these after you've read them)
    public float interactRange = 3f;
    public LayerMask interactableLayer;
    //public TMP_Text interactPrompt; this is a UI element, so this line gets moved to gameManager

    private IInteractable currentInteractable;

    private void Update()
    {
        CheckForInteractables();

        if (currentInteractable != null && Input.GetButtonDown("Interact"))
        {
            currentInteractable.Interact();
            GameManager.instance.interactPrompt.gameObject.SetActive(false); //toggle active from gameManager
        }
    }

    void CheckForInteractables()
    {
        Ray ray = new Ray(transform.position + Vector3.up, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactRange, interactableLayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                currentInteractable = interactable;
                GameManager.instance.interactPrompt.text = "Press [E] to Interact";
                GameManager.instance.interactPrompt.gameObject.SetActive(true);
                return;
            }
        }

        currentInteractable = null;
        GameManager.instance.interactPrompt.gameObject.SetActive(false);
    }
}
