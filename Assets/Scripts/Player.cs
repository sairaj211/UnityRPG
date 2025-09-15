using UnityEngine;

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
    private bool facingRight = true;
    public int facingDir { get; private set; } = 1;
    [Range(0f, 1f)]
    public float inAirMoveMultiplier = 0.5f;

    [Header("Collision Detection")]
    [SerializeField] private float groundCheckDistance = 0.1f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.5f, 0.1f);

    public bool isGrounded;


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

    private void Update()
    {
        HandleCollisionDetection();
        stateMachine.UpdateActiveState();
        DebugManager.Log($"Current State {stateMachine.currentState}");
    }

    public void SetVelocity(float xVelocity, float yVelocity)
    {
        rigidBody.linearVelocity = new Vector2(xVelocity, yVelocity);
        HandleFlip(xVelocity);
    }

    private void HandleFlip(float xVelcoity)
    {
        // Only flip if there is horizontal input
        if (moveInput.x > 0 && !facingRight)
        {
            Flip();
        }
        else if (moveInput.x < 0 && facingRight)
        {
            Flip();
        }
    }

    public void Flip()
    {
        facingRight = !facingRight;
        facingDir = facingRight ? 1 : -1;
        // Rotate the transform to flip the sprite
        transform.localScale = new Vector3(facingDir, 1, 1);
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
