using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private int currentHealth;
    [SerializeField] private float maxMana = 100f;
    [SerializeField] private float currentMana;
    [SerializeField] private int attackPower = 1;
    
    
    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public float MaxMana => maxMana;
    public float CurrentMana => currentMana;
    public int AttackPower => attackPower;

    
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
        
        if (isInvincible) return;

        currentHealth -= damage;
        Debug.Log($"�ƾ�! ü�� ����: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("�÷��̾� ���...");
        // ���� ���� ó�� ����
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
        attackPower += amount;
        Debug.Log($"현재 공격력: {attackPower}");
    }
}