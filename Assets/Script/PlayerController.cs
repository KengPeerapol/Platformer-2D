using System.Collections;
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

    [Header("Dash System")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;

    [Header("Dash Visuals (No Animation)")]
    [SerializeField] private TrailRenderer dashTrail;
    [SerializeField] private Color dashColor = new Color(1f, 1f, 1f, 0.5f);
    private Color originalColor;

    [Header("Camera Look System")]
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private float lookDownOffset = -4f;
    [SerializeField] private float lookSpeed = 5f;
    [SerializeField] private float timeToLook = 0.5f;

    [Header("Spawn System")]
    private Vector3 currentSpawnPosition;

    private float lookTimer;
    private float defaultCameraY;

    private bool canDash = true;
    private bool isDashing;
    private float dashDir;

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

        currentSpawnPosition = transform.position;

        originalColor = spriteRenderer.color;

        if (dashTrail != null)
        {
            dashTrail.emitting = false;
        }

        if (cameraTarget != null)
        {
            defaultCameraY = cameraTarget.localPosition.y;
        }

        playerControls.Movement.Jump.performed += OnJump;
        playerControls.Movement.Dash.performed += OnDash;
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
        if (isDashing)
        {
            return;
        }

        PlayerInput();
        CheckIsGrounded();
        HandleVariableJump();
        UpdateAnimations();
        FlipSprite();
        ResetJumps();
        HandleCameraLook();

        // เรียกใช้งานระบบเสียงจาก SoundManager
        HandleFootstepsSound();
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            rigiBody.linearVelocity = new Vector2(dashDir * dashSpeed, 0f);
            return;
        }

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
        rigiBody.linearVelocity = new Vector2(movement.x * moveSpeed, rigiBody.linearVelocity.y);
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

            // สั่งเล่นเสียงกระโดดผ่าน SoundManager
            if (SoundManager.Instance != null) SoundManager.Instance.PlayJump();
        }
    }

    private void OnDash(InputAction.CallbackContext context)
    {
        if (canDash)
        {
            StartCoroutine(DashRoutine());
        }
    }

    private IEnumerator DashRoutine()
    {
        canDash = false;
        isDashing = true;

        // สั่งหยุดเสียงวิ่งผ่าน SoundManager ตอนเริ่ม Dash
        if (SoundManager.Instance != null) SoundManager.Instance.StopRun();

        float originalGravity = rigiBody.gravityScale;
        rigiBody.gravityScale = 0f;

        dashDir = movement.x != 0 ? Mathf.Sign(movement.x) : (spriteRenderer.flipX ? -1f : 1f);

        if (dashTrail != null) dashTrail.emitting = true;
        spriteRenderer.color = dashColor;

        // สั่งเล่นเสียง Dash ผ่าน SoundManager
        if (SoundManager.Instance != null) SoundManager.Instance.PlayDash();

        yield return new WaitForSeconds(dashDuration);

        if (dashTrail != null) dashTrail.emitting = false;
        spriteRenderer.color = originalColor;

        rigiBody.gravityScale = originalGravity;
        rigiBody.linearVelocity = new Vector2(0f, rigiBody.linearVelocity.y);
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
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

    private void HandleCameraLook()
    {
        if (cameraTarget == null) return;

        if (movement.y < -0.5f && Mathf.Abs(movement.x) < 0.1f && isGrounded)
        {
            lookTimer += Time.deltaTime;

            if (lookTimer >= timeToLook)
            {
                Vector3 targetPos = new Vector3(cameraTarget.localPosition.x, defaultCameraY + lookDownOffset, cameraTarget.localPosition.z);
                cameraTarget.localPosition = Vector3.Lerp(cameraTarget.localPosition, targetPos, Time.deltaTime * lookSpeed);
            }
        }
        else
        {
            lookTimer = 0f;
            Vector3 targetPos = new Vector3(cameraTarget.localPosition.x, defaultCameraY, cameraTarget.localPosition.z);
            cameraTarget.localPosition = Vector3.Lerp(cameraTarget.localPosition, targetPos, Time.deltaTime * lookSpeed);
        }
    }

    private void HandleFootstepsSound()
    {
        if (SoundManager.Instance == null) return;

        if (isGrounded && Mathf.Abs(movement.x) > 0.1f)
        {
            SoundManager.Instance.PlayRun();
        }
        else
        {
            SoundManager.Instance.StopRun();
        }
    }

    public void SetSpawnPosition(Vector3 newPosition)
    {
        currentSpawnPosition = newPosition;
    }

    public void Respawn()
    {
        transform.position = currentSpawnPosition;
        rigiBody.linearVelocity = Vector2.zero;
    }
}