using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System;

public class EnemyStats : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;
    private EnemyFSM fsm;

    // 외부에서 참조용 프로퍼티 변수 (첫 글자 대문자)
    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;

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
    }

    void Update()
    {
        
    }

    public void Initialize()
    {
        currentHealth = maxHealth;
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
        Debug.Log("ㅎㅎ");
        if (isInvincible) return;

        currentHealth -= damage;
        Debug.Log($"보스 남은 체력: {currentHealth}");

        OnHealthChanged?.Invoke(); // 이벤트

        if (blinkCo != null) StopCoroutine(blinkCo);
        blinkCo = StartCoroutine(BlinkByToggle());
        if(currentHealth <= maxHealth * 3 / 10)
        {
            Debug.Log($"2페이즈");
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (fsm != null) fsm.OnEnemyDie();
        Destroy(gameObject);
        Debug.Log("적 사망");
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
