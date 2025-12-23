using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
    [Header("Skill 1: Flow Style (Homing)")]
    [SerializeField] private float homingCooldown = 3f; // [기획서] 쿨타임 3초
    private float lastHomingSwitchTime = -10f; // 바로 쓸 수 있게 초기화

    [Header("Skill 2: Dimension Barrier")]
    [SerializeField] private GameObject barrierObject; // 플레이어 자식으로 만든 Barrier 오브젝트 연결
    [SerializeField] private float maxBarrierEnergy = 3f; // [기획서] 최대 3초 유지
    [SerializeField] private float barrierRecoveryRate = 0.5f; // 에너지 회복 속도
    [SerializeField] private float currentBarrierEnergy;

    [Header("Skill 3: Code OverWrite (Bomb)")]
    [SerializeField] private GameObject bombEffectPrefab; // 필살기 이펙트 (나중에 연결)

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
        // 방벽 에너지 꽉 채우고 시작
        currentBarrierEnergy = maxBarrierEnergy;

        // 시작할 때 방벽은 꺼둠
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
        // 1. 유도탄 모드 (X키) - 토글 방식
        if (isFlowStyleDown)
        {
            TryToggleHoming();
        }

        // 2. 차원 방벽 (C키) - 꾹 누르는 동안(Hold)
        HandleBarrier(isBarrierPressed);

        // 3. 필살기 (Ctrl키) - 즉발
        if (isOverWriteDown)
        {
            TryUseBomb();
        }
    }

    // --- 스킬 1: 유도탄 (Flow Style) ---
    private void TryToggleHoming()
    {
        // 쿨타임 체크
        if (Time.time < lastHomingSwitchTime + homingCooldown)
        {
            Debug.Log($"유도 모드 전환 쿨타임 중! 남은 시간: {lastHomingSwitchTime + homingCooldown - Time.time:F1}초");
            return;
        }

        // 현재 모드 반대로 전환 (Shooter에게 시킴)
        bool newMode = !shooter.IsHomingMode;
        shooter.SetHomingMode(newMode);

        lastHomingSwitchTime = Time.time;
    }

    // --- 스킬 2: 차원 방벽 (Barrier) ---
    private void HandleBarrier(bool isPressed)
    {
        if (barrierObject == null) return;

        // 키를 누르고 있고 + 에너지가 남았으면 -> 방벽 ON
        if (isPressed && currentBarrierEnergy > 0)
        {
            if (!barrierObject.activeSelf) barrierObject.SetActive(true);

            currentBarrierEnergy -= Time.deltaTime; // 에너지 소모

            if (currentBarrierEnergy <= 0)
            {
                Debug.Log("방벽 에너지 고갈!");
                barrierObject.SetActive(false); // 에너지 다 쓰면 강제로 끔
            }
        }
        else
        {
            // 키를 뗐거나 에너지가 없으면 -> 방벽 OFF
            if (barrierObject.activeSelf) barrierObject.SetActive(false);
        }
    }

    // --- 스킬 3: 필살기 (Code OverWrite) ---
    private void TryUseBomb()
    {
        // 밤(Bomb) 개수 확인 및 소모 (PlayerStats에 위임)
        if (stats.TryUseBomb())
        {
            Debug.Log("필살기 발동! (Code OverWrite)");

            // 1. 화면의 모든 적 탄알 찾기 (Tag: EnemyBullet)
            GameObject[] enemyBullets = GameObject.FindGameObjectsWithTag("EnemyBullet");

            int count = 0;
            // 2. 전부 삭제 (나중엔 플레이어 탄알로 변환하거나 점수로 전환)
            foreach (GameObject bullet in enemyBullets)
            {
                Destroy(bullet);
                count++;
            }
            Debug.Log($"적 탄환 {count}개 삭제됨!");

            // 3. 이펙트 생성 (프리팹이 연결되어 있다면)
            if (bombEffectPrefab != null)
            {
                Instantiate(bombEffectPrefab, transform.position, Quaternion.identity);
            }
        }
        else
        {
            Debug.Log("밤(Bomb)이 부족합니다!");
        }
    }
}