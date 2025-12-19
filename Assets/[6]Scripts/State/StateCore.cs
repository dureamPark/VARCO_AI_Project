using UnityEngine;

public abstract class StateCore : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator animator;
    public AnimationClip anim;

    public StateMachine machine;

    public State state => machine.state;
    public void SetupInstance()
    {
        machine = new StateMachine();
        State[] allChildStates = GetComponentsInChildren<State>();
        foreach(State state in allChildStates)
        {
            state.SetCore(this);
        }
    }
}
