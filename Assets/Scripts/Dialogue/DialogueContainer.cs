/*
 * ---------------------------------------------------------------
 *                      DIALOGUE CONTAINER
 * ---------------------------------------------------------------
 * DESCRIPCIÓN:
 * ScriptableObject contenedor que agrupa todos los dialogos
 * de un NPC, objeto o situación en un solo lugar.
 * 
 * En lugar de conectar DialogueData entre si manualmente,
 * el contenedor actua como el "guion completo" de una entidad.
 * El NPC solo necesita una referencia a este contenedor.
 * 
 * SETUP:
 * Clic derecho en Assets -> Dialogue -> Dialogue Container
 * ---------------------------------------------------------------
 */
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogueContainer",
                 menuName = "Dialogue/Dialogue Container")]
public class DialogueContainer : ScriptableObject
{
    [Header("Información")]
    [Tooltip("Nombre identificador de este contenedor (ej: Evan, Baul_Cuarto1)")]
    public string containerName;

    [Tooltip("Diálogo que se muestra la primera vez que se interactúa")]
    public DialogueData initialDialogue;

    [Tooltip("Diálogo que se repite si ya se completó el inicial")]
    public DialogueData repeatingDialogue;

    [Header("Todos los diálogos de esta entidad")]
    [Tooltip("Aquí van TODOS los DialogueData que pertenecen a esta entidad. " +
             "Solo es para organizacion, el flujo lo controlan las referencias " +
             "dentro de cada DialogueData y el InteractableEntity.")]
    public List<DialogueData> allDialogues;

    // Busca un dialogo por nombre dentro del contenedor
    public DialogueData GetDialogue(string dialogueName)
    {
        return allDialogues.Find(d => d.name == dialogueName);
    }
}