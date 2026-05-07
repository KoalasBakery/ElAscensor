using System.Collections.Generic;
using UnityEngine;

/*
 * ---------------------------------------------------------------
 *                       DIALOGUE DATA
 * ---------------------------------------------------------------
 * 
 * DESCRIPCION:
 * ScriptableObject que contiene toda la informacion de un dialogo.
 * Se crea desde: clic derecho en Assets -> Dialogue -> Dialogue Data
 * 
 * ESTRUCTURA:
 * Un DialogueData contiene una lista de DialogueLines (lineas).
 * Cada linea puede tener opciones de respuesta (choices) que
 * ramifican hacia otros DialogueData distintos.
 * 
 * CAMPOS DE CADA LINEA (DialogueLine):
 * 
 *    LOCALIZACION
 *      - speakerNameKey   -> Key del nombre en la Localization Table
 *                           (dejar vacío si el nombre viene del 
 *                            InteractableEntity.entityName)
 *      - dialogueTextKey  -> Key del texto en la Localization Table
 *                           Ejemplo: "Evan.Linea01"
 * 
 *    PORTRAIT
 *      - speakerPortrait  -> Sprite del personaje que habla
 *                           (opcional, se oculta si esta vacio)
 * 
 *    CHOICES (opciones de respuesta)
 *      - hasChoices       -> Activa el sistema de choices en esta línea
 *      - choices          -> Lista de opciones disponibles
 *        Cada choice tiene:
 *          · choiceTextKey    -> Key del texto de la opcion
 *          · nextDialogue     -> DialogueData al que lleva esta opcion
 *          · requiredFlagKey  -> Solo muestra esta opción si la flag
 *                               esta activa (opcional)
 *          · setFlagKey       -> Flag que activa al elegir esta opción
 * 
 *    EVENTS (efectos al llegar a esta linea)
 *      - setFlagKey       -> Flag que se activa al mostrar esta línea
 *      - setFlagValue     -> Valor que se le asigna a la flag
 *      - nextDialogue     -> Dialogo que continua tras esta linea
 * 
 * CAMPOS GLOBALES DEL SO:
 *      - isRepeatable     -> Si el dialogo puede iniciarse multiples
 *                           veces aunque tenga completedFlagKey
 *      - completedFlagKey -> Flag que se activa al terminar el dialogo
 *                           Usala para saber si ya se vio este dialogo
 * 
 * FLUJO DE UN DIALOGO:
 * 
 *   Linea 1 -> Linea 2 -> Linea con choices
 *                              |           |
 *                         Opcion A     Opcion B
 *                              |           |
 *                        Dialogo X   Dialogo Y
 * 
 * LOCALIZACION:
 * Los textos NO se escriben directamente aqui, solo las Keys.
 * Los textos van en: Window -> Asset Management -> Localization Tables
 * Tabla: "Story"(por ahora, luego dividimos) con columnas English (en) y Spanish (es)
 * 
 * EJEMPLO DE KEYS:
 *   "NPC.Evan.Saludo"     -> "Hey there!" / "¡Oye!"
 *   "NPC.Evan.Pregunta"   -> "Found my disc?" / "¿Encontraste mi disco?"
 *   "Choice.Si"           -> "Yes!" / "¡Sí!"
 *   "Choice.No"           -> "Not yet" / "Aun no"
 * 
 * DEPENDENCIAS:
 *   - DialogueManager     (quien ejecuta este SO)
 *   - FlagManager         (para flags)
 *   - LocalizationTables  (para los textos)
 * ---------------------------------------------------------------
 */

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    [System.Serializable]
    public class DialogueLine
    {
        [Header("Localization")]
        public string speakerNameKey;      
        public string dialogueTextKey;     

        [Header("Portrait")]
        public Sprite speakerPortrait;

        [Header("Choices")]
        public bool hasChoices = false;
        public List<DialogueChoice> choices;

        [Header("Events")]
        public string setFlagKey;          
        public bool setFlagValue = true;
        public DialogueData nextDialogue;  
    }

    [System.Serializable]
    public class DialogueChoice
    {
        public string choiceTextKey;       // Key del texto de la opcion
        public DialogueData nextDialogue;  // Dialogo al que lleva esta opcion
        public string requiredFlagKey;     // Flag necesaria para mostrar esta opcion
        public string setFlagKey;          // Flag que activa al elegir esta opcion
        public bool setFlagValue = true;
    }

    [Header("Dialogue Lines")]
    public List<DialogueLine> lines;

    [Header("Settings")]
    public bool isRepeatable = false;     // Si puede activarse mas de una vez
    public string completedFlagKey;       // Flag que se activa al terminar el dialogo
}