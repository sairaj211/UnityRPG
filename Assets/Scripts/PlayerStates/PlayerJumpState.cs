using UnityEngine;

public class PlayerJumpState : PlayerAirState
{
    public PlayerJumpState(Player player, StateMachine stateMachine, int animationHash) : base(player, stateMachine, animationHash)
    {
    }
    public override void Enter()
    {
        base.Enter();
        player.coyoteTimeCounter = 0f; // Prevent further coyote jumps until grounded again
        player.SetVelocity(rb.linearVelocityX, player.jumpForce);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if(rb.linearVelocityY < 0f)
        {
            stateMachine.ChangeState(player.fallState); 
        }

        player.HandleVerticalLedgeCorrection();
    }
}
