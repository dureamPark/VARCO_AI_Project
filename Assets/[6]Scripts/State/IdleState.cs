using UnityEngine;

public class IdleState : State
{
    public override void Enter()
    {
        animator.Play(anim.name);
    }

    public override void Do()
    {

    }
    public override void Exit()
    {

    }
}
