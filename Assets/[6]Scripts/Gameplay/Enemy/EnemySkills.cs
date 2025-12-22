using System.Collections;
using UnityEngine;

public class EnemySkills : MonoBehaviour
{
    [Header("Skill Settings")]
    [SerializeField] private float skillCoolDown = 2.0f;
    [SerializeField] private float invincibilityDuration = 5.0f;

    [Header("Phase 2 Prefabs (Pentagon)")]
    [SerializeField] private GameObject pentagonBulletPrefab;

    private Color prevColor;

    private EnemyStats stats;
    private SpriteRenderer sr;

    private System.Action onSkillEndCallback;

    [Header("Prefabs")]
    [SerializeField] private GameObject triangleBulletPrefab; // 삼각형 탄막 (거친 미래)
    [SerializeField] private GameObject squareBulletPrefab;   // 사각형 탄막 (믿음의 신뢰)
    [SerializeField] private GameObject circleBulletPrefab;   // 원형 탄막 (자유의 감옥)

    private Transform playerTransform;
    private void Awake()
    {
        stats = GetComponent<EnemyStats>();
        sr = GetComponent<SpriteRenderer>();
        prevColor = sr.color;
    }
    void Start()
    {
        FindPlayer();
    }

    void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerTransform = playerObj.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void CastSkillByPhase(int phase, System.Action onFinished)
    {
        this.onSkillEndCallback = onFinished;
        if (playerTransform == null) FindPlayer();

        if (phase == 1)
        {
            // 1페이즈: 0~2번 스킬
            int rnd = Random.Range(0, 3);
            switch (rnd)
            {
                case 0: StartCoroutine(Skill_RoughFuture()); break;
                case 1: StartCoroutine(Skill_TrustOfBelief()); break;
                case 2: StartCoroutine(Skill_PrisonOfFreedom()); break;
            }
        }
        else
        {
            // 2페이즈: 신규 스킬 (인덱스 3~7에 해당)
            int rnd = Random.Range(0, 5);
            switch (rnd)
            {
                case 0: StartCoroutine(Skill_GraceOfGod()); break;    // 신의 은총
                case 1: StartCoroutine(Skill_DistortedShape()); break; // 왜곡된 도형
                case 2: StartCoroutine(Skill_ChaosPolygon()); break;   // 카오스 폴리곤
                case 3: StartCoroutine(Skill_ErodingWave()); break;    // 침식하는 파동
                case 4: StartCoroutine(Skill_CoveredFuture()); break;  // 덮인 미래
            }
        }
    }

    // ========================================================================
    // Phase 1 Skills
    // ========================================================================

    // 1. 거친 미래 (기본)
    private IEnumerator Skill_RoughFuture()
    {
        Debug.Log("1페이즈: 거친 미래");
        yield return StartCoroutine(Routine_RoughFuture()); // 로직 재사용을 위해 분리함
        yield return new WaitForSeconds(skillCoolDown);
        onSkillEndCallback?.Invoke();
    }

    // 2. 믿음의 신뢰
    private IEnumerator Skill_TrustOfBelief()
    {
        Debug.Log("1페이즈: 믿음의 신뢰");
        int explosionCount = 4;
        for (int i = 0; i < explosionCount; i++)
        {
            Vector2 origin = GetRandomScreenPos();
            FireNWay(squareBulletPrefab, origin, 12, 6f);
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(skillCoolDown);
        onSkillEndCallback?.Invoke();
    }

    // 3. 자유의 감옥
    private IEnumerator Skill_PrisonOfFreedom()
    {
        Debug.Log("1페이즈: 자유의 감옥");
        for (int k = 0; k < 4; k++)
        {
            if (playerTransform == null) break;
            Vector2 targetPos = playerTransform.position;

            // 플레이어를 둘러싸는 원형 배치
            int count = 16;
            float step = 360f / count;
            for (int i = 0; i < count; i++)
            {
                Vector2 spawnPos = targetPos + (Vector2)(Quaternion.Euler(0, 0, i * step) * Vector2.right * 4f);
                Vector2 dir = (targetPos - spawnPos).normalized;
                CreateBullet(circleBulletPrefab, spawnPos, dir, 3f);
            }
            yield return new WaitForSeconds(2.0f);
        }
        yield return new WaitForSeconds(skillCoolDown);
        onSkillEndCallback?.Invoke();
    }


    // ========================================================================
    // Phase 2 Skills (Pentagon Theme)
    // ========================================================================

    // 4. 신의 은총: <거친 미래> + 오각형에서 플레이어 조준 사격 (3초)
    private IEnumerator Skill_GraceOfGod()
    {
        Debug.Log("2페이즈: 신의 은총");

        // 병렬 실행: 거친 미래 패턴을 시작하고, 동시에 추가 공격을 수행
        StartCoroutine(Routine_RoughFuture());

        float duration = 3.0f;
        float timer = 0f;
        float fireRate = 0.5f;

        while (timer < duration)
        {
            if (playerTransform != null)
            {
                // 보스 위치 기준 오각형 5발 발사 -> 플레이어 쪽으로
                for (int i = 0; i < 5; i++)
                {
                    // 오각형 꼭지점 위치 계산 (보스 주변 1.5 거리)
                    float angle = i * 72f; // 360 / 5 = 72도
                    Vector2 spawnOffset = Quaternion.Euler(0, 0, angle) * Vector2.up * 1.5f;
                    Vector2 spawnPos = (Vector2)transform.position + spawnOffset;

                    // 플레이어 방향 계산
                    Vector2 targetDir = (playerTransform.position - (Vector3)spawnPos).normalized;
                    CreateBullet(pentagonBulletPrefab, spawnPos, targetDir, 8f);
                }
            }
            yield return new WaitForSeconds(fireRate);
            timer += fireRate;
        }

        // 거친 미래가 끝날 때쯤 같이 종료 (대략적인 시간 맞춤)
        yield return new WaitForSeconds(1.0f);
        onSkillEndCallback?.Invoke();
    }

    // 5. 왜곡된 도형: 플레이어 주위에 느린 오각형 탄막 랜덤 생성
    private IEnumerator Skill_DistortedShape()
    {
        Debug.Log("2페이즈: 왜곡된 도형");

        float duration = 3.0f; // 지속시간 임의 설정
        float endTime = Time.time + duration;

        while (Time.time < endTime)
        {
            if (playerTransform != null)
            {
                // 플레이어 주변 랜덤 위치 (반경 2~5 사이)
                Vector2 randomOffset = Random.insideUnitCircle.normalized * Random.Range(2f, 5f);
                Vector2 spawnPos = (Vector2)playerTransform.position + randomOffset;

                // 플레이어를 향해 아주 느리게 다가옴
                Vector2 dir = (playerTransform.position - (Vector3)spawnPos).normalized;
                CreateBullet(pentagonBulletPrefab, spawnPos, dir, 2f); // 속도 2 (느림)
            }
            yield return new WaitForSeconds(0.2f); // 0.2초마다 생성
        }

        yield return new WaitForSeconds(skillCoolDown);
        onSkillEndCallback?.Invoke();
    }

    // 6. 카오스 폴리곤: 주위에 오각형 형태로 생성 후 방사형 터짐
    private IEnumerator Skill_ChaosPolygon()
    {
        Debug.Log("2페이즈: 카오스 폴리곤");

        // 3번 반복
        for (int k = 0; k < 3; k++)
        {
            Vector2 center = GetRandomScreenPos();
            float radius = 2.0f;
            int bulletsPerSide = 5; // 변당 총알 수

            // 오각형 그리기 (5개의 변)
            for (int i = 0; i < 5; i++)
            {
                float angleCurrent = i * 72f * Mathf.Deg2Rad;
                float angleNext = (i + 1) * 72f * Mathf.Deg2Rad;

                Vector2 p1 = center + new Vector2(Mathf.Cos(angleCurrent), Mathf.Sin(angleCurrent)) * radius;
                Vector2 p2 = center + new Vector2(Mathf.Cos(angleNext), Mathf.Sin(angleNext)) * radius;

                // 두 꼭지점 사이를 채움 (선 긋기)
                for (int j = 0; j < bulletsPerSide; j++)
                {
                    float t = (float)j / bulletsPerSide;
                    Vector2 spawnPos = Vector2.Lerp(p1, p2, t);

                    // 일단 멈춰있는 상태로 생성 (속도 0) -> 나중에 움직이게 하려면 투사체 스크립트 수정 필요
                    // 여기서는 생성 즉시 바깥으로 퍼지게 구현
                    Vector2 dir = (spawnPos - center).normalized;
                    CreateBullet(pentagonBulletPrefab, spawnPos, dir, 5f);
                }
                yield return new WaitForSeconds(0.05f); // 그려지는 연출
            }
            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(skillCoolDown);
        onSkillEndCallback?.Invoke();
    }

    // 7. 침식하는 파동: 보스 중심으로 오각형 파동 5번
    private IEnumerator Skill_ErodingWave()
    {
        Debug.Log("2페이즈: 침식하는 파동");

        for (int wave = 0; wave < 5; wave++)
        {
            // 오각형 형태로 전방위 발사 (N-Way인데 각도를 조절해서 오각형 모양 유지)
            // 간단한 방법: 5방향으로 쏘되, 각 방향마다 부채꼴로 3발씩 쏴서 뭉툭한 오각형 느낌 내기
            float startAngle = wave * 15f; // 파동마다 약간 회전

            for (int i = 0; i < 5; i++) // 5각
            {
                float baseAngle = startAngle + (i * 72f);
                // 한 꼭지점에서 3발 부채꼴
                for (int j = -1; j <= 1; j++)
                {
                    float angle = baseAngle + (j * 5f); // 5도 간격
                    Vector2 dir = Quaternion.Euler(0, 0, angle) * Vector2.right;
                    CreateBullet(pentagonBulletPrefab, transform.position, dir, 7f);
                }
            }
            yield return new WaitForSeconds(0.4f);
        }

        yield return new WaitForSeconds(skillCoolDown);
        onSkillEndCallback?.Invoke();
    }

    // 8. 덮인 미래: 오각형 파동 8번 + 무작위 방사 (8초)
    private IEnumerator Skill_CoveredFuture()
    {
        Debug.Log("2페이즈: 덮인 미래");

        // 8초 동안 무작위 탄막 뿌리는 코루틴 별도 실행
        Coroutine randomSpray = StartCoroutine(Routine_RandomSpray(8.0f));

        // 메인: 오각형 파동 8번
        for (int wave = 0; wave < 8; wave++)
        {
            // 침식하는 파동보다 좀 더 촘촘하게
            float startAngle = wave * 10f;
            for (int i = 0; i < 5; i++)
            {
                float baseAngle = startAngle + (i * 72f);
                // 꼭지점마다 5발
                for (int j = -2; j <= 2; j++)
                {
                    float angle = baseAngle + (j * 8f);
                    Vector2 dir = Quaternion.Euler(0, 0, angle) * Vector2.right;
                    CreateBullet(pentagonBulletPrefab, transform.position, dir, 6f);
                }
            }
            yield return new WaitForSeconds(0.8f); // 8번을 8초동안 하려면 대략 1초 간격
        }

        yield return randomSpray; // 끝날 때까지 대기
        onSkillEndCallback?.Invoke();
    }

    private IEnumerator Routine_RoughFuture()
    {
        Vector2 startPos = transform.position;
        // 가로 5줄
        for (int i = 0; i < 5; i++)
        {
            float yOffset = 2f - i * 1.0f;
            for (int x = -2; x <= 2; x++)
            {
                Vector2 spawnPos = startPos + new Vector2(x * 1.5f, yOffset);
                Vector2 dir = (spawnPos - startPos).normalized;
                if (dir == Vector2.zero) dir = Vector2.up;
                CreateBullet(triangleBulletPrefab, spawnPos, dir, 5f);
            }
            yield return new WaitForSeconds(0.2f);
        }
        // 세로 7줄
        for (int i = 0; i < 7; i++)
        {
            float xOffset = -3f + i * 1.0f;
            for (int y = -3; y <= 3; y++)
            {
                Vector2 spawnPos = startPos + new Vector2(xOffset, y * 1.0f);
                Vector2 dir = (spawnPos - startPos).normalized;
                CreateBullet(triangleBulletPrefab, spawnPos, dir, 5f);
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    // 랜덤 스프레이 (지속시간 동안 랜덤 발사)
    private IEnumerator Routine_RandomSpray(float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            Vector2 dir = Random.insideUnitCircle.normalized;
            CreateBullet(pentagonBulletPrefab, transform.position, dir, 5f);
            yield return new WaitForSeconds(0.1f);
            timer += 0.1f;
        }
    }

    private void FireNWay(GameObject prefab, Vector2 origin, int count, float speed)
    {
        float angleStep = 360f / count;
        for (int j = 0; j < count; j++)
        {
            float angle = j * angleStep;
            Vector2 dir = Quaternion.Euler(0, 0, angle) * Vector2.right;
            CreateBullet(prefab, origin, dir, speed);
        }
    }

    private Vector2 GetRandomScreenPos()
    {
        // 화면 안쪽 안전한 랜덤 위치
        float randX = Random.Range(-7f, 7f);
        float randY = Random.Range(-4f, 4f);
        return new Vector2(randX, randY);
    }

    private void CreateBullet(GameObject prefab, Vector2 pos, Vector2 dir, float speed)
    {
        if (prefab == null) return;

        // [수정] Instantiate -> ObjectPoolManager.Instance.Spawn 사용
        GameObject go = ObjectPoolManager.Instance.Spawn(prefab, pos, Quaternion.identity);

        EnemyPojectile p = go.GetComponent<EnemyPojectile>();
        if (p != null)
        {
            // 초기화 로직은 그대로
            p.Initialize(dir, 10, speed);
        }
    }
}
