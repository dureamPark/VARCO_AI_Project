using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemyStats : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;
    [SerializeField] private int maxShield = 100;
    [SerializeField] private int currentShield;
    private EnemyFSM fsm;

    // 외부에서 참조용 프로퍼티 변수 (첫 글자 대문자)
    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public int MaxShield => maxShield;
    public int CurrentShield => currentShield;
    
    // enemy가 데미지를 받으면 아이템을 드롭하게 신호 보내는 이벤트
    public event Action<int> OnTakeDamage;
    public event Action OnDead;
    public event Action OnHealthChanged; // 이벤트

    //버티기 동안 무적
    private bool isInvincible = false;

    Coroutine blinkCo;
    SpriteRenderer[] srs;

    void Start()
    {
        srs = GetComponentsInChildren<SpriteRenderer>(true);
        fsm = GetComponent<EnemyFSM>();
        Initialize();

        if (EnemyStatsUI.Instance != null)
        {
            EnemyStatsUI.Instance.SetBoss(this);
        }
    }

    void Update()
    {
        
    }

    public void Initialize()
    {
        currentHealth = maxHealth;
        currentShield = maxShield;
        isInvincible = false;

        OnHealthChanged?.Invoke();
    }

    public void SetInvincible(bool state)
    {
        isInvincible = state;
    }

    public void TakeDamage(int damage)
    {
        // 무적 상태라면 데미지를 입지 않음
        if (isInvincible) return;

        if(currentShield > 0)
        {
            currentShield -= damage;
        }
        else
        {
            currentHealth -= damage;
        }
        UnityEngine.Debug.Log($"데미지 : {damage}");
        OnTakeDamage?.Invoke(damage);
        UnityEngine.Debug.Log($"보스 남은 체력: {currentHealth}");
        UnityEngine.Debug.Log($"보스 남은 실드: {currentShield}");

        OnHealthChanged?.Invoke(); // 이벤트
        
        if (blinkCo != null) StopCoroutine(blinkCo);
        blinkCo = StartCoroutine(BlinkByToggle());
        if(currentHealth <= maxHealth * 3 / 10)
        {
            UnityEngine.Debug.Log($"2페이즈");
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (fsm != null) fsm.OnEnemyDie();

        OnDead?.Invoke();
        UnityEngine.Debug.Log("OnDead 이벤트 호출됨");
        
        GameObject[] projectiles = GameObject.FindGameObjectsWithTag("EnemyProjectile");

        foreach (GameObject p in projectiles)
        {
            EnemyPojectile ep = p.GetComponent<EnemyPojectile>();
            if (ep != null)
            {
                ep.ReturnToPool();
            }
            else
            {
                p.SetActive(false);
            }
        }
        Destroy(gameObject);
        UnityEngine.Debug.Log("적 사망");
    }

    IEnumerator BlinkByToggle()
    {
        const int times = 6;
        const float interval = 0.05f;

        for (int i = 0; i < times; i++)
        {
            SetRenderersEnabled(false);
            yield return new WaitForSeconds(interval);
            SetRenderersEnabled(true);
            yield return new WaitForSeconds(interval);
        }

        SetRenderersEnabled(true);
        blinkCo = null;
    }

    void SetRenderersEnabled(bool on)
    {
        if (srs == null) return;
        for (int i = 0; i < srs.Length; i++)
        {
            if (srs[i] != null) srs[i].enabled = on;
        }
    }
}
