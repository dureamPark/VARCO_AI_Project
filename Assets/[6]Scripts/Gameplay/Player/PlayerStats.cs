using UnityEngine;
using System;

public class PlayerStats : MonoBehaviour
{
    [Header("Game Stats")]
    [SerializeField] private int maxLives = 5;    
    [SerializeField] private int currentLives;

    [SerializeField] private int maxBombs = 3;  
    [SerializeField] private int currentBombs;

    [Header("Attack Info")]
    [SerializeField] private int attackPower = 1;
    private const int MaxAttackPower = 5;    

    // UI 갱신용 이벤트
    public event Action OnStatsChanged;

    // 외부에서 갖다 쓸 프로퍼티
    public int AttackPower => attackPower;
    public int CurrentLives => currentLives;
    public int CurrentBombs => currentBombs;

    private bool isDead = false;

    public void Initialize()
    {
        currentLives = maxLives;
        currentBombs = maxBombs;
        attackPower = 1;
        isDead = false;

        OnStatsChanged?.Invoke();
    }

    // 적 총알이 플레이어에게 닿았을 때 직접 호출하는 함수
    public void TakeDamage()
    {
        if (isDead) return;

        Debug.Log("피격! 목숨 차감");
        LoseLife();
    }

    private void LoseLife()
    {
        currentLives--;
        OnStatsChanged?.Invoke(); // UI 갱신
        Debug.Log($"남은 목숨: {currentLives}");

        if (currentLives > 0)
        {
            Debug.Log("플레이어 부활");
        }
        else
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("게임 오버 (Game Over)");
        // GameManager.Instance.GameOver(); // 나중에 연결
        gameObject.SetActive(false); // 플레이어 끄기
    }

    // 밤 사용 시도 (PlayerSkill에서 호출)
    public bool TryUseBomb()
    {
        if (currentBombs > 0)
        {
            currentBombs--;
            OnStatsChanged?.Invoke();
            return true;
        }
        return false;
    }

    // 공격력 증가
    public void AttackPowerUp(int amount)
    {
        attackPower += amount;
        if (attackPower > MaxAttackPower) attackPower = MaxAttackPower;
        OnStatsChanged?.Invoke();
    }
}