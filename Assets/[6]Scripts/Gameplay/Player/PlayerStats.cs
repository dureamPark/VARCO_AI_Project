using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private int currentHealth;
    [SerializeField] private float maxMana = 100f;
    [SerializeField] private float currentMana;
    [SerializeField] private int attackPower = 1;
    
    // �ܺο��� ������ ������Ƽ ���� (ù ���� �빮��)
    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public float MaxMana => maxMana;
    public float CurrentMana => currentMana;
    public int AttackPower => attackPower;

    // ���� ��ų �÷��׿� ����
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
        // ���� ���¶�� �������� ���� ����
        if (isInvincible) return;

        currentHealth -= damage;
        Debug.Log($"�ƾ�! ü�� ����: {currentHealth}");

        //attackpower - 1 에서 1은 목숨 깎일 때마다 일정 공격력 수치를 낮추는 용도
        if (attackPower - 1 > 1)
        {
            attackPower -= 1;
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("�÷��̾� ���...");
        // ���� ���� ó�� ����

        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
        else
        {
            Debug.LogError("GameManager가 씬에 없습니다!");
        }
    }

    // please make player heal up method
    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        Debug.Log($"현재 체력: {currentHealth}");
    }

    // please make player attackPower up method 
    public void AttackPowerUp(int amount)
    {
        if (attackPower >= 100)
        {
            Debug.Log("최대 공격력에 도달했습니다.");
            return;
        }
        attackPower += amount;
        Debug.Log($"현재 공격력: {attackPower}");
    }
}