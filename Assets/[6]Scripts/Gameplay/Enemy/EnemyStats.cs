using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemyStats : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;
    private EnemyFSM fsm;

    [Header("Drop System")]
    [SerializeField] 
    private GameObject itemPrefab; // 떨어뜨릴 아이템 프리팹
    [SerializeField] 
    [Range(0, 100)] // 아이템 드롭 확률
    private float dropPercent = 50f; // 50%로 설정

    // �ܺο��� ������ ������Ƽ ���� (ù ���� �빮��)
    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;

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
        GenerateDropItem();

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

    private void GenerateDropItem()
    {
        UnityEngine.Debug.Log("아이템 드롭 메소드 실행");
        // 1. 떨어뜨릴 아이템이 설정되어 있지 않다면 그냥 종료
        if (itemPrefab == null)
        {
            return;
        }    

        // 2. 0부터 100 사이의 랜덤한 숫자를 뽑음
        float randomValue = Random.Range(0f, 100f);

        // 예: 확률이 30%라면, 0~30 사이의 숫자가 나와야 당첨
        if (randomValue <= dropPercent)
        {
        // 아이템 생성 (생성할 물건, 위치, 회전값(회전없는 상태))
            Instantiate(itemPrefab, transform.position, Quaternion.identity);
        
            UnityEngine.Debug.Log("아이템 드롭 성공!");
        }
    }
}
