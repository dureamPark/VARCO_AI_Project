using System.Collections;
using UnityEngine;
public enum EnemyState
{
    Idle,
    Move,
    Attack,
    PhaseChange,
    Die
}

public class EnemyFSM : MonoBehaviour
{
    [Header("State")]
    [SerializeField] private EnemyState currentState;
    [SerializeField] private float MoveRate = 0.2f;
    public EnemyState CurrentState => currentState; // 외부 확인용

    [Header("Timers")]
    [SerializeField] private float idleTime = 1.0f;
    [SerializeField] private float moveTime = 2.0f;
    private float timer;

    private EnemyMovement movement;
    private EnemyStats stats;

    private EnemySkillBase skill;
    private Rigidbody2D rb;

    public int CurrentPhase => stats.CurrentHealth <= stats.MaxHealth * 0.3f ? 2 : 1;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        movement = GetComponent<EnemyMovement>();
        stats = GetComponent<EnemyStats>();
        skill = GetComponent<EnemySkillBase>();
    }
    void Start()
    {
        movement.Initialize(rb);
        stats.Initialize();

        ChangeState(EnemyState.Idle);
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                UpdateIdle();
                break;
            case EnemyState.Move:
                UpdateMove();
                break;
            case EnemyState.Attack:
                break;
            case EnemyState.Die:
                break;
        }
    }

    public void ChangeState(EnemyState newState)
    {
        currentState = newState;
        timer = 0f; // 타이머 초기화

        switch (currentState)
        {
            case EnemyState.Idle:
                Debug.Log("상태: 대기");
                movement.StopMove(); // 이동 멈춤
                break;
            case EnemyState.Move:
                Debug.Log("상태: 이동");
                movement.StartRandomMove(); // 이동 시작
                break;
            case EnemyState.Attack:
                Debug.Log("상태: 공격 시작");
                movement.StopMove(); // 공격 중엔 보통 멈춤
                skill.CastSkill(CurrentPhase, OnSkillFinished);
                break;
            case EnemyState.Die:
                // 사망 처리
                break;
        }
    }


    void UpdateIdle()
    {
        timer += Time.deltaTime;
        if (timer >= idleTime)
        {
            // 대기 시간이 끝나면 이동이나 공격으로 전환
            if (Random.value > MoveRate) ChangeState(EnemyState.Attack);
            else ChangeState(EnemyState.Move);
        }
    }

    void UpdateMove()
    {
        timer += Time.deltaTime;
        if (timer >= moveTime)
        {
            // 이동 시간이 끝나면 바로 공격
            ChangeState(EnemyState.Attack);
        }
    }
    void OnSkillFinished()
    {
        // 공격이 끝나면 다시 Idle 상태로 복귀
        ChangeState(EnemyState.Idle);
    }

    // 외부(Stats)에서 호출할 사망 처리
    public void OnEnemyDie()
    {
        ChangeState(EnemyState.Die);
    }
}
