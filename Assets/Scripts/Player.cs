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
        stateMachine.UpdateActiveState();
        DebugManager.Log($"Current State {stateMachine.currentState}");

        Flip();
    }

    public void SetVelocity(float xVelocity, float yVelocity)
    {
        rigidBody.linearVelocity = new Vector2(xVelocity, yVelocity);
    }

    private void Flip()
    {
        // Flip sprite based on movement direction
        if (moveInput.x != 0)
        {
            spriteRenderer.flipX = moveInput.x < 0;
        }
    }
}
