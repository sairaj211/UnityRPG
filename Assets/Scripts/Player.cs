using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    private StateMachine stateMachine;
    public Rigidbody2D rigidBody { get; private set; }
    private SpriteRenderer spriteRenderer;

    #region States
    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerFallState fallState{ get; private set; }
    #endregion

    #region InputAction
    public PlayerInput_Actions inputActions { get; private set; }
    #endregion

    public Vector2 moveInput { get; private set; }

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float maxFallSpeed = -10f; // Negative value for downward velocity

    [Header("Jump Settings")]
    public float coyoteTime = 0.15f; // How long after leaving ground you can still jump
    public float coyoteTimeCounter; // Should be public if accessed from states
    public bool CanCoyoteJump => coyoteTimeCounter > 0f;

    private bool isFacingRight = true;
    public int facingDir { get; private set; } = 1;
    [Range(0f, 1f)]
    public float inAirMoveMultiplier = 0.5f;

    [Header("Collision Detection")]
    [SerializeField] private float groundCheckDistance = 0.1f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.5f, 0.1f);

    public bool isGrounded;

    [SerializeField] private Transform ledgeCheckLeft;
    [SerializeField] private Transform ledgeCheckRight;
    [SerializeField] private float ledgeCheckDistance = 0.1f;
    [SerializeField] private float ledgeCorrectionHeight = 0.5f;

    private bool isLedgeCorrecting = false;
    private Vector3 ledgeCorrectionTarget;
    public float ledgeCorrectionSpeed = 10f; // Tune for feel
    private bool hasLedgeCorrected = false;

    public Animator animator { get; private set; }

    private void Awake()
    {
        stateMachine = gameObject.AddComponent<StateMachine>();
        rigidBody = GetComponent<Rigidbody2D>();
        inputActions = new PlayerInput_Actions();
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        idleState = new PlayerIdleState(this, stateMachine, Constants.PlayerAnimationHash_IDLE);
        moveState = new PlayerMoveState(this, stateMachine, Constants.PlayerAnimationHash_MOVE);
        jumpState = new PlayerJumpState(this, stateMachine, Constants.PlayerAnimationHash_JUMP);
        fallState = new PlayerFallState(this, stateMachine, Constants.PlayerAnimationHash_JUMP);
    }

    private void OnEnable()
    {
        inputActions.Enable();

        inputActions.Player.Movement.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Movement.canceled += ctx => moveInput = Vector2.zero;
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Start()
    {
        stateMachine.Initialize(idleState);
    }

    private float groundedTime = 0f;
    private const float minGroundedTime = 0.05f;

    private void Update()
    {
        HandleCollisionDetection();

        if (isGrounded)
        {
            groundedTime += Time.deltaTime;
            if (groundedTime > minGroundedTime)
                hasLedgeCorrected = false;
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            groundedTime = 0f;
            coyoteTimeCounter -= Time.deltaTime;
        }


        // Smooth ledge correction
        if (isLedgeCorrecting)
        {
            transform.position = Vector3.MoveTowards(transform.position, ledgeCorrectionTarget, ledgeCorrectionSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, ledgeCorrectionTarget) < 0.01f)
            {
                transform.position = ledgeCorrectionTarget;
                isLedgeCorrecting = false;
            }
            return; // Skip state update while correcting
        }

        stateMachine.UpdateActiveState();
        DebugManager.Log($"Current State {stateMachine.currentState}");
    }

    public void SetVelocity(float xVelocity, float yVelocity)
    {
        rigidBody.linearVelocity = new Vector2(xVelocity, yVelocity);
        HandleFlip();
    }

    private void HandleFlip()
    {
        // Only flip if there is horizontal input
        if (moveInput.x > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (moveInput.x < 0 && isFacingRight)
        {
            Flip();
        }
    }

    public void Flip()
    {


        //using scale to flip the sprite
        isFacingRight = !isFacingRight;
        facingDir = isFacingRight ? 1 : -1;
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * facingDir;
        transform.localScale = scale;


        //if (isFacingRight)
        //{
        //    Vector3 rotator = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
        //    transform.rotation = Quaternion.Euler(rotator);
        //    isFacingRight = !isFacingRight;
        //}
        //else
        //{
        //    Vector3 rotator = new Vector3(transform.rotation.x, 0f, transform.rotation.z);
        //    transform.rotation = Quaternion.Euler(rotator);
        //    isFacingRight = !isFacingRight;
        //}
    }

    private void HandleCollisionDetection()
    {
        isGrounded = Physics2D.BoxCast(
            groundCheckPoint.position,
            groundCheckSize,
            0f,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );
    }

    public void HandleVerticalLedgeCorrection()
    {
        if (isGrounded) return; // Don't correct if grounded
        if (isLedgeCorrecting || hasLedgeCorrected) return; // Prevent repeated correction
                                                            // Only correct if moving upward (jumping)
        if (rigidBody.linearVelocity.y <= 0f) return;

        // Determine which check point is actually on the left/right in world space
        Transform worldLeft = ledgeCheckLeft;
        Transform worldRight = ledgeCheckRight;

        if (transform.localScale.x < 0)
        {
            // Flipped: swap references
            worldLeft = ledgeCheckRight;
            worldRight = ledgeCheckLeft;
        }

        // Now use worldLeft for left checks, worldRight for right checks
        bool leftBlocked = Physics2D.Raycast(worldLeft.position, Vector2.left, ledgeCheckDistance, groundLayer);
        bool rightBlocked = Physics2D.Raycast(worldRight.position, Vector2.right, ledgeCheckDistance, groundLayer);

        bool leftSpaceAbove = !Physics2D.Raycast(worldLeft.position + Vector3.up * ledgeCorrectionHeight, Vector2.left, ledgeCheckDistance, groundLayer);
        bool rightSpaceAbove = !Physics2D.Raycast(worldRight.position + Vector3.up * ledgeCorrectionHeight, Vector2.right, ledgeCheckDistance, groundLayer);

        if ((leftBlocked && leftSpaceAbove) || (rightBlocked && rightSpaceAbove))
        {
            ledgeCorrectionTarget = transform.position + new Vector3(0, ledgeCorrectionHeight, 0);
            isLedgeCorrecting = true;
            hasLedgeCorrected = true; // Set flag so it only happens once
        }
    }


    private void OnDrawGizmos()
    {
        if (groundCheckPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(
                groundCheckPoint.position + Vector3.down * groundCheckDistance,
                groundCheckSize
            );
        }
    }
}
