using System;
using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
    [Header("Skill 1: Flow Style (Homing)")]
    [SerializeField] private float homingCooldown = 3f; 
    private float lastHomingSwitchTime = -10f; 

    [Header("Skill 2: Dimension Barrier")]
    [SerializeField] private GameObject barrierObject; 
    [SerializeField] private float maxBarrierEnergy = 3f; 
    [SerializeField] private float barrierRecoveryRate = 0.5f; 
    [SerializeField] private float currentBarrierEnergy;

    [Header("Skill 3: Code OverWrite (Bomb)")]
    [SerializeField] private GameObject overwriteBulletPrefab;
    [SerializeField] private GameObject bombEffectPrefab; // 필살기 이펙트 (나중에 연결)


    public float CurrentBarrierEnergy => currentBarrierEnergy; //Ui에서 땡겨쓸라고 여따 추가했습니다 두사부
    public float MaxBarrierEnergy => maxBarrierEnergy;
    public bool IsHomingMode => shooter != null && shooter.IsHomingMode;
    public event Action<bool> OnWeaponModeChanged;

    // 컴포넌트 참조
    private PlayerShooter shooter;
    private PlayerStats stats;

    private void Awake()
    {
        shooter = GetComponent<PlayerShooter>();
        stats = GetComponent<PlayerStats>();
    }

    private void Start()
    {
        currentBarrierEnergy = maxBarrierEnergy;

        if (barrierObject != null) barrierObject.SetActive(false);
    }

    private void Update()
    {
        // 방벽 에너지 회복 로직 (방벽을 안 쓰고 있을 때만)
        if (barrierObject != null && !barrierObject.activeSelf)
        {
            if (currentBarrierEnergy < maxBarrierEnergy)
            {
                currentBarrierEnergy += Time.deltaTime * barrierRecoveryRate;
            }
            // 최대치를 넘지 않게 고정
            else if (currentBarrierEnergy > maxBarrierEnergy)
            {
                currentBarrierEnergy = maxBarrierEnergy;
            }
        }
    }

    // PlayerController에서 호출하는 함수
    public void HandleSkills(bool isFlowStyleDown, bool isBarrierPressed, bool isOverWriteDown)
    {
        // 유도탄 모드 (X키) - 토글
        if (isFlowStyleDown)
        {
            TryToggleHoming();
            AudioEvents.TriggerPlaySFX("PlayerSkillX");
            AudioEvents.TriggerPlaySFX("PlayerSkillXVoice");
        }

        // 차원 방벽 (C키) - 키다운
        HandleBarrier(isBarrierPressed);

        // 필살기 (Ctrl키) - 즉발
        if (isOverWriteDown)
        {
            TryUseBomb();
            AudioEvents.TriggerPlaySFX("PlayerSkillCtrl");
            AudioEvents.TriggerPlaySFX("PlayerSkillCtrlVoice");
        }
    }

    // 스킬 1: 유도탄 (Flow Style)
    private void TryToggleHoming()
    {
        if (Time.time < lastHomingSwitchTime + homingCooldown)
        {
            Debug.Log($"유도 모드 전환 쿨타임 중! 남은 시간: {lastHomingSwitchTime + homingCooldown - Time.time:F1}초");
            return;
        }

        bool newMode = !shooter.IsHomingMode;
        shooter.SetHomingMode(newMode);

        lastHomingSwitchTime = Time.time;
        OnWeaponModeChanged?.Invoke(newMode);
    }

    // 스킬 2: 차원 방벽 (Barrier)
    private void HandleBarrier(bool isPressed)
    {
        if (barrierObject == null) return;

        if (isPressed && currentBarrierEnergy > 0)
        {
            if (!barrierObject.activeSelf)
            {
                barrierObject.SetActive(true);
                AudioEvents.TriggerPlaySFX("PlayerSkillC");
                AudioEvents.TriggerPlaySFX("PlayerSkillCVoice");
            }

            currentBarrierEnergy -= Time.deltaTime; 

            if (currentBarrierEnergy <= 0)
            {
                Debug.Log("방벽 에너지 고갈!");
                barrierObject.SetActive(false); 
            }
        }
        else
        {
            if (barrierObject.activeSelf) barrierObject.SetActive(false);
        }
    }

    // 스킬 3: 필살기 (Code OverWrite)
    public void TryUseBomb()
    {
        if (stats.TryUseBomb())
        {
            Debug.Log("필살기 발동! (Code OverWrite)");

            GameObject[] enemyBullets = GameObject.FindGameObjectsWithTag("EnemyProjectile");

            GameObject boss = GameObject.FindGameObjectWithTag("Enemy");

            int count = 0;

            foreach (GameObject enemyBullet in enemyBullets)
            {
                Vector3 targetPos = enemyBullet.transform.position;

                Destroy(enemyBullet);

                if (overwriteBulletPrefab != null)
                {
                    GameObject myBullet = ObjectPoolManager.Instance.Spawn(overwriteBulletPrefab, targetPos, Quaternion.identity);

                    PlayerProjectile projectile = myBullet.GetComponent<PlayerProjectile>();
                    if (projectile != null)
                    {
                        int damage = stats.AttackPower;

                        Vector2 finalDir = Vector2.up; // 적이 없으면 위로

                        if (boss != null && boss.activeInHierarchy)
                        {
                            finalDir = (boss.transform.position - targetPos).normalized;
                        }

                        projectile.Initialize(finalDir, damage, true);
                    }
                }
                count++;
            }

            Debug.Log($"적 탄환 {count}개를 내 유도탄으로 변환!");

            // 필살기 이펙트 (화면 중앙이나 플레이어 위치 등)
            if (bombEffectPrefab != null)
            {
                Instantiate(bombEffectPrefab, transform.position, Quaternion.identity);
            }
        }
        else
        {
            Debug.Log("Bomb이 부족합니다!");
        }
    }
}