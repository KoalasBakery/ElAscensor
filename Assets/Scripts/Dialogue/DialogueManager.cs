using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI speakerNameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Image speakerPortrait;
    [SerializeField] private GameObject continueIndicator; // La flechita esa que ponen como pa saber que ya acabo el pedo de escribir

    [Header("Typewriter Settings")]
    [SerializeField] private float typingSpeed = 0.05f;

    private DialogueData currentDialogue;
    private int currentLineIndex;
    private bool isTyping;
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

    public void StartDialogue(DialogueData dialogue)
    {
        currentDialogue = dialogue;
        currentLineIndex = 0;
        isDialogueActive = true;

        dialoguePanel.SetActive(true);
        continueIndicator.SetActive(false);

        // Bloqueo el movimiento del jugador
        PlayerController player = FindAnyObjectByType<PlayerController>();
        player?.SetMovementEnabled(false);

        // Cambio el esquema de input a UI
        InputManager.Instance.SwitchToUI();

        ShowLine();
    }

    public void OnContinue()
    {
        // Si esta escribiendo, se completa el texto de golpe
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueText.text = currentDialogue.lines[currentLineIndex].text;
            isTyping = false;
            continueIndicator.SetActive(true);
            return;
        }

        // Siguiente línea
        currentLineIndex++;

        if (currentLineIndex < currentDialogue.lines.Length)
        {
            ShowLine();
        }
        else
        {
            EndDialogue();
        }
    }

    private void ShowLine()
    {
        DialogueData.DialogueLine line = currentDialogue.lines[currentLineIndex];

        speakerNameText.text = line.speakerName;
        continueIndicator.SetActive(false);

        // Portrait PERSONAA tururur
        if (line.speakerPortrait != null)
        {
            speakerPortrait.sprite = line.speakerPortrait;
            speakerPortrait.gameObject.SetActive(true);
        }
        else
        {
            speakerPortrait.gameObject.SetActive(false);
        }

        StopAllCoroutines();
        StartCoroutine(TypeText(line.text));
    }

    private IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        continueIndicator.SetActive(true);
    }

    private void EndDialogue()
    {
        isDialogueActive = false;
        dialoguePanel.SetActive(false);
        currentDialogue = null;

        // Devolvemos el movimiento al jugador
        PlayerController player = FindAnyObjectByType<PlayerController>();
        player?.SetMovementEnabled(true);

        // Regresamos al esquema de Gameplay
        InputManager.Instance.SwitchToGameplay();
    }
}