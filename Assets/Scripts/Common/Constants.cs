using UnityEngine;

public static class Constants
{
    // PLAYER ANIMATION HASHES
    public static readonly int PlayerAnimationHash_IDLE = Animator.StringToHash("Idle");
    public static readonly int PlayerAnimationHash_MOVE = Animator.StringToHash("Move");
    public static readonly int PlayerAnimationHash_JUMP = Animator.StringToHash("Jump");
    public static readonly int PlayerAnimationHash_DASH = Animator.StringToHash("Dash");
    public static readonly int PlayerAnimationHash_DIE = Animator.StringToHash("Die");
    public static readonly int PlayerAnimationHash_WALLSLIDE = Animator.StringToHash("WallSlide");
    public static readonly int PlayerAnimationHash_JUMP_VELOCITY = Animator.StringToHash("yVelocity");
    public static readonly int PlayerAnimationHash_ATTACK = Animator.StringToHash("Attack");
    public static readonly int PlayerAnimationHash_COMBO_COUNTER = Animator.StringToHash("ComboCounter");
}
