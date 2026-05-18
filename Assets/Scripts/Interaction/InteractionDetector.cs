using System.Collections;
using UnityEngine;
using TMPro;

/*
 * ---------------------------------------------------------------
 *                   INTERACTION DETECTOR
 * ---------------------------------------------------------------
 * DESCRIPCION:
 * Detecta objetos interactuables cercanos al jugador.
 * Muestra el nombre del objeto con fade in/out.
 * Al presionar E interactua con el mas cercano.
 *
 * SETUP EN UNITY:
 *   1. Agregar este script al Player
 *   2. Crear un Text TMP en el Canvas para el nombre
 *   3. Asignar el texto en el Inspector
 *   4. Configurar el radio de deteccion
 * ---------------------------------------------------------------
 */

public class InteractionDetector : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private float detectionRadius = 2f;
    [SerializeField] private LayerMask interactableLayer;

    [Header("UI")]
    [SerializeField] private CanvasGroup interactionPromptGroup;
    [SerializeField] private TextMeshProUGUI interactionPromptText;
    [SerializeField] private float fadeSpeed = 5f;

    private Interactable currentInteractable;
    private bool isPromptVisible = false;
    private Coroutine fadeCoroutine;

    private void Update()
    {
        DetectInteractable();
    }

    private void DetectInteractable()
    {
        // Buscar el interactuable mas cercano
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position, detectionRadius, interactableLayer);

        Interactable closest = null;
        float closestDistance = float.MaxValue;

        foreach (var hit in hits)
        {
            Interactable interactable = hit.GetComponent<Interactable>();
            if (interactable == null || !interactable.canInteract) continue;

            float distance = Vector2.Distance(
                transform.position, hit.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = interactable;
            }
        }

        // Si cambio el interactuable cercano
        if (closest != currentInteractable)
        {
            currentInteractable = closest;

            if (currentInteractable != null)
                ShowPrompt(currentInteractable.interactionPrompt);
            else
                HidePrompt();
        }
    }

    // --- INTERACTUAR --- //
    public void TryInteract()
    {
        if (currentInteractable != null && currentInteractable.canInteract)
            currentInteractable.Interact();
    }

    // --- PROMPT --- //
    private void ShowPrompt(string text)
    {
        if (interactionPromptText != null)
            interactionPromptText.text = text;

        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadePrompt(true));
    }

    private void HidePrompt()
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadePrompt(false));
    }

    private IEnumerator FadePrompt(bool show)
    {
        float target = show ? 1f : 0f;
        float current = interactionPromptGroup.alpha;

        while (Mathf.Abs(current - target) > 0.01f)
        {
            current = Mathf.Lerp(current, target, Time.deltaTime * fadeSpeed);
            interactionPromptGroup.alpha = current;
            yield return null;
        }

        interactionPromptGroup.alpha = target;
        isPromptVisible = show;
    }

    // --- GIZMOS --- //
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}