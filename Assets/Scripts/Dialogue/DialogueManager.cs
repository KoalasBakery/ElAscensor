using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

/*
 * ---------------------------------------------------------------
 *                      DIALOGUE MANAGER
 * ---------------------------------------------------------------
 * 
 * DESCRIPCION:
 * Singleton que controla el flujo completo de todos los dialogos
 * del juego. Es el cerebro del sistema de Dialogos.
 * 
 * FUNCIONAMIENTO:
 *   - Iniciar, avanzar y terminar dialogos
 *   - Obtener textos localizados (ingles/español)
 *   - Filtrar choices segun flags activas
 *   - Activar flags por linea o al completar un dialogo
 *   - Bloquear/desbloquear el movimiento del jugador
 *   - Cambiar el esquema de input (Gameplay <-> UI)
 * 
 * COMO INICIAR UN DIALOGO DESDE OTRO SCRIPT:
 *   DialogueManager.Instance.StartDialogue(miDialogueData);
 * 
 *   Con nombre del speaker:
 *   DialogueManager.Instance.StartDialogue(miDialogueData, "Evan");
 * 
 * FLUJO INTERNO:
 * 
 *   StartDialogue()
 *       |
 *   Ya completado y no repetible? -> No inicia
 *       |
 *   Bloquea movimiento + cambia input a UI
 *       |
 *   ShowLine() -> obtiene texto localizado -> DialogueUI.ShowLine()
 *       |
 *   Tiene choices? -> ShowChoices() -> espera seleccion
 *       |
 *   OnContinue() (al presionar E)
 *       |
 *   Est escribiendo? -> SkipTypewriter()
 *   Hay choices activas? -> No avanza
 *   LInea tiene nextDialogue? -> StartDialogue(nextDialogue)
 *   Hay mas lineas? -> siguiente lInea
 *   No hay mas? -> EndDialogue()
 *       |
 *   EndDialogue()
 *       |
 *   Activa completedFlagKey + desbloquea movimiento + regresa a Gameplay
 * 
 * LOCALIZACION:
 * Usa Unity Localization Package para obtener textos.
 * La tabla se llama "Story" y tiene columnas en/es.
 * Se obtienen de forma asíncrona con GetLocalizedStringAsync().
 * 
 * IMPORTANTE:
 * Este script vive en el GameObject "DialogueManager" en la escena.
 * NO esta en DontDestroyOnLoad porque pertenece a cada escena.
 * La referencia al DialogueUI se asigna en el Inspector.
 * 
 * DEPENDENCIAS:
 *   - DialogueUI      (muestra el texto en pantalla)
 *   - FlagManager     (activa/lee flags)
 *   - InputManager    (cambia esquemas de control)
 *   - PlayerController (bloquea movimiento)
 * ---------------------------------------------------------------
 */

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("UI Reference")]
    [SerializeField] private DialogueUI dialogueUI;

    private string currentSpeakerName = "";
    private DialogueData currentDialogue;
    private int currentLineIndex;
    private bool isDialogueActive;

    public bool IsDialogueActive => isDialogueActive;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // --- INICIAR DIALOGO --- //
    public void StartDialogue(DialogueData dialogue, string speakerName = "")
    {
        if (dialogue == null) return;

        if (!dialogue.isRepeatable && !string.IsNullOrEmpty(dialogue.completedFlagKey))
        {
            if (FlagManager.Instance.GetFlag(dialogue.completedFlagKey)) return;
        }

        currentDialogue = dialogue;
        currentSpeakerName = speakerName; // guardar nombre
        currentLineIndex = 0;
        isDialogueActive = true;

        PlayerController player = FindAnyObjectByType<PlayerController>();
        player?.SetMovementEnabled(false);
        InputManager.Instance.SwitchToUI();

        dialogueUI.Show();
        ShowLine();
    }

    // --- MOSTRAR LINEA ACTUAL --- //
    private void ShowLine()
    {
        if (currentLineIndex >= currentDialogue.lines.Count)
        {
            EndDialogue();
            return;
        }

        DialogueData.DialogueLine line = currentDialogue.lines[currentLineIndex];

        if (!string.IsNullOrEmpty(line.setFlagKey))
            FlagManager.Instance.SetFlag(line.setFlagKey, line.setFlagValue);

        StartCoroutine(ShowLineLocalized(line, currentSpeakerName));
    }

    private IEnumerator ShowLineLocalized(DialogueData.DialogueLine line, string speakerNameOverride = "")
    {
        // Si hay override de nombre, usarlo directo
        string speakerName = speakerNameOverride;

        // Solo buscar en localization si no hay override
        if (string.IsNullOrEmpty(speakerName) && !string.IsNullOrEmpty(line.speakerNameKey))
        {
            var nameOp = LocalizationSettings.StringDatabase.GetLocalizedStringAsync("Story", line.speakerNameKey);
            yield return nameOp;
            speakerName = nameOp.Result;
        }

        // Texto localizado igual que antes
        string dialogueText = "";
        if (!string.IsNullOrEmpty(line.dialogueTextKey))
        {
            var textOp = LocalizationSettings.StringDatabase.GetLocalizedStringAsync("Story", line.dialogueTextKey);
            yield return textOp;
            dialogueText = textOp.Result;
        }

        dialogueUI.ShowLine(speakerName, dialogueText, line.speakerPortrait);

        if (line.hasChoices && line.choices != null && line.choices.Count > 0)
            ShowChoices(line.choices);
    }

    // --- CHOICES --- //
    private void ShowChoices(List<DialogueData.DialogueChoice> choices)
    {
        List<DialogueData.DialogueChoice> availableChoices = new List<DialogueData.DialogueChoice>();

        foreach (var choice in choices)
        {
            // Solo mostrar choices cuya flag requerida este activa
            if (string.IsNullOrEmpty(choice.requiredFlagKey) ||
                FlagManager.Instance.GetFlag(choice.requiredFlagKey))
            {
                availableChoices.Add(choice);
            }
        }

        StartCoroutine(ShowChoicesLocalized(availableChoices));
    }

    private IEnumerator ShowChoicesLocalized(List<DialogueData.DialogueChoice> choices)
    {
        List<string> choiceTexts = new List<string>();

        foreach (var choice in choices)
        {
            var op = LocalizationSettings.StringDatabase.GetLocalizedStringAsync("Story", choice.choiceTextKey);
            yield return op;
            choiceTexts.Add(op.Result);
        }

        dialogueUI.ShowChoices(choiceTexts, (index) => OnChoiceSelected(choices[index]));
    }

    private void OnChoiceSelected(DialogueData.DialogueChoice choice)
    {
        // Limpiar choices primero
        dialogueUI.HideChoices();

        // Activar flag de la choice
        if (!string.IsNullOrEmpty(choice.setFlagKey))
            FlagManager.Instance.SetFlag(choice.setFlagKey, choice.setFlagValue);

        // Si tiene dialogo siguiente, lo iniciamos
        if (choice.nextDialogue != null)
        {
            StartDialogue(choice.nextDialogue);
            return;
        }

        // Si no, avanzamos a la siguiente linea
        currentLineIndex++;
        ShowLine();
    }

    // --- CONTINUAR --- //
    public void OnContinue()
    {
        // Si la UI esta escribiendo, la completamos
        if (dialogueUI.IsTyping)
        {
            dialogueUI.SkipTypewriter();
            return;
        }

        // Si hay choices activas, no avanzamos con E
        if (dialogueUI.IsShowingChoices) return;

        // Si la linea actual tiene nextDialogue, lo iniciamos
        DialogueData.DialogueLine currentLine = currentDialogue.lines[currentLineIndex];
        if (currentLine.nextDialogue != null)
        {
            StartDialogue(currentLine.nextDialogue);
            return;
        }

        // Avanzamos a la siguiente linea
        currentLineIndex++;
        ShowLine();
    }

    // --- TERMINAR DIALOGO --- //
    private void EndDialogue()
    {
        // Activar flag de completado
        if (!string.IsNullOrEmpty(currentDialogue.completedFlagKey))
            FlagManager.Instance.SetFlag(currentDialogue.completedFlagKey, true);

        isDialogueActive = false;
        currentDialogue = null;

        dialogueUI.Hide();

        // Devolvemos control al jugador
        PlayerController player = FindAnyObjectByType<PlayerController>();
        player?.SetMovementEnabled(true);
        InputManager.Instance.SwitchToGameplay();
    }
}