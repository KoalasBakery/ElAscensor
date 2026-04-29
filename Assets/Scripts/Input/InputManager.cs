using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private PlayerInputActions inputActions;
    private PlayerController playerController;
    private InteractionDetector interactionDetector;

    private void Awake()
    {
        // Singleton
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
    }

    private void OnDisable()
    {
        inputActions.Gameplay.Move.performed -= OnMove;
        inputActions.Gameplay.Move.canceled -= OnMove;
        inputActions.Gameplay.Jump.performed -= OnJump;
        inputActions.Gameplay.Interact.performed -= OnInteract;

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

    // --- CAMBIO DE ESQUEMAS --- //
    public void SwitchToGameplay()
    {
        inputActions.Gameplay.Enable();
    }

    public void SwitchToUI()
    {
        inputActions.Gameplay.Disable();
    }
}