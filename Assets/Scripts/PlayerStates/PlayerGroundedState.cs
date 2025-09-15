using UnityEngine;

public class PlayerGroundedState : EntityState
{
    public PlayerGroundedState(Player player, StateMachine stateMachine, int animationHash) : base(player, stateMachine, animationHash)
    {
    }
    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        if(rb.linearVelocityY < 0f)
        {
            stateMachine.ChangeState(player.fallState);
        }

        if (playerInput.Player.Jump.WasPerformedThisFrame())
        {
            stateMachine.ChangeState(player.jumpState); 
        }
    }
}
