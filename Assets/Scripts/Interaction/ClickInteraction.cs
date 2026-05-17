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
 * interactuable o simplemente el suelo/pared para caminar.
 *
 * FUNCIONAMIENTO:
 *   1. Jugador hace click en pantalla
 *   2. Se lanza un Raycast desde la camara
 *   3. Si golpea un Interactable -> camina hacia el y luego interactua
 *   4. Si golpea el suelo/pared(con Layer Clickable) -> camina hacia ese punto
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
 *   4. Crear un ClickableArea con Layer = Clickable que cubra
 *      toda el area de la habitacion
 * ---------------------------------------------------------------
 */

public class ClickInteraction : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private LayerMask walkableLayer;
    [SerializeField] private LayerMask clickableLayer;
    //[SerializeField] private float clickRange = 20f;

    [Header("Cursor")]
    [SerializeField] private Texture2D defaultCursor;
    [SerializeField] private Texture2D interactCursor;

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

        // Checar si clickeo dentro del area clickeable
        Collider2D clickableHit = Physics2D.OverlapPoint(worldPos, clickableLayer);
        if (clickableHit != null)
        {
            Vector2 playerPos = playerController.transform.position;
            Vector2 targetOnFloor = new Vector2(worldPos.x, playerPos.y);

            // Buscar suelo hacia abajo desde el punto objetivo
            RaycastHit2D groundCheck = Physics2D.Raycast(
                targetOnFloor, Vector2.down, 2f, walkableLayer);

            if (groundCheck.collider != null)
            {
                // Hay suelo debajo, caminar directo
                playerController.SetMoveTarget(new Vector2(targetOnFloor.x, playerPos.y));
            }
            else
            {
                // No hay suelo en ese X, buscar el borde mas cercano
                Vector2 direction = new Vector2(worldPos.x - playerPos.x, 0).normalized;
                float maxDistance = Mathf.Abs(worldPos.x - playerPos.x);

                RaycastHit2D hit = Physics2D.Raycast(
                    playerPos, direction, maxDistance, walkableLayer);

                if (hit.collider != null)
                    playerController.SetMoveTarget(new Vector2(hit.point.x, playerPos.y));
                else
                    Debug.Log("No hay suelo en esa direccion");
            }
            return;
        }

        // Si clickeo fuera del area clickeable, ignorar
        Debug.Log("Click fuera del area, tonto");
    }

    // --- CURSOR --- //
    private void UpdateCursor()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector2 worldPos = mainCamera.ScreenToWorldPoint(mousePos);

        Collider2D hit = Physics2D.OverlapPoint(worldPos, interactableLayer);

        if (hit != null && hit.GetComponent<Interactable>() != null)
        {
            if (interactCursor != null)
                Cursor.SetCursor(interactCursor, Vector2.zero, CursorMode.Auto);
        }
        else
        {
            if (defaultCursor != null)
                Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
            else
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }
}