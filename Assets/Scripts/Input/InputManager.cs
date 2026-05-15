using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private PlayerInputActions inputActions;
    private PlayerController playerController;
    private InventoryUI inventoryUI;
    private JournalUI journalUI;

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
    }

    private void OnEnable()
    {
        inputActions.Gameplay.Enable();

        inputActions.Gameplay.Move.performed += OnMove;
        inputActions.Gameplay.Move.canceled += OnMove;
        inputActions.Gameplay.Jump.performed += OnJump;
        inputActions.Gameplay.Interact.performed += OnInteract;
        inputActions.Gameplay.Inventory.performed += OnInventory;
        inputActions.Gameplay.Journal.performed += OnJournal;
    }

    private void OnDisable()
    {
        inputActions.Gameplay.Move.performed -= OnMove;
        inputActions.Gameplay.Move.canceled -= OnMove;
        inputActions.Gameplay.Jump.performed -= OnJump;
        inputActions.Gameplay.Interact.performed -= OnInteract;
        inputActions.Gameplay.Inventory.performed -= OnInventory;
        inputActions.Gameplay.Journal.performed -= OnJournal;
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
        if (DialogueManager.Instance.IsDialogueActive)
            DialogueManager.Instance.OnContinue();
    }

    private void OnInventory(InputAction.CallbackContext context)
    {
        if (inventoryUI == null)
            inventoryUI = FindAnyObjectByType<InventoryUI>();
        inventoryUI?.ToggleInventory();
    }

    private void OnJournal(InputAction.CallbackContext context)
    {
        if (journalUI == null)
            journalUI = FindAnyObjectByType<JournalUI>();
        journalUI?.ToggleJournal();
    }

    public void SwitchToUI()
    {
        inputActions.Gameplay.Move.Disable();
        inputActions.Gameplay.Jump.Disable();
    }

    public void SwitchToGameplay()
    {
        inputActions.Gameplay.Move.Enable();
        inputActions.Gameplay.Jump.Enable();
    }
}