using UnityEngine;
using System;
using System.Collections;

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

    [Header("Hit Effect Settings")]
    [SerializeField] private float flashDuration = 1.0f; // 무적 지속 시간
    [SerializeField] private float flashInterval = 0.1f; // 깜빡임 속도
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool isInvincible = false;

    // UI 갱신용 이벤트
    public event Action OnStatsChanged;


    // 외부에서 갖다 쓸 프로퍼티
    public int AttackPower => attackPower;
    public int CurrentLives => currentLives;
    public int CurrentBombs => currentBombs;

    private bool isDead = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

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

        if (isInvincible) return;

        // OnStatsChanged?.Invoke();

        Debug.Log("피격! 목숨 차감");
        AudioEvents.TriggerPlaySFX("PlayerHit");
        AudioEvents.TriggerPlaySFX("PlayerHitVoice");
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

            StartCoroutine(HitFlashRoutine());
        }
        else
        {
            AudioEvents.TriggerPlaySFX("PlayerDeath");
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

        if (PlayerStatsUI.Instance != null)
        {
            PlayerStatsUI.Instance.ShowGameOverPanel();
        }
    }

    // continue 누를 시 호출할 부활 함수
    public void Revive()
    {
        isDead = false;

        currentLives = 3;

        gameObject.SetActive(true);

        isInvincible = false;
        if (spriteRenderer != null) spriteRenderer.color = originalColor;

        OnStatsChanged?.Invoke();

        StartCoroutine(HitFlashRoutine()); // 부활 시 무적 효과

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

    // 목숨 증가
    public void HealLife(int healItem)
    {
        if (currentLives < maxLives)
        {
            currentLives += healItem;
            OnStatsChanged?.Invoke();
        }    
    }

    // 밤 아이템 획득시 호출해주시면 됩니다
    public void IncreaseBomb(int bombItem)
    {
        if (currentBombs < maxBombs)
        {
            currentBombs += bombItem;
            OnStatsChanged?.Invoke();
        }
    }

    IEnumerator HitFlashRoutine()
    {
        isInvincible = true; 

        Color flashColor = originalColor;
        flashColor.a = 0.3f; 

        float elapsed = 0f;

        while (elapsed < flashDuration)
        {
            if (spriteRenderer != null) spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(flashInterval);

            if (spriteRenderer != null) spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(flashInterval);

            elapsed += flashInterval * 2;
        }

        if (spriteRenderer != null) spriteRenderer.color = originalColor;
        isInvincible = false; 
    }
}