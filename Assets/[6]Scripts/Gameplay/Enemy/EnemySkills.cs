using System.Collections;
using UnityEngine;

public class EnemySkills : MonoBehaviour
{
    [Header("Skill Settings")]
    [SerializeField] private float skillCoolDown = 2.0f;
    [SerializeField] private float invincibilityDuration = 5.0f;
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
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerTransform = playerObj.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void CastRandomSkill(System.Action onFinished)
    {
        this.onSkillEndCallback = onFinished;

        int randomIdx = Random.Range(0, 3);

        switch (randomIdx)
        {
            case 0: StartCoroutine(Skill_RoughFuture()); break;
            case 1: StartCoroutine(Skill_TrustOfBelief()); break;
            case 2: StartCoroutine(Skill_PrisonOfFreedom()); break;
        }
    }
    private IEnumerator Skill_RoughFuture()
    {
        Debug.Log("스킬: 거친 미래");

        // 화면 중앙 기준 잡기 (혹은 보스 위치)
        Vector2 startPos = transform.position;

        // 1. 가로 5줄 발사 (위에서 아래로 순차적)
        for (int i = 0; i < 5; i++)
        {
            float yOffset = 2f - i * 1.0f; // Y축 간격 조절
                                           // 왼쪽에서 오른쪽으로 훑으며 생성 or 그냥 가로줄 생성
                                           // 여기서는 "방사형으로 퍼져나감"을 위해, 생성 후 각자 바깥쪽으로 날아가게 설정

            // 한 줄에 5개씩 배치
            for (int x = -2; x <= 2; x++)
            {
                Vector2 spawnPos = startPos + new Vector2(x * 1.5f, yOffset);
                Vector2 dir = (spawnPos - startPos).normalized; // 중앙에서 바깥으로
                if (dir == Vector2.zero) dir = Vector2.up; // 중앙인 경우 위로

                CreateBullet(triangleBulletPrefab, spawnPos, dir, 5f);
            }
            yield return new WaitForSeconds(0.2f); // 줄마다 약간의 딜레이
        }

        // 2. 세로 7줄 발사 (왼쪽에서 오른쪽으로)
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

        yield return new WaitForSeconds(skillCoolDown);
        onSkillEndCallback?.Invoke();
    }

    private IEnumerator Skill_TrustOfBelief()
    {
        Debug.Log("스킬: 믿음의 신뢰");

        int explosionCount = 4;

        for (int i = 0; i < explosionCount; i++)
        {
            // 화면 내 랜덤 위치 생성 (하드코딩된 범위, 필요시 Camera Viewport 로직 적용)
            float randX = Random.Range(-7f, 7f);
            float randY = Random.Range(-4f, 4f);
            Vector2 origin = new Vector2(randX, randY);

            // 원형으로 퍼지는 탄막 (N-Way)
            int bulletCount = 12; // 12발
            float angleStep = 360f / bulletCount;

            for (int j = 0; j < bulletCount; j++)
            {
                float angle = j * angleStep;
                // 각도를 벡터로 변환 (Cos, Sin)
                float rad = angle * Mathf.Deg2Rad;
                Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

                CreateBullet(squareBulletPrefab, origin, dir, 6f);
            }

            // 다음 폭발까지 대기
            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(skillCoolDown);
        onSkillEndCallback?.Invoke();
    }

    private IEnumerator Skill_PrisonOfFreedom()
    {
        Debug.Log("스킬: 자유의 감옥");

        int repeatCount = 4;

        for (int k = 0; k < repeatCount; k++)
        {
            if (playerTransform == null) break;

            Vector2 targetPos = playerTransform.position;
            float radius = 4.0f; // 플레이어로부터 거리
            int bulletCount = 16;
            float angleStep = 360f / bulletCount;

            for (int i = 0; i < bulletCount; i++)
            {
                float angle = i * angleStep;
                float rad = angle * Mathf.Deg2Rad;

                // 원형 배치 좌표 계산
                Vector2 spawnPos = targetPos + new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * radius;

                // 방향: 생성 위치에서 -> 플레이어 위치로 (조여오기)
                Vector2 dir = (targetPos - spawnPos).normalized;

                // 속도는 조금 느리게 (조여오는 공포감)
                CreateBullet(circleBulletPrefab, spawnPos, dir, 3f);
            }

            // "2초 주기"라고 했으므로 대기
            yield return new WaitForSeconds(2.0f);
        }

        yield return new WaitForSeconds(skillCoolDown);
        onSkillEndCallback?.Invoke();
    }

    private IEnumerator PenSkill_1()
    {
        Debug.Log("신의 은총");

        yield return new WaitForSeconds(skillCoolDown);
        onSkillEndCallback?.Invoke();
    }

    private IEnumerator PenSkill_2()
    {
        Debug.Log("왜곡된 도형");

        yield return new WaitForSeconds(skillCoolDown);
        onSkillEndCallback?.Invoke();
    }

    private IEnumerator PenSkill_3()
    {
        Debug.Log("카오스 폴리곤");

        yield return new WaitForSeconds(skillCoolDown);
        onSkillEndCallback?.Invoke();
    }

    private IEnumerator PenSkill_4()
    {
        Debug.Log("침식하는 파동");

        yield return new WaitForSeconds(skillCoolDown);
        onSkillEndCallback?.Invoke();
    }

    private IEnumerator PenSkill_5()
    {
        Debug.Log("덮인 미래");
        stats.SetInvincible(true);
        sr.color = Color.red;
        Debug.Log("무적 시작");

        yield return new WaitForSeconds(invincibilityDuration);

        stats.SetInvincible(false);
        sr.color = prevColor;
        Debug.Log("무적 끝");
        yield return new WaitForSeconds(skillCoolDown);
        onSkillEndCallback?.Invoke();
    }

    private void CreateBullet(GameObject prefab, Vector2 pos, Vector2 dir, float speed)
    {
        if (prefab == null) return;
        GameObject go = Instantiate(prefab, pos, Quaternion.identity);
        EnemyPojectile p = go.GetComponent<EnemyPojectile>();
        if (p != null)
        {
            // 데미지는 임의로 10 설정
            p.Initialize(dir, 10, speed);
        }
    }
}
