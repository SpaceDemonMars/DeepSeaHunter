using UnityEngine;
using TMPro;

public class PlayerInteract : MonoBehaviour
{
    public float interactRange = 3f;
    public LayerMask interactableLayer;
    public TMP_Text interactPrompt; 

    private IInteractable currentInteractable;

    private void Update()
    {
        CheckForInteractables();

        if (currentInteractable != null && Input.GetButtonDown("Interact"))
        {
            currentInteractable.Interact();
            interactPrompt.gameObject.SetActive(false);
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
                interactPrompt.text = "Press [E] to Interact";
                interactPrompt.gameObject.SetActive(true);
                return;
            }
        }

        currentInteractable = null;
        interactPrompt.gameObject.SetActive(false);
    }
}
