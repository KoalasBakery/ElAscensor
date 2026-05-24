using UnityEngine;
using UnityEngine.InputSystem;

/*
 * ---------------------------------------------------------------
 *                     PLAYER CONTROLLER
 * ---------------------------------------------------------------
 * DESCRIPCION:
 * Controla el movimiento del jugador.
 * Soporta caminar, correr y agacharse.
 *
 * CONTROLES:
 *   A/D        -> caminar
 *   Shift + AD -> correr
 *   Ctrl       -> agacharse (puede caminar agachado)
 *   Movil      -> stick al 80%+ para correr
 *
 * DEPENDENCIAS:
 *   - Rigidbody2D
 *   - CapsuleCollider2D
 *   - Animator (opcional)
 * ---------------------------------------------------------------
 */

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float runMultiplier = 1.8f;
    [SerializeField] private float crouchMultiplier = 0.5f;
    [SerializeField] private float mobileRunThreshold = 0.8f;

    [Header("Crouch Settings")]
    [SerializeField] private float normalHeight = 1f;
    [SerializeField] private float crouchHeight = 0.5f;
    [SerializeField] private Vector2 normalOffset = new Vector2(0f, 0f);
    [SerializeField] private Vector2 crouchOffset = new Vector2(0f, -0.25f);

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.2f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private CapsuleCollider2D capsuleCollider;

    private float horizontalInput;
    private bool isRunning;
    private bool isCrouching;
    private bool isGrounded;
    private bool isRunningByKey = false;
    private bool isRunningByStick = false;

    public bool CanMove { get; private set; } = true;
    public bool IsCrouching => isCrouching;
    public bool IsRunning => isRunning;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    private void Update()
    {
        CheckGround();
        HandleAnimations();
    }

    private void FixedUpdate()
    {
        if (!CanMove) return;
        Move();
    }

    // --- INPUT --- //
    public void OnMove(InputAction.CallbackContext context)
    {
        horizontalInput = context.ReadValue<Vector2>().x;

        // Solo detectar correr por stick si el input viene de gamepad
        // El teclado siempre da 1.0 exacto, el stick da valores entre 0 y 1
        float magnitude = Mathf.Abs(horizontalInput);
        bool isGamepad = context.control.device is
            UnityEngine.InputSystem.Gamepad;

        if (isGamepad)
            isRunningByStick = magnitude >= mobileRunThreshold;
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.performed) isRunningByKey = true;
        if (context.canceled) isRunningByKey = false;
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.performed) StartCrouch();
        if (context.canceled) StopCrouch();
    }

    // --- MOVIMIENTO --- //
    private void Move()
    {
        float currentSpeed = moveSpeed;

        if (isCrouching)
            currentSpeed *= crouchMultiplier;
        else if (IsRunning)
            currentSpeed *= runMultiplier;

        float targetVelocityX = horizontalInput * currentSpeed;
        float newVelocityX = Mathf.MoveTowards(
            rb.linearVelocity.x,
            targetVelocityX,
            currentSpeed * Time.fixedDeltaTime * 10f);

        rb.linearVelocity = new Vector2(newVelocityX, rb.linearVelocity.y);
        FlipSprite(horizontalInput);
    }

    // --- AGACHARSE --- //
    private void StartCrouch()
    {
        if (isCrouching) return;
        isCrouching = true;

        if (capsuleCollider != null)
        {
            capsuleCollider.size = new Vector2(
                capsuleCollider.size.x, crouchHeight);
            capsuleCollider.offset = crouchOffset;
        }
    }

    private void StopCrouch()
    {
        if (!isCrouching) return;
        isCrouching = false;

        if (capsuleCollider != null)
        {
            capsuleCollider.size = new Vector2(
                capsuleCollider.size.x, normalHeight);
            capsuleCollider.offset = normalOffset;
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

        float speed = Mathf.Abs(rb.linearVelocity.x);

        animator.SetFloat("Speed", speed);
        animator.SetBool("IsRunning", IsRunning && speed > 0.1f);
        animator.SetBool("IsCrouching", isCrouching);
        animator.SetBool("IsGrounded", isGrounded);
    }

    public void SetMovementEnabled(bool enabled)
    {
        CanMove = enabled;
        if (!enabled)
        {
            horizontalInput = 0;
            isRunning = false;
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }
}