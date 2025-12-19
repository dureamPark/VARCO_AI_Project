using UnityEngine;

public abstract class State : MonoBehaviour
{
    public bool isComplete { get; protected set; }

    protected float startTime;
    public float time => Time.time - startTime;

    protected StateCore core;
    protected Rigidbody2D rb => core.rb;
    protected Animator animator => core.animator;
    public AnimationClip anim;

    public StateMachine machine;

    protected StateMachine parent;
    public State state => machine.state;
    void Start()
    {
        
    }

    protected void Set(State newState, bool forceReset = false)
    {
        machine.Set(newState, forceReset);
    }

    public void SetCore(StateCore _core)
    {
        machine = new StateMachine();
        core = _core;
    }


    public virtual void Enter() { }
    public virtual void Do() { }
    public virtual void FixedDo() { }
    public virtual void Exit() { }

    public void DoBranch()
    {
        Do();
        state?.DoBranch();
    }
    public void FixedDoBranch()
    {
        FixedDo();
        state?.FixedDoBranch();
    }
    public void Initialize(StateMachine _parent)
    {
        parent = _parent;
        isComplete = false;
        startTime = Time.time;
    }
}
