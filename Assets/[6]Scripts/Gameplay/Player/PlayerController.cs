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

        if (shooter != null)
        {
            shooter.HandleShooting();
        }
    }

    private void HandleInput()
    {
        Vector2 inputDir = inputManager.GetMovementInput();
        movement.Move(inputDir);

        // 2. 스킬 입력 처리
        if (skill != null)
        {
            // K: 가속 스킬
            if (inputManager.GetSpeedSkillDown())
            {
                skill.UseSpeedSkill();
            }

            // L: 방어 스킬
            if (inputManager.GetShieldSkillDown())
            {
                skill.UseShieldSkill();
            }

            // J: 폭격 스킬 (추후 탄막 만들어지면 구현)
            if (inputManager.GetBombSkillDown())
            {
                Debug.Log("폭격 스킬 사용 (아직 미구현)");
            }
        }
    }
}