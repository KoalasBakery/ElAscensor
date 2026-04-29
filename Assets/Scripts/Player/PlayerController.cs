using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float jumpForce = 10f; //Ni idea si se usara de algo, pero pues si no lo quito

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.2f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private float horizontalInput;
    private bool isGrounded;
    private bool canMove = true; // Esto para bloquear movimiento en diálogos/UI

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        CheckGround();
        HandleAnimations();
        FlipSprite();
    }

    void FixedUpdate()
    {
        if (canMove)
            Move();
    }

    // --- INPUT InputManager) --- //

    public void OnMove(InputAction.CallbackContext context)
    {
        horizontalInput = context.ReadValue<Vector2>().x;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded && canMove)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    // --- MOVIMIENTO --- //
    private void Move()
    {
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }

    // --- SUELO --- //
    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    // --- FLIP DEL SPRITE --- //
    private void FlipSprite()
    {
        if (horizontalInput > 0)
            spriteRenderer.flipX = false;
        else if (horizontalInput < 0)
            spriteRenderer.flipX = true;
    }

    // --- ANIMACIONES --- //
    private void HandleAnimations()
    {
        if (animator == null) return;

        if (animator.runtimeAnimatorController == null) return;// por que aun no tengo animaciones

        animator.SetFloat("Speed", Mathf.Abs(horizontalInput));
        animator.SetBool("IsGrounded", isGrounded);
    }

    // --- BLOQUEO DE MOVIMIENTO --- //
    public void SetMovementEnabled(bool enabled)
    {
        canMove = enabled;
        if (!enabled)
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }
}