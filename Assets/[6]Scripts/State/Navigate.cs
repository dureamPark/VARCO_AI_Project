using UnityEngine;

public class Navigate : State
{
    public Vector2 destination;
    public float speed = 1.0f;
    public float threshold = 0.1f;
    public State animation;

    public override void Enter()
    {
        Set(animation, true);
    }

    public override void Do()
    {
        if(Vector2.Distance(core.transform.position, destination) < threshold){
            isComplete = true;
        }
        FaceDestination();
    }
    public override void FixedDo()
    {
        Vector2 direction = (destination - (Vector2)core.transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * speed, rb.linearVelocity.y);
    }

    void FaceDestination()
    {
        core.transform.localScale = new Vector3(Mathf.Sign(rb.linearVelocity.x), 1, 1);
    }
}
