using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization.Settings;

/*
 * ---------------------------------------------------------------
 *                        JOURNAL UI
 * ---------------------------------------------------------------
 * DESCRIPCION:
 * UI del diario del jugador con dos pestanas:
 *   - Misiones: activas y completadas
 *   - Notas: documentos encontrados
 *
 * SETUP EN UNITY:
 *   Este script va en el Canvas.
 *   Asignar en el Inspector todos los campos.
 * ---------------------------------------------------------------
 */

public class JournalUI : MonoBehaviour
{
    public static JournalUI Instance { get; private set; }

    [Header("Panel Principal")]
    [SerializeField] private GameObject journalPanel;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Pestanas")]
    [SerializeField] private Button missionsTabButton;
    [SerializeField] private Button notesTabButton;
    [SerializeField] private GameObject missionsPanel;
    [SerializeField] private GameObject notesPanel;

    [Header("Misiones")]
    [SerializeField] private Transform activeMissionsContainer;
    [SerializeField] private Transform completedMissionsContainer;
    [SerializeField] private GameObject missionEntryPrefab;

    [Header("Notas")]
    [SerializeField] private Transform notesContainer;
    [SerializeField] private GameObject noteEntryPrefab;

    [Header("Vista de detalle")]
    [SerializeField] private GameObject detailPanel;
    [SerializeField] private TextMeshProUGUI detailTitle;
    [SerializeField] private TextMeshProUGUI detailContent;
    [SerializeField] private Image detailImage;
    [SerializeField] private Button closeDetailButton;

    [Header("Animacion")]
    [SerializeField] private float animSpeed = 6f;

    private bool isOpen = false;
    private bool isAnimating = false;
    private List<NoteData> collectedNotes = new List<NoteData>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (canvasGroup == null)
            canvasGroup = journalPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = journalPanel.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        journalPanel.SetActive(false);

        if (detailPanel != null)
            detailPanel.SetActive(false);

        missionsTabButton?.onClick.AddListener(() => ShowTab(true));
        notesTabButton?.onClick.AddListener(() => ShowTab(false));

        if (closeDetailButton != null)
            closeDetailButton.onClick.AddListener(CloseDetail);

        MissionManager.Instance.onMissionUnlocked.AddListener(OnMissionUnlocked);
        MissionManager.Instance.onMissionCompleted.AddListener(OnMissionCompleted);
        MissionManager.Instance.onStepCompleted.AddListener(OnStepCompleted);

        ShowTab(true);
    }

    // --- TOGGLE --- //
    public void ToggleJournal()
    {
        // Si esta animando ignorar el input
        if (isAnimating) return;

        isOpen = !isOpen;

        if (isOpen)
        {
            RefreshMissions();
            RefreshNotes();
        }

        StartCoroutine(AnimateJournal(isOpen));
    }

    private IEnumerator AnimateJournal(bool open)
    {
        isAnimating = true;
        journalPanel.SetActive(true);
        canvasGroup.blocksRaycasts = false; // desactivar mientras anima

        float targetAlpha = open ? 1f : 0f;
        float currentAlpha = canvasGroup.alpha;

        while (Mathf.Abs(currentAlpha - targetAlpha) > 0.01f)
        {
            currentAlpha = Mathf.Lerp(currentAlpha, targetAlpha,
                Time.deltaTime * animSpeed);
            canvasGroup.alpha = currentAlpha;
            yield return null;
        }

        // Asegurar valor final exacto
        canvasGroup.alpha = targetAlpha;
        canvasGroup.blocksRaycasts = open;

        if (!open)
            journalPanel.SetActive(false);

        isAnimating = false;
    }

    // --- PESTANAS --- //
    private void ShowTab(bool showMissions)
    {
        if (missionsPanel != null) missionsPanel.SetActive(showMissions);
        if (notesPanel != null) notesPanel.SetActive(!showMissions);
    }

    // --- MISIONES --- //
    private void RefreshMissions()
    {
        ClearContainer(activeMissionsContainer);
        ClearContainer(completedMissionsContainer);

        foreach (var mission in MissionManager.Instance.GetActiveMissions())
            CreateMissionEntry(mission, activeMissionsContainer, false);

        foreach (var mission in MissionManager.Instance.GetCompletedMissions())
            CreateMissionEntry(mission, completedMissionsContainer, true);
    }

    private void CreateMissionEntry(MissionData mission, Transform container, bool completed)
    {
        if (missionEntryPrefab == null || container == null) return;

        GameObject entry = Instantiate(missionEntryPrefab, container);

        TextMeshProUGUI titleText = entry.transform.Find("Title")?.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI stepsText = entry.transform.Find("Steps")?.GetComponent<TextMeshProUGUI>();

        // Titulo localizado
        if (titleText != null)
            StartCoroutine(SetLocalizedText(titleText, mission.titleKey));

        // Pasos localizados
        if (stepsText != null)
            StartCoroutine(SetStepsText(stepsText, mission));

        // Color si completada
        if (completed && titleText != null)
            titleText.color = Color.gray;

        // Click para ver detalle
        Button btn = entry.GetComponent<Button>();
        if (btn != null)
        {
            MissionData capturedMission = mission;
            btn.onClick.AddListener(() => ShowMissionDetail(capturedMission));
        }
    }

    private IEnumerator SetStepsText(TextMeshProUGUI text, MissionData mission)
    {
        string steps = "";

        foreach (var step in mission.steps)
        {
            string stepText = step.stepDescriptionKey;

            if (!string.IsNullOrEmpty(step.stepDescriptionKey))
            {
                var op = LocalizationSettings.StringDatabase
                    .GetLocalizedStringAsync("Story", step.stepDescriptionKey);
                yield return op;
                stepText = op.Result;
            }

            steps += (step.isCompleted ? "[OK] " : "[ ] ") + stepText + "\n";
        }

        if (text != null)
            text.text = steps;
    }

    private void ShowMissionDetail(MissionData mission)
    {
        if (detailPanel == null) return;

        detailPanel.SetActive(true);

        if (detailTitle != null)
            StartCoroutine(SetLocalizedText(detailTitle, mission.titleKey));

        if (detailContent != null)
            StartCoroutine(SetLocalizedText(detailContent, mission.descriptionKey));

        if (detailImage != null)
            detailImage.gameObject.SetActive(false);
    }

    // --- NOTAS --- //
    public void AddNote(NoteData note)
    {
        if (collectedNotes.Contains(note)) return;

        collectedNotes.Add(note);
        Debug.Log($"Nota agregada al journal: {note.noteTitle}");
    }

    private void RefreshNotes()
    {
        ClearContainer(notesContainer);

        foreach (var note in collectedNotes)
            CreateNoteEntry(note);
    }

    private void CreateNoteEntry(NoteData note)
    {
        if (noteEntryPrefab == null || notesContainer == null) return;

        GameObject entry = Instantiate(noteEntryPrefab, notesContainer);

        TextMeshProUGUI titleText = entry.transform.Find("Title")?.GetComponent<TextMeshProUGUI>();
        if (titleText != null)
            titleText.text = note.noteTitle;

        Button btn = entry.GetComponent<Button>();
        if (btn != null)
        {
            NoteData capturedNote = note;
            btn.onClick.AddListener(() => ShowNoteDetail(capturedNote));
        }
    }

    private void ShowNoteDetail(NoteData note)
    {
        if (detailPanel == null) return;

        detailPanel.SetActive(true);

        if (detailTitle != null)
            detailTitle.text = note.noteTitle;

        if (detailContent != null)
            StartCoroutine(SetLocalizedText(detailContent, note.contentKey));

        if (detailImage != null)
        {
            if (note.noteImage != null)
            {
                detailImage.sprite = note.noteImage;
                detailImage.gameObject.SetActive(true);
            }
            else
            {
                detailImage.gameObject.SetActive(false);
            }
        }
    }

    private void CloseDetail()
    {
        if (detailPanel != null)
            detailPanel.SetActive(false);
    }

    // --- EVENTOS --- //
    private void OnMissionUnlocked(MissionData mission)
    {
        if (isOpen) RefreshMissions();
    }

    private void OnMissionCompleted(MissionData mission)
    {
        if (isOpen) RefreshMissions();
    }

    private void OnStepCompleted(MissionData mission, int stepIndex)
    {
        if (isOpen) RefreshMissions();
    }

    // --- HELPERS --- //
    private void ClearContainer(Transform container)
    {
        if (container == null) return;
        foreach (Transform child in container)
            Destroy(child.gameObject);
    }

    private IEnumerator SetLocalizedText(TextMeshProUGUI text, string key)
    {
        if (string.IsNullOrEmpty(key)) yield break;

        var op = LocalizationSettings.StringDatabase.GetLocalizedStringAsync("Story", key);
        yield return op;

        if (text != null)
            text.text = op.Result;
    }

    public bool IsOpen => isOpen;
}