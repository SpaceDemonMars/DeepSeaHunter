using UnityEngine;
using TMPro;

public class PlayerInteract : MonoBehaviour
{
    public float interactRange = 3f;

    private IInteractable currentInteractable;

    private void Update()
    {
        CheckForInteractables();

        if (currentInteractable != null && Input.GetButtonDown("Interact"))
        {
            currentInteractable.Interact();
            GameManager.instance.interactPrompt.gameObject.SetActive(false); 
        }
    }

    void CheckForInteractables()
    {
        Ray ray = new Ray(transform.position + Vector3.up, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactRange))
        {
            if (hit.collider.CompareTag("Interactable")) 
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
        }

        currentInteractable = null;
        GameManager.instance.interactPrompt.gameObject.SetActive(false);
    }
}
