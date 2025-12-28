using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hex2Skills : EnemySkillBase
{
    [Header("Skill Settings")]
    [SerializeField] private float skillCoolDown = 0.0f;

    private EnemyStats stats;
    private SpriteRenderer sr;

    private Color originColor;

    private int lastSkillId = -1;

    private void Awake()
    {
        stats = GetComponent<EnemyStats>();
        sr = GetComponent<SpriteRenderer>();
        originColor = sr.color;
    }
    public override void CastSkill(int phase, System.Action onFinished)
    {
        this.onSkillEndCallback = onFinished;
        if (playerTransform == null) FindPlayer();

        int rnd;

        do { rnd = Random.Range(0, 5); } while (rnd == lastSkillId);
        lastSkillId = rnd;

        switch (rnd)
        {
            case 0: StartCoroutine(Skill_GraceOfGod()); break;
            case 1: StartCoroutine(Skill_DistortedShape()); break;
            case 2: StartCoroutine(Skill_ChaosPolygon()); break;
            case 3: StartCoroutine(Skill_ErodingWave()); break;
            case 4: StartCoroutine(Skill_CoveredFuture()); break;
        }
    }

    private IEnumerator Skill_RoughFuture()
    {
        Debug.Log("스킬: 거친 미래 (사이즈 조절 가능)");

        List<EnemyPojectile> spawnedBullets = new List<EnemyPojectile>();
        Vector2 centerPos = new Vector2(0, 0);

        int rowCount = 5;      // 가로 줄 개수 (높이 결정)
        int colCount = 5;      // 세로 줄 개수 (너비 결정)

        float gapX = 2f;     // 가로 간격 (넓을수록 뚱뚱해짐)
        float gapY = 2f;     // 세로 간격 (넓을수록 길어짐)

        float drawSpeed = 0.1f; // 그려지는 속도

        // [자동 계산] 전체 크기의 절반을 구해서 시작점(왼쪽 위)을 잡음
        float startY = ((rowCount - 1) * gapY) / 2f;
        float startX = ((colCount - 1) * gapX) / 2f;

        // 1. 가로 줄 그리기 (위 -> 아래)
        for (int i = 0; i < rowCount; i++)
        {
            // 현재 줄의 Y 좌표 (위에서부터 아래로 내려옴)
            float currentY = startY - (i * gapY);

            // 한 줄 긋기 (왼쪽 -> 오른쪽)
            for (int j = 0; j < colCount; j++)
            {
                float currentX = -startX + (j * gapX); // 왼쪽 끝에서 시작

                Vector2 spawnPos = centerPos + new Vector2(currentX, currentY);
                Vector2 dir = spawnPos.normalized; // (0,0 기준 방사형)
                if (dir == Vector2.zero) dir = Vector2.up;

                EnemyPojectile p = CreateBulletAndReturn(spawnPos, dir, 0f, 0f, BulletShape.Triangle);
                if (p != null) spawnedBullets.Add(p);

                yield return new WaitForSeconds(drawSpeed);
            }
        }

        yield return new WaitForSeconds(0.5f);

        Debug.Log("발사!");
        foreach (var bullet in spawnedBullets)
        {
            if (bullet != null && bullet.gameObject.activeSelf)
            {
                Vector2 currentDir = bullet.transform.position.normalized;
                if (currentDir == Vector2.zero) currentDir = Vector2.up;
                bullet.Launch(currentDir, 7f);
            }
        }

        spawnedBullets.Clear();
        yield return new WaitForSeconds(skillCoolDown);
        onSkillEndCallback?.Invoke();
    }

    private IEnumerator Skill_GraceOfGod()
    {
        Debug.Log("2페이즈: 신의 은총");
        AnnounceSkill("GraceOfGod");

        // 병렬 실행: 거친 미래 패턴을 시작하고, 동시에 추가 공격을 수행
        StartCoroutine(Skill_RoughFuture());

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
                    CreateBullet(commonBulletPrefab, spawnPos, targetDir, 8f, 0f, BulletShape.Pentagon);
                }
            }
            yield return new WaitForSeconds(fireRate);
            timer += fireRate;
        }

        // 거친 미래가 끝날 때쯤 같이 종료 (대략적인 시간 맞춤)
        yield return new WaitForSeconds(1.0f);
        onSkillEndCallback?.Invoke();
    }

    private IEnumerator Skill_DistortedShape()
    {
        Debug.Log("2페이즈: 왜곡된 도형");
        AnnounceSkill("DistortedShape");

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
                CreateBullet(commonBulletPrefab, spawnPos, dir, 2f, 0f, BulletShape.Pentagon); // 속도 2 (느림)
            }
            yield return new WaitForSeconds(0.1f); //이거 줄이면 탄막 수많아짐
        }

        yield return new WaitForSeconds(skillCoolDown);
        onSkillEndCallback?.Invoke();
    }

    private IEnumerator Skill_ChaosPolygon()
    {
        Debug.Log("2페이즈: 카오스 폴리곤");
        AnnounceSkill("ChaosPolygon");

        List<EnemyPojectile> spawnedBullets = new List<EnemyPojectile>();

        for (int k = 0; k < 3; k++)
        {
            Vector2 center = GetRandomScreenPos();
            float radius = 3.0f;
            int bulletsPerSide = 6;

            for (int i = 0; i < 5; i++)
            {
                float angleCurrent = i * 72f * Mathf.Deg2Rad;
                float angleNext = (i + 1) * 72f * Mathf.Deg2Rad;

                Vector2 p1 = center + new Vector2(Mathf.Cos(angleCurrent), Mathf.Sin(angleCurrent)) * radius;
                Vector2 p2 = center + new Vector2(Mathf.Cos(angleNext), Mathf.Sin(angleNext)) * radius;

                // 변 채우기
                for (int j = 0; j < bulletsPerSide; j++)
                {
                    float t = (float)j / bulletsPerSide;
                    Vector2 spawnPos = Vector2.Lerp(p1, p2, t);

                    EnemyPojectile p = CreateBulletAndReturn(spawnPos, Vector2.zero, 0f, 0f, BulletShape.Pentagon);

                    if (p != null)
                    {
                        Collider2D col = p.GetComponent<Collider2D>();
                        if (col != null) col.enabled = false;

                        spawnedBullets.Add(p);
                    }
                }
                // 변 하나가 그려지는 연출 시간
                yield return new WaitForSeconds(0.05f);
            }

            yield return new WaitForSeconds(0.5f);

            foreach (var bullet in spawnedBullets)
            {
                if (bullet != null && bullet.gameObject.activeSelf)
                {
                    Collider2D col = bullet.GetComponent<Collider2D>();
                    if (col != null) col.enabled = true;

                    Vector2 dir = (bullet.transform.position - (Vector3)center).normalized;
                    if (dir == Vector2.zero) dir = Vector2.up;

                    bullet.Launch(dir, 6f);
                }
            }

            spawnedBullets.Clear();

            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(skillCoolDown);
        onSkillEndCallback?.Invoke();
    }

    private IEnumerator Skill_ErodingWave()
    {
        Debug.Log("2페이즈: 침식하는 파동");
        AnnounceSkill("ErodingWave");

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
                    CreateBullet(commonBulletPrefab, transform.position, dir, 7f, 0f, BulletShape.Pentagon);
                }
            }
            yield return new WaitForSeconds(0.4f);
        }

        yield return new WaitForSeconds(skillCoolDown);
        onSkillEndCallback?.Invoke();
    }

    private IEnumerator Skill_CoveredFuture()
    {
        Debug.Log("2페이즈: 덮인 미래");
        AnnounceSkill("CoveredFuture");

        sr.color = Color.red;
        stats.SetInvincible(true);

        StartCoroutine(Routine_InvincibilityTimer(5.0f));

        Coroutine randomSpray = StartCoroutine(Routine_RandomSpray(8.0f));

        for (int wave = 0; wave < 8; wave++)
        {
            float startAngle = wave * 10f;
            for (int i = 0; i < 5; i++)
            {
                float baseAngle = startAngle + (i * 72f);
                for (int j = -2; j <= 2; j++)
                {
                    float angle = baseAngle + (j * 8f);
                    Vector2 dir = Quaternion.Euler(0, 0, angle) * Vector2.right;
                    CreateBullet(commonBulletPrefab, transform.position, dir, 6f, 0f, BulletShape.Pentagon);
                }
            }

            yield return new WaitForSeconds(0.8f);
        }

        yield return randomSpray;
        onSkillEndCallback?.Invoke();
    }

    private void CreateBullet(GameObject prefab, Vector2 pos, Vector2 dir, float speed, float startDelay, BulletShape shape)
    {
        CreateBulletAndReturn(pos, dir, speed, startDelay, shape);
    }
    private Vector2 GetRandomScreenPos()
    {
        float randX = Random.Range(-7f, 7f);
        float randY = Random.Range(-4f, 4f);
        return new Vector2(randX, randY);
    }

    private void FireNWay(GameObject prefab, Vector2 origin, int count, float speed)
    {
        float angleStep = 360f / count;
        for (int j = 0; j < count; j++)
        {
            float angle = j * angleStep;
            Vector2 dir = Quaternion.Euler(0, 0, angle) * Vector2.right;
            CreateBullet(prefab, origin, dir, speed, 0f, BulletShape.Square);
        }
    }

    private IEnumerator Routine_RandomSpray(float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            Vector2 dir = Random.insideUnitCircle.normalized;
            CreateBullet(commonBulletPrefab, transform.position, dir, 5f, 0f, BulletShape.Pentagon);
            yield return new WaitForSeconds(0.1f);
            timer += 0.1f;
        }
    }

    private IEnumerator Routine_InvincibilityTimer(float duration)
    {
        // 정확히 duration(5초)만큼 대기
        yield return new WaitForSeconds(duration);

        // 무적 해제 및 색상 복구
        // (EnemySkills.cs에 정의된 원래 색상 변수가 prevColor라면 그것을 사용)
        sr.color = originColor;
        stats.SetInvincible(false);
        Debug.Log("무적 자동 해제됨 (5초 경과)");
    }
}
