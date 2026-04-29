using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [Header("Interaction Settings")]
    public string interactionPrompt = "Presiona E para interactuar"; // Texto a aparecer en pantalla (temporal)
    public bool canInteract = true;

    // Cada objeto usa esto a su manera (sobreescribir)
    public abstract void Interact();

    // Para cuando el jugador se acerca
    public virtual void OnPlayerEnter() { }

    // Para cuando el jugador se aleja
    public virtual void OnPlayerExit() { }
}