using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

   [SerializeField] private PlayerInputActions inputActions;
    [SerializeField] PlayerInput actions;
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
        //inputActions = new PlayerInputActions();
        playerController = FindAnyObjectByType<PlayerController>();
    }

   /* private void OnEnable()
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
    }*/

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
        if (DialogueManager.Instance.IsDialogueActive && context.performed)
            DialogueManager.Instance.OnContinue();
    }
    
    //Esto es para los puzzles
    public void OnInteractPosition(InputAction.CallbackContext context)
    {
        if (PuzzleManager.Instance==null||!PuzzleManager.Instance.activePuzzle) return;

        if (context.canceled)
        {
            PuzzleManager.Instance.OnRelease();   
            return;
        }
        PuzzleManager.Instance.OnInteract(context);
    }

    public void OnInventory(InputAction.CallbackContext context)
    {
        if (inventoryUI == null && context.performed)
            inventoryUI = FindAnyObjectByType<InventoryUI>();
        inventoryUI?.ToggleInventory();
    }

    public void OnJournal(InputAction.CallbackContext context)
    {
        if (journalUI == null && context.performed)
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