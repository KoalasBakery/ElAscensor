using UnityEngine;

public class InteractionDetector : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private float detectionRadius = 1.5f;
    [SerializeField] private LayerMask interactableLayer;

    private Interactable currentInteractable;

    void Update()
    {
        DetectInteractable();
    }

    private void DetectInteractable()
    {
        Collider2D hit = Physics2D.OverlapCircle(
            transform.position,
            detectionRadius,
            interactableLayer
        );

        if (hit != null)
        {
            Interactable interactable = hit.GetComponent<Interactable>();

            if (interactable != null && interactable.canInteract)
            {
                // Avisamos que el jugador entró
                if (currentInteractable != interactable)
                {
                    currentInteractable?.OnPlayerExit();
                    currentInteractable = interactable;
                    currentInteractable.OnPlayerEnter();
                }
            }
        }
        else
        {
            // No hay nada cerca
            if (currentInteractable != null)
            {
                currentInteractable.OnPlayerExit();
                currentInteractable = null;
            }
        }
    }

    // Lo llamara el InputManager cuando se presione E
    public void TryInteract()
    {
        if (currentInteractable != null && currentInteractable.canInteract)
            currentInteractable.Interact();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}