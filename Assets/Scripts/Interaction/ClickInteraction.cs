using UnityEngine;
using UnityEngine.InputSystem;

/*
 * ---------------------------------------------------------------
 *                     CLICK INTERACTION
 * ---------------------------------------------------------------
 * DESCRIPCION:
 * Maneja el sistema de Point and Click.
 * Detecta clicks del mouse en la pantalla, convierte la posicion
 * a coordenadas del mundo y determina si se clickeo un objeto
 * interactuable o simplemente el suelo para caminar.
 *
 * FUNCIONAMIENTO:
 *   1. Jugador hace click en pantalla
 *   2. Se lanza un Raycast desde la camara
 *   3. Si golpea un Interactable -> camina hacia el y luego interactua
 *   4. Si golpea el suelo -> camina hacia ese punto
 *
 * DEPENDENCIAS:
 *   - PlayerController
 *   - Camera (Main Camera)
 *   - Interactable (en objetos del mundo)
 *
 * SETUP EN UNITY:
 *   1. Agregar este script al Player
 *   2. Asegurense que los objetos interactuables tengan
 *      Collider2D con IsTrigger = true y Layer = Interactable
 *   3. El suelo debe tener Collider2D con Layer = Ground
 * ---------------------------------------------------------------
 */

public class ClickInteraction : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private LayerMask walkableLayer;
    [SerializeField] private float clickRange = 20f;

    [Header("Cursor")]
    [SerializeField] private Texture2D defaultCursor;
    [SerializeField] private Texture2D interactCursor;
    // TODO: Agregar cursor personalizado del juego cuando este definido

    private PlayerController playerController;
    private Camera mainCamera;
    private PlayerInputActions inputActions;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        mainCamera = Camera.main;
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Gameplay.Enable();
        inputActions.Gameplay.Interact.performed += OnClick;
    }

    private void OnDisable()
    {
        inputActions.Gameplay.Interact.performed -= OnClick;
        inputActions.Gameplay.Disable();
    }

    private void Update()
    {
        UpdateCursor();
    }

    // --- CLICK --- //
    private void OnClick(InputAction.CallbackContext context)
    {
        if (!playerController.CanMove) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector2 worldPos = mainCamera.ScreenToWorldPoint(mousePos);

        // Primero checar si clickeo un interactuable
        Collider2D interactableHit = Physics2D.OverlapPoint(worldPos, interactableLayer);
        if (interactableHit != null)
        {
            Interactable interactable = interactableHit.GetComponent<Interactable>();
            if (interactable != null && interactable.canInteract)
            {
                playerController.SetMoveTarget(
                    interactableHit.transform.position, interactable);
                return;
            }
        }

        // Checar si clickeo area caminable
        Collider2D walkableHit = Physics2D.OverlapPoint(worldPos, walkableLayer);
        if (walkableHit != null)
        {
            playerController.SetMoveTarget(worldPos);
            return;
        }

        // Si no clickeo ni interactuable ni area caminable, ignorar el click
        Debug.Log("Click fuera del area caminable, ignorando");
    }

    // --- CURSOR --- //
    private void UpdateCursor()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector2 worldPos = mainCamera.ScreenToWorldPoint(mousePos);

        Collider2D hit = Physics2D.OverlapPoint(worldPos, interactableLayer);

        if (hit != null && hit.GetComponent<Interactable>() != null)
        {
            // Cursor de interaccion
            if (interactCursor != null)
                Cursor.SetCursor(interactCursor, Vector2.zero, CursorMode.Auto);
        }
        else
        {
            // Cursor normal
            if (defaultCursor != null)
                Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
            else
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }
}