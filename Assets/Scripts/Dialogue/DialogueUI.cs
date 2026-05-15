using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static DialogueData;

/*
 * ---------------------------------------------------------------
 *                        DIALOGUE UI
 * ---------------------------------------------------------------
 * DESCRIPCION:
 * Maneja toda la parte visual del sistema de dialogos.
 * Separado del DialogueManager para respetar el principio
 * de responsabilidad unica (cada script hace una sola cosa).
 *
 * SETUP EN UNITY:
 *   Este script va en el Canvas (siempre activo).
 *   El DialoguePanel empieza DESACTIVADO en la Hierarchy.
 *   Asignar en el Inspector:
 *     · Dialogue Panel     -> DialoguePanel
 *     · Speaker Name Text  -> SpeakerName (TMP)
 *     · Dialogue Text      -> DialogueText (TMP)
 *     · Speaker Portrait   -> SpeakerPortrait (Image)
 *     · Continue Indicator -> ContinueIndicator
 *     · Choices Panel      -> ChoicesPanel
 *     · Choice Button Prefab -> Prefab con Button + TMP
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

    private DialogueTextEffects textEffects;
    private string fullText;
    private Coroutine typingCoroutine;
    private List<GameObject> activeChoiceButtons = new List<GameObject>();
    private TextEffect currentEffect = TextEffect.None;

    public bool IsTyping { get; private set; }
    public bool IsShowingChoices { get; private set; }

    private void Awake()
    {
        textEffects = dialogueText.GetComponent<DialogueTextEffects>();
        if (textEffects == null)
            textEffects = dialogueText.gameObject.AddComponent<DialogueTextEffects>();
    }

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
    public void ShowLine(string speakerName, string text, Sprite portrait,
                         TMP_FontAsset customFont = null,
                         Color? textColor = null,
                         float fontSize = 0,
                         TextEffect effect = TextEffect.None,
                         float customTypingSpeed = 0)
    {
        currentEffect = effect;
        ClearChoices();
        IsShowingChoices = false;
        continueIndicator.SetActive(false);

        // Nombre del speaker
        speakerNameText.text = speakerName;

        // Fuente — solo cambiar si hay una personalizada
        if (customFont != null)
            dialogueText.font = customFont;

        // Tamaño — solo cambiar si hay uno personalizado
        if (fontSize > 0)
            dialogueText.fontSize = fontSize;

        // Solo aplicar color si tiene alpha mayor a 0
        if (textColor.HasValue && textColor.Value.a > 0)
            dialogueText.color = textColor.Value;

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

        // Detener efecto anterior
        textEffects?.StopCurrentEffect();

        // Typewriter
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        float speed = customTypingSpeed > 0 ? customTypingSpeed : typingSpeed;
        fullText = text;
        typingCoroutine = StartCoroutine(TypewriterEffect(text, speed, effect));
    }

    private IEnumerator TypewriterEffect(string text, float speed, TextEffect effect)
    {
        IsTyping = true;
        dialogueText.text = "";

        if (effect == TextEffect.Wave || effect == TextEffect.Shake)
            textEffects?.PlayEffect(effect);

        for (int i = 0; i < text.Length; i++)
        {
            dialogueText.text += text[i];

            if (effect == TextEffect.FadeIn)
            {
                // Fadear la ultima letra agregada
                yield return StartCoroutine(textEffects.FadeInCharacter(i, speed * 3f));
            }
            else
            {
                yield return new WaitForSeconds(speed);
            }
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

        // Mantener el efecto activo
        if (currentEffect != TextEffect.None)
            textEffects?.PlayEffect(currentEffect);
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
            int capturedIndex = i;
            GameObject btn = Instantiate(choiceButtonPrefab, choicesPanel.transform);
            activeChoiceButtons.Add(btn);

            TextMeshProUGUI btnText = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (btnText != null)
                btnText.text = choiceTexts[i];

            Button button = btn.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => onChoiceSelected(capturedIndex));
                button.interactable = true;
            }
        }

        // Seleccionar el primer boton
        if (activeChoiceButtons.Count > 0)
            activeChoiceButtons[0].GetComponent<Button>()?.Select();
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