using UnityEngine;

public class AirState : State
{
    public float jumpSpeed;
    public override void Enter()
    {
        animator.Play(anim.name);
    }

    public override void Do()
    {
        isComplete = true;
    }
    public override void Exit()
    {

    }
}
