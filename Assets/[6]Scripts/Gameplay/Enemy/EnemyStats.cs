using System;
using System.Collections;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;
    [SerializeField] private int maxShield = 100;
    [SerializeField] private int currentShield;

    // 외부 참조용 프로퍼티
    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public int MaxShield => maxShield;
    public int CurrentShield => currentShield;

    // 상태 플래그
    private bool isShieldBroken = false;
    private bool isInvincible = false;

    // 컴포넌트 참조
    private EnemyFSM fsm;
    private SpriteRenderer[] srs;
    private Coroutine blinkCo;

    // ★ 이벤트 정의
    public event Action<int> OnTakeDamage; // 데미지 수치 전달용
    public event Action OnHealthChanged;   // UI 갱신용
    public event Action OnShieldBroken;    // [핵심] StageManager가 실드 파괴 감지용
    public event Action OnDead;            // [핵심] StageManager가 사망 감지용

    void Start()
    {
        srs = GetComponentsInChildren<SpriteRenderer>(true);
        fsm = GetComponent<EnemyFSM>();
        
        Initialize();

        // 보스 체력바 UI 연결 (기존 코드 유지)
        if (EnemyStatsUI.Instance != null)
        {
            EnemyStatsUI.Instance.SetBoss(this);
        }
    }

    public void Initialize()
    {
        currentHealth = maxHealth;
        currentShield = maxShield;
        isInvincible = false;
        isShieldBroken = false;
        OnHealthChanged?.Invoke();
    }

    public void SetInvincible(bool state)
    {
        isInvincible = state;
    }

    public void TakeDamage(int damage)
    {
        // 무적 상태면 데미지 무시
        if (isInvincible) return;

        // 1. 실드 로직
        if (currentShield > 0)
        {
            currentShield -= damage;

            // 실드가 깨지는 순간 체크 (0 이하가 됐고, 아직 깨짐 처리가 안 됐을 때)
            if (currentShield <= 0 && !isShieldBroken)
            {
                currentShield = 0; 
                isShieldBroken = true;
                
                Debug.Log("🛡️ 실드 파괴됨! (이벤트 발생)");
                OnShieldBroken?.Invoke(); // ★ StageManager에게 알림
            }
        }
        // 2. 체력 로직 (실드가 없을 때)
        else
        {
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                currentHealth = 0; // 음수 방지
                Die();
            }
        }
        
        // 로그 및 기타 이벤트 처리
        // Debug.Log($"데미지 : {damage}, 남은 체력: {currentHealth}, 남은 실드: {currentShield}");
        
        OnTakeDamage?.Invoke(damage); // 피격 이벤트
        OnHealthChanged?.Invoke();    // UI 갱신 이벤트
        
        // 피격 시 깜빡임 효과
        if (blinkCo != null) StopCoroutine(blinkCo);
        blinkCo = StartCoroutine(BlinkByToggle());

        // (기존 로직) 체력 30% 이하 시 로그
        if(currentHealth <= maxHealth * 0.3f)
        {
            // Debug.Log($"2페이즈 진입 구간");
        }
    }

    private void Die()
    {
        // 1. FSM에게 죽음 알림
        if (fsm != null) fsm.OnEnemyDie();

        // 2. StageManager에게 죽음 알림 (가장 중요)
        OnDead?.Invoke();
        Debug.Log("💀 적 사망 이벤트 호출됨");
        
        // 3. 화면에 남은 투사체 정리 (기존 코드 유지)
        GameObject[] projectiles = GameObject.FindGameObjectsWithTag("EnemyProjectile");
        foreach (GameObject p in projectiles)
        {
            // EnemyProjectile 스크립트가 있으면 풀 반환, 없으면 비활성화
            // (EnemyPojectile 오타 수정 여부에 따라 클래스명 맞춰주세요)
            var ep = p.GetComponent<EnemyPojectile>(); 
            if (ep != null)
            {
                ep.ReturnToPool();
            }
            else
            {
                p.SetActive(false);
            }
        }

        // 4. 오브젝트 파괴
        Destroy(gameObject);
    }

    // 깜빡임 효과 코루틴 (기존 코드 유지)
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