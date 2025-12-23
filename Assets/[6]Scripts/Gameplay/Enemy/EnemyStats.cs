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
    private EnemyFSM fsm;

    // �ܺο��� ������ ������Ƽ ���� (ù ���� �빮��)
    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;

    // enemy가 데미지를 받으면 아이템을 드롭하게 신호 보내는 이벤트
    public event Action<int> OnTakeDamage;
    public event Action OnDead;

    //��Ƽ�� ���� ����
    private bool isInvincible = false;

    Coroutine blinkCo;
    SpriteRenderer[] srs;

    void Start()
    {
        srs = GetComponentsInChildren<SpriteRenderer>(true);
        fsm = GetComponent<EnemyFSM>();
    }

    void Update()
    {
        
    }

    public void Initialize()
    {
        currentHealth = maxHealth;
        isInvincible = false;
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
        
        // 데미지에 따른 플레이어 점수 추가
        if (GameManager.Instance != null && GameManager.Instance.scoreManager != null)
        {
            GameManager.Instance.scoreManager.AddDamageScore(damage);
        }
        UnityEngine.Debug.Log($"데미지 : {damage}");
        OnTakeDamage?.Invoke(damage);

        UnityEngine.Debug.Log($"���� ���� ü��: {currentHealth}");
        if (blinkCo != null) StopCoroutine(blinkCo);
        blinkCo = StartCoroutine(BlinkByToggle());
        if(currentHealth <= maxHealth * 3 / 10)
        {
            UnityEngine.Debug.Log($"2������");
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
        Destroy(gameObject);
        UnityEngine.Debug.Log("�� ���");
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
