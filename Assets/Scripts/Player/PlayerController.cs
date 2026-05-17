using UnityEngine;
using UnityEngine.InputSystem;

/*
 * ---------------------------------------------------------------
 *                     PLAYER CONTROLLER
 * ---------------------------------------------------------------
 * DESCRIPCION:
 * Controla el movimiento del jugador.
 * Soporta dos modos:
 *   1. Movimiento por input (teclado/gamepad)
 *   2. Movimiento por destino (point and click)
 *
 * DEPENDENCIAS:
 *   - Rigidbody2D
 *   - Animator (Ahorita no eh puesto ni verga)
 *   - ClickInteraction (para point and click)
 * ---------------------------------------------------------------
 */

public class PlayerController : MonoBehaviour
{
    [Header("Point and Click")]
    [SerializeField] private float moveTimeout = 3f; // segundos antes de cancelar por si se bugeara o algo
    private float moveTimer = 0f;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float interactionRange = 1f; // Rango para interactuar

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.2f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // Input normal
    private float horizontalInput;

    // Point and click
    private Vector2 moveTarget;
    private bool hasTarget = false;
    private Interactable targetInteractable;

    private bool isGrounded;
    public bool CanMove { get; private set; } = true;

    // Events
    public System.Action OnReachedTarget;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        CheckGround();
        HandleAnimations();
        CheckTargetReached();
    }

    private void FixedUpdate()
    {
        if (!CanMove) return;

        if (hasTarget)
        {
            moveTimer += Time.fixedDeltaTime;

            // Si lleva demasiado tiempo sin llegar, cancelar
            if (moveTimer >= moveTimeout)
            {
                Debug.Log("Movimiento cancelado por timeout");
                ClearTarget();
                return;
            }

            MoveToTarget();
        }
        else
        {
            Move();
        }
    }

    // --- INPUT NORMAL --- //
    public void OnMove(InputAction.CallbackContext context)
    {
        // Solo si no hay donde hacer click
        if (!hasTarget)
            horizontalInput = context.ReadValue<Vector2>().x;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded && CanMove)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    // --- MOVIMIENTO NORMAL --- //
    private void Move()
    {
        float targetVelocityX = horizontalInput * moveSpeed;
        float newVelocityX = Mathf.MoveTowards(
            rb.linearVelocity.x,
            targetVelocityX,
            moveSpeed * Time.fixedDeltaTime * 10f);

        rb.linearVelocity = new Vector2(newVelocityX, rb.linearVelocity.y);
        FlipSprite(horizontalInput);
    }

    // --- POINT AND CLICK --- //
    public void SetMoveTarget(Vector2 target, Interactable interactable = null)
    {
        moveTarget = target;
        hasTarget = true;
        targetInteractable = interactable;
        horizontalInput = 0;
        moveTimer = 0f; // reset timer
    }

    public void ClearTarget()
    {
        hasTarget = false;
        targetInteractable = null;
        moveTimer = 0f;
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }

    private void MoveToTarget()
    {
        float direction = moveTarget.x - transform.position.x;
        float distance = Mathf.Abs(direction);

        if (distance <= interactionRange)
        {
            ClearTarget();
            OnReachedTarget?.Invoke();
            return;
        }

        // Usar MoveTowards para movimiento mas suave (se ve mejor, pero por mi pixel art a veces se siente raro)
        float targetVelocityX = Mathf.Sign(direction) * moveSpeed;
        float newVelocityX = Mathf.MoveTowards(
            rb.linearVelocity.x,
            targetVelocityX,
            moveSpeed * Time.fixedDeltaTime * 10f);

        rb.linearVelocity = new Vector2(newVelocityX, rb.linearVelocity.y);
        FlipSprite(direction);
    }

    private void CheckTargetReached()
    {
        if (!hasTarget || targetInteractable == null) return;

        float distance = Mathf.Abs(
            targetInteractable.transform.position.x - transform.position.x);

        if (distance <= interactionRange)
        {
            Interactable toInteract = targetInteractable;
            ClearTarget();
            toInteract.Interact();
        }
    }

    // --- HELPERS --- //
    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void FlipSprite(float direction)
    {
        if (spriteRenderer == null) return;
        if (direction > 0) spriteRenderer.flipX = false;
        else if (direction < 0) spriteRenderer.flipX = true;
    }

    private void HandleAnimations()
    {
        if (animator == null) return;
        if (animator.runtimeAnimatorController == null) return;

        float speed = hasTarget ?
            Mathf.Abs(rb.linearVelocity.x) :
            Mathf.Abs(horizontalInput);

        animator.SetFloat("Speed", speed);
        animator.SetBool("IsGrounded", isGrounded);
    }

    public void SetMovementEnabled(bool enabled)
    {
        CanMove = enabled;
        if (!enabled)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            ClearTarget();
        }
    }
}