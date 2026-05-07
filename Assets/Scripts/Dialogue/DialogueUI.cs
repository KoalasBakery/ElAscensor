using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
 * ---------------------------------------------------------------
 *                        DIALOGUE UI
 * ---------------------------------------------------------------
 * 
 * DESCRIPCION:
 * Maneja toda la parte visual del sistema de dialogos.
 * Separado del DialogueManager para respetar el principio
 * de responsabilidad unica (cada script hace una sola cosa).
 * 
 * RESPONSABILIDADES:
 *   - Mostrar/ocultar el panel de dialogo
 *   - Efecto typewriter (texto letra por letra)
 *   - Mostrar nombre y portrait del speaker
 *   - Crear y destruir botones de choices dinamicamente
 *   - Exponer estados (IsTyping, IsShowingChoices) al Manager
 * 
 * METODOS PUBLICOS:
 * 
 *   Show()
 *   -> Activa el panel de dialogo
 * 
 *   Hide()
 *   -> Desactiva el panel y limpia choices
 * 
 *   ShowLine(speakerName, text, portrait)
 *   -> Muestra una linea con efecto typewriter
 *   -> Si portrait es null, oculta la imagen del speaker
 * 
 *   SkipTypewriter()
 *   -> Completa el texto de golpe si esta escribiendo
 * 
 *   ShowChoices(textos, callback)
 *   -> Crea botones dinámicamente para cada opción
 *   -> Al clickear un boton llama al callback con el indice elegido
 * 
 *   HideChoices()
 *   -> Destruye todos los botones y oculta el panel de choices
 * 
 * PROPIEDADES:
 *   IsTyping          -> true mientras el typewriter esta escribiendo
 *   IsShowingChoices  -> true mientras hay choices visibles
 * 
 * SETUP EN UNITY:
 *   Este script va en el Canvas (siempre activo).
 *   El DialoguePanel empieza DESACTIVADO en la Hierarchy.
 *   Asignar en el Inspector:
 *     · Dialogue Panel    -> DialoguePanel
 *     · Speaker Name Text -> SpeakerName (TMP)
 *     · Dialogue Text     -> DialogueText (TMP)
 *     · Speaker Portrait  -> SpeakerPortrait (Image)
 *     · Continue Indicator -> ContinueIndicator (">>" o flecha)
 *     · Choices Panel     -> ChoicesPanel
 *     · Choice Button Prefab -> Prefab con Button + TMP
 * 
 * DEPENDENCIAS:
 *   - TextMeshPro     (textos)
 *   - UnityEngine.UI  (botones e imagenes)
 * ---------------------------------------------------------------
 */

public class DialogueUI : MonoBehaviour
{
    [Header("Main Panel")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI speakerNameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Image speakerPortrait;
    [SerializeField] private GameObject continueIndicator;

    [Header("Choices")]
    [SerializeField] private GameObject choicesPanel;
    [SerializeField] private GameObject choiceButtonPrefab;

    [Header("Typewriter")]
    [SerializeField] private float typingSpeed = 0.05f;

    private string fullText;
    private Coroutine typingCoroutine;
    private List<GameObject> activeChoiceButtons = new List<GameObject>();

    public bool IsTyping { get; private set; }
    public bool IsShowingChoices { get; private set; }

    // --- SHOW / HIDE --- //
    public void Show()
    {
        dialoguePanel.SetActive(true);
        choicesPanel.SetActive(false);
        continueIndicator.SetActive(false);
    }

    public void Hide()
    {
        dialoguePanel.SetActive(false);
        choicesPanel.SetActive(false);
        ClearChoices();
    }

    // --- MOSTRAR LINEA --- //
    public void ShowLine(string speakerName, string text, Sprite portrait)
    {
        // Limpiar choices anteriores
        ClearChoices();
        IsShowingChoices = false;
        continueIndicator.SetActive(false);

        // Nombre
        speakerNameText.text = speakerName;

        // Portrait
        if (portrait != null)
        {
            speakerPortrait.sprite = portrait;
            speakerPortrait.gameObject.SetActive(true);
        }
        else
        {
            speakerPortrait.gameObject.SetActive(false);
        }

        // Typewriter
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        fullText = text;
        typingCoroutine = StartCoroutine(TypewriterEffect(text));
    }

    private IEnumerator TypewriterEffect(string text)
    {
        IsTyping = true;
        dialogueText.text = "";

        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        IsTyping = false;
        continueIndicator.SetActive(true);
    }

    public void SkipTypewriter()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        dialogueText.text = fullText;
        IsTyping = false;
        continueIndicator.SetActive(true);
    }

    // --- CHOICES --- //
    public void ShowChoices(List<string> choiceTexts, Action<int> onChoiceSelected)
    {
        ClearChoices();
        IsShowingChoices = true;
        continueIndicator.SetActive(false);
        choicesPanel.SetActive(true);

        for (int i = 0; i < choiceTexts.Count; i++)
        {
            int index = i;
            GameObject btn = Instantiate(choiceButtonPrefab, choicesPanel.transform);

            TextMeshProUGUI btnText = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (btnText != null)
                btnText.text = choiceTexts[i];

            Button button = btn.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                int capturedIndex = index; // 
                button.onClick.AddListener(() =>
                {
                    Debug.Log("Choice clickeada: " + capturedIndex);
                    onChoiceSelected(capturedIndex);
                });
                button.interactable = true;
            }
        }

        // Seleccionar el primer boton para navegacion
        if (activeChoiceButtons.Count > 0)
        {
            Button firstButton = activeChoiceButtons[0].GetComponent<Button>();
            firstButton?.Select();
        }
    }

    private void ClearChoices()
    {
        foreach (var btn in activeChoiceButtons)
        {
            if (btn != null)
            {
                btn.GetComponent<Button>()?.onClick.RemoveAllListeners();
                Destroy(btn);
            }
        }

        activeChoiceButtons.Clear();

        // Destruir cualquier hijo que haya quedado 
        foreach (Transform child in choicesPanel.transform)
            Destroy(child.gameObject);

        choicesPanel.SetActive(false);
        IsShowingChoices = false;
    }

    public void HideChoices()
    {
        ClearChoices();
        IsShowingChoices = false;
    }
}