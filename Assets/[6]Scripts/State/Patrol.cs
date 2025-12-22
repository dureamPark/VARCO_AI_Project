using UnityEngine;

public class Patrol: State
{
    public Navigate navigate;
    public IdleState idle;
    public Transform anchor1;
    public Transform anchor2;
    const float IDLETIME = 5.0f;

    void GoToNextDestination()
    {
        //float randomSpot = Random.Range(anchor1.position.x, anchor2.position.x);
        //navigate.destination = new Vector2 (randomSpot, core.transform.position.y);
        if(navigate.destination == (Vector2)anchor1.position)
        {
            navigate.destination = anchor2.position;
        }
        else
        {
            navigate.destination = anchor1.position;
        }
        Set(navigate, true);
    }

    public override void Enter()
    {
        GoToNextDestination();
    }

    public override void Do()
    {
        if(machine.state == navigate)
        {
            if (navigate.isComplete)
            {
                Set(idle, true);
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            }
            else
            {
                if(machine.state.time > IDLETIME)
                {
                    GoToNextDestination ();
                }
            }
        }
    }
}
