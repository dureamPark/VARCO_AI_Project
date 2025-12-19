using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private int currentHealth;
    [SerializeField] private float maxMana = 100f;
    [SerializeField] private float currentMana;
    [SerializeField] private int attackPower = 1;
    
    // 외부에서 참조용 프로퍼티 변수 (첫 글자 대문자)
    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public float MaxMana => maxMana;
    public float CurrentMana => currentMana;
    public int AttackPower => attackPower;

    // 무적 스킬 플래그용 변수
    private bool isInvincible = false;

    public void Initialize()
    {
        currentHealth = maxHealth;
        currentMana = 0f;
        isInvincible = false;
    }

    private void Update()
    {
        if (currentMana < maxMana)
        {
            currentMana += Time.deltaTime * 5f;
        }
    }

    public void SetInvincible(bool state)
    {
        isInvincible = state;
    }

    public void TakeDamage(int damage)
    {
        // 무적 상태라면 데미지를 입지 않음
        if (isInvincible) return;

        currentHealth -= damage;
        Debug.Log($"아야! 체력 남음: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("플레이어 사망...");
        // 게임 오버 처리 로직
    }
}