using UnityEngine;
using UnityEngine.InputSystem;

public abstract class EntityState 
{
    protected Player player;
    protected StateMachine stateMachine;
    protected int animationHash;

    protected Animator animator;
    protected Rigidbody2D rb;
    protected PlayerInput_Actions playerInput;

    public EntityState(Player player, StateMachine stateMachine, int animationHash)
    {
        this.player = player;
        this.stateMachine = stateMachine;
        this.animationHash = animationHash;

        rb = player.rigidBody;
        animator = player.animator; 
        playerInput = player.inputActions;  
    }

    public virtual void Enter() 
    {
        if (animator != null && animationHash != 0)
        {
            animator.SetBool(animationHash, true);
        }
    }
    
    public virtual void Update() 
    {
    }    
    
    public virtual void Exit() 
    { 
        if (animator != null && animationHash != 0)
        {
            animator.SetBool(animationHash, false);
        }
    }

}
