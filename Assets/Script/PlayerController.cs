using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 12f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Better Jump")]
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;

    [SerializeField] private int amountOfJumps = 1;
    private int jumpsRemaining;

    private PlayerControls playerControls;
    private Vector2 movement;
    private Rigidbody2D rigiBody;
    private bool isGrounded;
    private Animator playerAnimator;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        playerControls = new PlayerControls();
        rigiBody = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        playerControls.Movement.Jump.performed += OnJump;
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void Update()
    {
        PlayerInput();
        CheckIsGrounded();
        HandleVariableJump();
        UpdateAnimations();
        FlipSprite();
        ResetJumps();
    }

    private void FixedUpdate()
    {
        Move();
        HandleBetterFall();
    }

    private void PlayerInput()
    {
        movement = playerControls.Movement.Move.ReadValue<Vector2>();
    }

    private void CheckIsGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void Move()
    {
        rigiBody.linearVelocity = new Vector2 (movement.x * moveSpeed, rigiBody.linearVelocity.y);    
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (jumpsRemaining > 0)
        {
            if (!isGrounded)
            {
                playerAnimator.SetTrigger("doubleJump");
            }

            rigiBody.linearVelocity = new Vector2(rigiBody.linearVelocity.x, jumpForce);
            jumpsRemaining--;
        }
    }

    private void ResetJumps()
    {
        if (isGrounded)
        {
            jumpsRemaining = amountOfJumps;
        }
    }

    private void HandleBetterFall()
    {
        if (rigiBody.linearVelocity.y < 0)
        {
            rigiBody.linearVelocity += Vector2.up * Physics2D.gravity.y * fallMultiplier * Time.fixedDeltaTime;
        }
    }

    private void HandleVariableJump()
    {
        bool releasedJumpEarly = rigiBody.linearVelocity.y > 0 && !playerControls.Movement.Jump.IsPressed();

        if (releasedJumpEarly)
        {
            rigiBody.linearVelocity += Vector2.up * Physics2D.gravity.y * lowJumpMultiplier * Time.fixedDeltaTime;
        }
    }

    private void UpdateAnimations()
    {
        playerAnimator.SetFloat("moveX", movement.x);
        playerAnimator.SetBool("isGrounded", isGrounded);
    }

    private void FlipSprite()
    {
        if (movement.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (movement.x > 0)
        {
            spriteRenderer.flipX = false;
        }
    }
}
