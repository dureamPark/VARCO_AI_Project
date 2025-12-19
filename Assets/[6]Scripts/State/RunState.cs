using UnityEngine;

public class RunState : State
{
    public float maxSpeed;
    public override void Enter()
    {
        animator.Play(anim.name);
    }

    public override void Do()
    {
        float velX = rb.linearVelocity.x;
        animator.speed = Helpers.Map(maxSpeed, 0, 1, 0, 1.6f, true);

        if(Mathf.Abs(velX) < 0.1f)
        {
            isComplete = true;
        }
    }
    public override void Exit()
    {

    }
}
