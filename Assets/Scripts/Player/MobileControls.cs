using UnityEngine;
using UnityEngine.UI;

/*
 * ---------------------------------------------------------------
 *                      MOBILE CONTROLS
 * ---------------------------------------------------------------
 * DESCRIPCION:
 * Maneja los controles tactiles para movil.
 * Solo se muestran en iOS/Android.
 * Conecta los botones con el InputManager.
 * ---------------------------------------------------------------
 */

public class MobileControls : MonoBehaviour
{
    [Header("Botones")]
    [SerializeField] private Button interactButton;
    [SerializeField] private Button inventoryButton;
    [SerializeField] private Button journalButton;

    private void Start()
    {
        // Solo mostrar en movil lo comento por que no ando en movil ahorita
        /*
        #if UNITY_IOS || UNITY_ANDROID
            gameObject.SetActive(true);
        #else
            gameObject.SetActive(false);
            return;
        #endif
        */
        // Conectar botones
        interactButton?.onClick.AddListener(OnInteractPressed);
        inventoryButton?.onClick.AddListener(OnInventoryPressed);
        journalButton?.onClick.AddListener(OnJournalPressed);
    }

    private void OnInteractPressed()
    {
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
        {
            DialogueManager.Instance.OnContinue();
            return;
        }

        InteractionDetector detector = FindAnyObjectByType<InteractionDetector>();
        detector?.TryInteract();
    }

    private void OnInventoryPressed()
    {
        InventoryUI inventoryUI = FindAnyObjectByType<InventoryUI>();
        inventoryUI?.ToggleInventory();
    }

    private void OnJournalPressed()
    {
        JournalUI journalUI = FindAnyObjectByType<JournalUI>();
        journalUI?.ToggleJournal();
    }
}