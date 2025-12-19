using System.Collections; 
using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
    [Header("Skill Settings")]
    [SerializeField] private float speedBuffDuration = 2.0f;   
    [SerializeField] private float invincibilityDuration = 3.0f; 
    [SerializeField] private float speedMultiplier = 2.0f;   

    private PlayerMovement movement;
    private PlayerStats stats;

    // 스킬 중복 사용 방지용 플래그
    private bool isSpeedSkillActive = false;
    private bool isShieldSkillActive = false;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        stats = GetComponent<PlayerStats>();
    }

    // K키 입력 시 호출
    public void UseSpeedSkill()
    {
        if (!isSpeedSkillActive)
        {
            StartCoroutine(SpeedBuffRoutine());
        }
    }

    // L키 입력 시 호출
    public void UseShieldSkill()
    {
        if (!isShieldSkillActive)
        {
            StartCoroutine(InvincibilityRoutine());
        }
    }

    // 가속 스킬 로직 (코루틴)
    private IEnumerator SpeedBuffRoutine()
    {
        isSpeedSkillActive = true;

        float originalSpeed = movement.MoveSpeed; 
        movement.SetSpeed(originalSpeed * speedMultiplier);

        Debug.Log("가속 스킬 발동!");

        yield return new WaitForSeconds(speedBuffDuration);

        movement.SetSpeed(originalSpeed);
        isSpeedSkillActive = false;

        Debug.Log("가속 종료");
    }

    // 무적 스킬 로직
    private IEnumerator InvincibilityRoutine()
    {
        isShieldSkillActive = true;

        stats.SetInvincible(true);
        Debug.Log("무적 스킬 발동!");

        yield return new WaitForSeconds(invincibilityDuration);

        stats.SetInvincible(false);
        isShieldSkillActive = false;
        Debug.Log("무적 종료");
    }
}