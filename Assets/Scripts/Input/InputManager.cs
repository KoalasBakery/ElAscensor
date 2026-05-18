using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [SerializeField] private PlayerInput playerInput;
    private PlayerController playerController;
    private InventoryUI inventoryUI;
    private JournalUI journalUI;
    private InteractionDetector interactionDetector;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        playerController = FindAnyObjectByType<PlayerController>();
        interactionDetector = FindAnyObjectByType<InteractionDetector>();
    }

    // --- GAMEPLAY --- //
    public void OnMove(InputAction.CallbackContext context)
    {
        if (playerController != null)
            playerController.OnMove(context);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (playerController != null && context.performed)
            playerController.OnJump(context);
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
        {
            DialogueManager.Instance.OnContinue();
            return;
        }

        interactionDetector?.TryInteract();
    }

    public void OnInteractPosition(InputAction.CallbackContext context)
    {
        if (PuzzleManager.Instance == null || !PuzzleManager.Instance.activePuzzle) return;

        if (context.canceled)
        {
            PuzzleManager.Instance.OnRelease();
            return;
        }
        PuzzleManager.Instance.OnInteract(context);
    }

    public void OnInventory(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (inventoryUI == null)
            inventoryUI = FindAnyObjectByType<InventoryUI>();
        inventoryUI?.ToggleInventory();
    }

    public void OnJournal(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (journalUI == null)
            journalUI = FindAnyObjectByType<JournalUI>();
        journalUI?.ToggleJournal();
    }

    // --- CAMBIO DE ESQUEMAS --- //
    public void SwitchToUI()
    {
        // Solo bloqueamos movimiento y salto
        // Los clicks en UI siempre funcionan
        // E sigue funcionando para avanzar dialogo
        playerController?.SetMovementEnabled(false);
    }

    public void SwitchToGameplay()
    {
        playerController?.SetMovementEnabled(true);
    }
}