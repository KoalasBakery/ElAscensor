using UnityEngine;
using UnityEngine.InputSystem;

/*
 * ---------------------------------------------------------------
 *                     PLAYER CONTROLLER
 * ---------------------------------------------------------------
 * DESCRIPCION:
 * Controla el movimiento del jugador con A/D y joystick virtual.
 *
 * DEPENDENCIAS:
 *   - Rigidbody2D
 *   - Animator (opcional)
 * ---------------------------------------------------------------
 */

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float jumpForce = 10f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.2f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private float horizontalInput;
    private bool isGrounded;
    public bool CanMove { get; private set; } = true;

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
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded && CanMove)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    // --- MOVIMIENTO --- //
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

        animator.SetFloat("Speed", Mathf.Abs(horizontalInput));
        animator.SetBool("IsGrounded", isGrounded);
    }

    public void SetMovementEnabled(bool enabled)
    {
        CanMove = enabled;
        if (!enabled)
        {
            horizontalInput = 0;
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }
}