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
    private const int MaxAttackPower = 100; //최대 공격력 100으로 수정   

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

        if (PlayerStatsUI.Instance != null)
        {
            PlayerStatsUI.Instance.SetPlayer(this);
        }

        OnStatsChanged?.Invoke();
    }

    // 적 총알이 플레이어에게 닿았을 때 직접 호출하는 함수
    public void TakeDamage()
    {
        if (isDead) return;

        // OnStatsChanged?.Invoke();

        Debug.Log("피격! 목숨 차감");
        LoseLife();
    }

    private void LoseLife()
    {
        currentLives--;
        Debug.Log($"남은 목숨: {currentLives}");
        
        if (attackPower - 1 > 0)
        {
            attackPower -= 1;
        }

        if (currentLives > 0)
        {
            Debug.Log("플레이어 부활");
        }
        else
        {
            Die();
        }
        OnStatsChanged?.Invoke(); 
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("게임 오버 (Game Over)");

        gameObject.SetActive(false); // 플레이어 끄기

        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
        else
        {
            Debug.LogError("GameManager가 씬에 없습니다!");
        }
        Debug.Log("�÷��̾� ���...");
    }

    // continue 누를 시 호출할 부활 함수
    public void Revive()
    {
        isDead = false;

        currentLives = 3;

        gameObject.SetActive(true);

        // 체력 3으로 UI 업뎃 해주시면 됩니다

        Debug.Log($"플레이어 부활! HP: {currentLives}");
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
    public void AttackPowerUp(int attackUpItem)
    {
        if (attackPower >= 100)
        {
            Debug.Log("최대 공격력에 도달했습니다.");
            return;
        }
        if (attackPower < MaxAttackPower)
        {
            attackPower += attackUpItem;
            OnStatsChanged?.Invoke();
        }
    }

    public void HealLife(int healItem)
    {
        if (currentLives < maxLives)
        {
            currentLives += healItem;
            OnStatsChanged?.Invoke();
        }    
    }
}