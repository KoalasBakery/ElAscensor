/*
 * ---------------------------------------------------------------
 *                    INTERACTABLE ENTITY
 * ---------------------------------------------------------------
 * DESCRIPCIÓN:
 * Componente universal para cualquier entidad interactuable.
 * Ahora usa DialogueContainer en lugar de DialogueData directos,
 * lo que permite tener todos los dialogos organizados en un solo SO.
 *
 * FLUJO:
 *   1. Condiciones (siempre se evaluan primero)
 *   2. Repeating (si ya interactuo)
 *   3. Default (primera vez)
 * ---------------------------------------------------------------
 */
using System.Collections.Generic;
using UnityEngine;

public class InteractableEntity : Interactable
{
    [System.Serializable]
    public class ConditionalDialogue
    {
        [Header("Condición")]
        [Tooltip("Flag necesaria para que este dialogo aparezca")]
        public string requiredFlagKey;
        [Tooltip("Item necesario en el inventario para este dialogo")]
        public ItemData requiredItem;

        [Header("Diálogo")]
        [Tooltip("Nombre del DialogueData dentro del contenedor")]
        public string dialogueName;

        [Header("Efectos")]
        [Tooltip("Item que se da al jugador al triggear esta condicion")]
        public ItemData itemToGive;
        [Tooltip("¿Se quita el item requerido del inventario?")]
        public bool removeRequiredItem = false;
        [Tooltip("Flag que se activa al triggear esta condicion")]
        public string setFlagKey;
        public bool setFlagValue = true;
    }

    [Header("Contenedor")]
    [Tooltip("SO que contiene todos los dialogos de esta entidad")]
    [SerializeField] private DialogueContainer dialogueContainer;

    [Header("Condiciones")]
    [Tooltip("Dialogos condicionales, se evaluan en orden de arriba a abajo")]
    [SerializeField] private List<ConditionalDialogue> conditionalDialogues;

    [Header("Efectos por defecto")]
    [Tooltip("Item que da al interactuar por primera vez")]
    [SerializeField] private ItemData itemToGive;
    [Tooltip("Flag que activa al interactuar por primera vez")]
    [SerializeField] private string setFlagKey;
    public bool setFlagValue = true;

    [Header("Identidad")]
    [Tooltip("Nombre que aparece en la caja de dialogo")]
    [SerializeField] private string entityName = "";
    [Tooltip("Si esta activo, usa entityName en lugar de la key de localizacion")]
    [SerializeField] private bool useEntityNameAsSpeaker = true;

    [Header("Settings")]
    [Tooltip("Si está activo, solo dispara el default una vez y luego el repeating")]
    [SerializeField] private bool isOneTimeInteraction = false;

    private bool hasInteracted = false;

    public override void Interact()
    {
        if (!canInteract) return;
        if (dialogueContainer == null)
        {
            Debug.LogWarning("InteractableEntity: No hay DialogueContainer asignado en " + gameObject.name);
            return;
        }

        // SIEMPRE revisar condiciones primero
        foreach (var conditional in conditionalDialogues)
        {
            if (MeetsCondition(conditional))
            {
                TriggerConditional(conditional);
                return;
            }
        }

        // Si ya interactuo y es one time, mostrar repeating
        if (isOneTimeInteraction && hasInteracted)
        {
            if (dialogueContainer.repeatingDialogue != null)
            {
                string speaker = useEntityNameAsSpeaker ? entityName : "";
                DialogueManager.Instance.StartDialogue(
                    dialogueContainer.repeatingDialogue, speaker);
            }
            return;
        }

        // Default
        TriggerDefault();
    }

    private bool MeetsCondition(ConditionalDialogue conditional)
    {
        if (!string.IsNullOrEmpty(conditional.requiredFlagKey))
            if (!FlagManager.Instance.GetFlag(conditional.requiredFlagKey))
                return false;

        if (conditional.requiredItem != null)
            if (!Inventory.Instance.HasItem(conditional.requiredItem))
                return false;

        return true;
    }

    private void TriggerConditional(ConditionalDialogue conditional)
    {
        // Buscar el diálogo en el contenedor
        DialogueData dialogue = dialogueContainer.GetDialogue(conditional.dialogueName);
        if (dialogue != null)
        {
            string speaker = useEntityNameAsSpeaker ? entityName : "";
            DialogueManager.Instance.StartDialogue(dialogue, speaker);
        }
        else
        {
            Debug.LogWarning($"InteractableEntity: No se encontro el dialogo " +
                           $"'{conditional.dialogueName}' en el contenedor " +
                           $"'{dialogueContainer.containerName}'");
        }

        if (conditional.itemToGive != null)
            Inventory.Instance.AddItem(conditional.itemToGive);

        if (conditional.removeRequiredItem && conditional.requiredItem != null)
            Inventory.Instance.RemoveItem(conditional.requiredItem);

        if (!string.IsNullOrEmpty(conditional.setFlagKey))
            FlagManager.Instance.SetFlag(conditional.setFlagKey, conditional.setFlagValue);

        hasInteracted = true;
    }

    private void TriggerDefault()
    {
        if (dialogueContainer.initialDialogue != null)
        {
            string speaker = useEntityNameAsSpeaker ? entityName : "";
            DialogueManager.Instance.StartDialogue(
                dialogueContainer.initialDialogue, speaker);
        }

        if (itemToGive != null)
            Inventory.Instance.AddItem(itemToGive);

        if (!string.IsNullOrEmpty(setFlagKey))
            FlagManager.Instance.SetFlag(setFlagKey, setFlagValue);

        hasInteracted = true;
    }

    public override void OnPlayerEnter()
    {
        if (!string.IsNullOrEmpty(entityName))
            Debug.Log("Cerca de: " + entityName);
    }
}