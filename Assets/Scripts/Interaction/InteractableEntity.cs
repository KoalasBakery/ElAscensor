using System.Collections.Generic;
using UnityEngine;

/*
 * ---------------------------------------------------------------
 *                    INTERACTABLE ENTITY
 * ---------------------------------------------------------------
 * 
 * DESCRIPCION:
 * Componente universal para cualquier entidad interactuable del juego.
 * Este sistema maneja NPCs, objetos inspeccionables,
 * cofres, puertas, objetos de puzzle y cualquier cosa con la que el
 * jugador pueda interactuar, no vi necesario hacer un sistema separado.
 * 
 * FUNCIONAMIENTO:
 * Al presionar E cerca de un objeto con este componente, el sistema
 * lee las condiciones en este orden:
 * 
 *   1. CONDICIONES (ConditionalDialogues)
 *      Siempre se lee primero, sin importar si ya interactuo antes.
 *      Cada condicion puede requerir:
 *        - Un ítem específico en el inventario
 *        - Una flag activa en el FlagManager
 *      Si la condición se cumple, dispara su propio dialogo y efectos.
 * 
 *   2. REPEATING DIALOGUE
 *      Si ninguna condicion aplica y el objeto ya fue interactuado
 *      (hasInteracted = true) con isOneTimeInteraction activo,
 *      muestra el dialogo de repeticion (ej: "El baul esta vacio").
 * 
 *   3. DEFAULT DIALOGUE
 *      Si no hay condiciones ni interaccion previa, muestra el
 *      dialogo por defecto y ejecuta los efectos base.
 * 
 * EFECTOS DISPONIBLES:
 *   - Iniciar un dialogo (DialogueData)
 *   - Dar un item al inventario del jugador
 *   - Quitar un ítem del inventario (removeRequiredItem)
 *   - Activar o desactivar una flag global (FlagManager)
 * 
 * CASOS DE USO:
 * 
 *    NPC PARLANTE (ej: Evan)
 *      - entityName = "Evan"
 *      - useEntityNameAsSpeaker = true
 *      - defaultDialogue = saludo inicial
 *      - repeatingDialogue = "Ya vete wey"
 *      - conditionalDialogue = si tienes el disco -> dialogo especial
 *      - isOneTimeInteraction = true
 * 
 *    OBJETO INSPECCIONABLE (ej: cuadro)
 *      - entityName = "" (sin nombre)
 *      - defaultDialogue = "Ese cuadro que?"
 *      - isOneTimeInteraction = false (siempre dice lo mismo)
 * 
 *    BAUL CON ITEM
 *      - defaultDialogue = "Encontre un disco"
 *      - itemToGive = DiscoItem
 *      - repeatingDialogue = "El baul esta vacio"
 *      - isOneTimeInteraction = true
 * 
 *    OBJETO DE PUZZLE
 *      - conditionalDialogue con requiredItem = PiezaPuzzle
 *      - setFlagKey = "PuzzleResuelto"
 *      - removeRequiredItem = true
 * 
 * DEPENDENCIAS:
 *   - DialogueManager    (sistema de dialogos)
 *   - FlagManager        (sistema de flags globales)
 *   - Inventory          (sistema de inventario)
 *   - Interactable       (clase base abstracta)
 * 
 * SETUP EN UNITY:
 *   1. Agregar este componente al GameObject
 *   2. Asignar Layer "Interactable"
 *   3. Asegurarse que el BoxCollider2D tenga Is Trigger = true
 *   4. Configurar los campos en el Inspector
 *   5. Crear los DialogueData SOs necesarios
 * ---------------------------------------------------------------
 */

public class InteractableEntity : Interactable
{
    [System.Serializable]
    public class ConditionalDialogue
    {
        [Header("Condición")]
        public string requiredFlagKey;       
        public ItemData requiredItem;        

        [Header("Diálogo")]
        public DialogueData dialogue;        

        [Header("Efectos al interactuar")]
        public ItemData itemToGive;          
        public string setFlagKey;            
        public bool setFlagValue = true;
        public bool removeRequiredItem = false;
    }

    [Header("Identidad")]
    [SerializeField] private string entityName = "Objeto";
    [SerializeField] private bool useEntityNameAsSpeaker = true;

    [Header("Diálogos")]
    [SerializeField] private DialogueData defaultDialogue;
    [SerializeField] private DialogueData repeatingDialogue;
    [SerializeField] private List<ConditionalDialogue> conditionalDialogues;

    [Header("Efectos por defecto")]
    [SerializeField] private ItemData itemToGive;
    [SerializeField] private string setFlagKey;
    [SerializeField] private bool setFlagValue = true;

    [Header("Settings")]
    [SerializeField] private bool isOneTimeInteraction = false;

    private bool hasInteracted = false;

    public override void Interact()
    {
        if (!canInteract) return;

        // SIEMPRE revisar condiciones primero, sin importar si ya interactuo
        foreach (var conditional in conditionalDialogues)
        {
            if (MeetsCondition(conditional))
            {
                TriggerConditional(conditional);
                return;
            }
        }

        // Si es de una sola interaccion y ya interactuo, mostrar repeating
        if (isOneTimeInteraction && hasInteracted)
        {
            if (repeatingDialogue != null)
            {
                string speaker = useEntityNameAsSpeaker ? entityName : "";
                DialogueManager.Instance.StartDialogue(repeatingDialogue, speaker);
            }
            return;
        }

        // Si no hay condicion ni interaccion previa, usar default
        TriggerDefault();
    }

    private bool MeetsCondition(ConditionalDialogue conditional)
    {
        // Revisar flag requerida
        if (!string.IsNullOrEmpty(conditional.requiredFlagKey))
        {
            if (!FlagManager.Instance.GetFlag(conditional.requiredFlagKey))
                return false;
        }

        // Revisar ítem requerido
        if (conditional.requiredItem != null)
        {
            if (!Inventory.Instance.HasItem(conditional.requiredItem))
                return false;
        }

        return true;
    }

    private void TriggerDefault()
    {
        if (defaultDialogue != null)
        {
            string speaker = useEntityNameAsSpeaker ? entityName : "";
            DialogueManager.Instance.StartDialogue(defaultDialogue, speaker);
        }

        if (itemToGive != null)
            Inventory.Instance.AddItem(itemToGive);

        if (!string.IsNullOrEmpty(setFlagKey))
            FlagManager.Instance.SetFlag(setFlagKey, setFlagValue);

        hasInteracted = true;
    }

    private void TriggerConditional(ConditionalDialogue conditional)
    {
        if (conditional.dialogue != null)
        {
            string speaker = useEntityNameAsSpeaker ? entityName : "";
            DialogueManager.Instance.StartDialogue(conditional.dialogue, speaker);
        }

        if (conditional.itemToGive != null)
            Inventory.Instance.AddItem(conditional.itemToGive);

        if (conditional.removeRequiredItem && conditional.requiredItem != null)
            Inventory.Instance.RemoveItem(conditional.requiredItem);

        if (!string.IsNullOrEmpty(conditional.setFlagKey))
            FlagManager.Instance.SetFlag(conditional.setFlagKey, conditional.setFlagValue);

        hasInteracted = true;
    }

    public override void OnPlayerEnter()
    {
        Debug.Log("Cerca de: " + entityName);
    }
}