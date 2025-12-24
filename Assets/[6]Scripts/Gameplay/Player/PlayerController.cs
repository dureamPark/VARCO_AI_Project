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
        // 이동
        movement.Move(inputManager.GetMovementInput());

        // 공격
        if (shooter != null)
        {
            shooter.HandleShooting(inputManager.GetAttackDown());
        }

        // 스킬 
        if (skill != null)
        {
            skill.HandleSkills(
                inputManager.GetFlowStyleDown(), // 토글형
                inputManager.GetBarrierKey(),    // 지속형
                inputManager.GetOverWriteDown()  // 즉발형
            );
        }
    }
}