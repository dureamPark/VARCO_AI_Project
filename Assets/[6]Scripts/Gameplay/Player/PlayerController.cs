using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // 부품들 참조
    private IInputManager inputManager;
    private PlayerMovement movement;
    private PlayerStats stats;
    private PlayerShooter shooter;
    private PlayerSkill skill;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        movement = GetComponent<PlayerMovement>();
        stats = GetComponent<PlayerStats>();
        shooter = GetComponent<PlayerShooter>(); 
        skill = GetComponent<PlayerSkill>();     

        // 의존성 주입 
        inputManager = new KeyboardInputManager();
    }

    private void Start()
    {
        movement.Initialize(rb);
        stats.Initialize();
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        // 1. 이동
        movement.Move(inputManager.GetMovementInput());

        // 2. 공격
        if (shooter != null)
        {
            shooter.HandleShooting(inputManager.GetAttackDown());
        }

        // 3. 스킬 (아직 구현 안됨)
        if (inputManager.GetFlowStyleDown()) Debug.Log("유도탄 모드(X) 입력됨");
        if (inputManager.GetBarrierKey()) Debug.Log("배리어(Ctrl) 누르는 중");
        if (inputManager.GetOverWriteDown()) Debug.Log("필살기(C) 입력됨");
    }
}