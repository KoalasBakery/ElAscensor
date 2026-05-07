using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    private PlayerInputActions inputActions;
    private PlayerController playerController;
    private InteractionDetector interactionDetector;
    private InventoryUI inventoryUI;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        inputActions = new PlayerInputActions();
        playerController = FindAnyObjectByType<PlayerController>();
        interactionDetector = FindAnyObjectByType<InteractionDetector>();

    }

    private void OnEnable()
    {
        inputActions.Gameplay.Enable();

        inputActions.Gameplay.Move.performed += OnMove;
        inputActions.Gameplay.Move.canceled += OnMove;
        inputActions.Gameplay.Jump.performed += OnJump;
        inputActions.Gameplay.Interact.performed += OnInteract;
        inputActions.Gameplay.Inventory.performed += OnInventory;
    }

    private void OnDisable()
    {
        inputActions.Gameplay.Move.performed -= OnMove;
        inputActions.Gameplay.Move.canceled -= OnMove;
        inputActions.Gameplay.Jump.performed -= OnJump;
        inputActions.Gameplay.Interact.performed -= OnInteract;
        inputActions.Gameplay.Inventory.performed -= OnInventory;
        inputActions.Gameplay.Disable();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        if (playerController != null)
            playerController.OnMove(context);
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (playerController != null)
            playerController.OnJump(context);
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        interactionDetector?.TryInteract();
    }

    private void OnInventory(InputAction.CallbackContext context)
    {
        if (inventoryUI == null)
            inventoryUI = FindAnyObjectByType<InventoryUI>();

        inventoryUI?.ToggleInventory();
    }

    // --- CAMBIO DE ESQUEMAS DE CONTROLES --- //
    public void SwitchToUI()
    {
        // Solo desactivamos movimiento y salto
        inputActions.Gameplay.Move.Disable();
        inputActions.Gameplay.Jump.Disable();

        // Interact pasa a confirmar dialogo
        inputActions.Gameplay.Interact.performed -= OnInteract;
        inputActions.Gameplay.Interact.performed += OnUIConfirm;
    }

    public void SwitchToGameplay()
    {
        // Reactivamos movimiento y salto
        inputActions.Gameplay.Move.Enable();
        inputActions.Gameplay.Jump.Enable();

        // Interact regresa a interactuar
        inputActions.Gameplay.Interact.performed -= OnUIConfirm;
        inputActions.Gameplay.Interact.performed += OnInteract;
    }

    private void OnUIConfirm(InputAction.CallbackContext context)
    {
        if (DialogueManager.Instance.IsDialogueActive)
            DialogueManager.Instance.OnContinue();
    }
}