using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hex1Skills : EnemySkillBase
{
    [Header("Skill Settings")]
    [SerializeField] private float skillCoolDown = 0.0f;

    private EnemyStats stats;
    private SpriteRenderer sr;

    private int lastSkillId = -1;

    private void Awake()
    {
        stats = GetComponent<EnemyStats>();
        sr = GetComponent<SpriteRenderer>();
    }
    public override void CastSkill(int phase, System.Action onFinished)
    {
        this.onSkillEndCallback = onFinished;
        if (playerTransform == null) FindPlayer();

        int rnd;

        do { rnd = Random.Range(0, 3); } while (rnd == lastSkillId);
        lastSkillId = rnd;

        switch (rnd)
        {
            case 0: StartCoroutine(Skill_RoughFuture()); break;
            case 1: StartCoroutine(Skill_TrustOfBelief()); break;
            case 2: StartCoroutine(Skill_PrisonOfFreedom()); break;
        }
    }

    private IEnumerator Skill_RoughFuture()
    {
        Debug.Log("스킬: 거친 미래 (사이즈 조절 가능)");
        AnnounceSkill("RoughFuture");

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

    private IEnumerator Skill_TrustOfBelief()
    {
        AnnounceSkill("TrustOfBelief");
        Debug.Log("1페이즈: 믿음의 신뢰");
        int explosionCount = 4;
        for (int i = 0; i < explosionCount; i++)
        {
            Vector2 origin = GetRandomScreenPos();
            ShowWarning(origin, 0.5f, 2.0f);

            yield return new WaitForSeconds(0.5f);
            FireNWay(commonBulletPrefab, origin, 12, 4f);
        }
        yield return new WaitForSeconds(skillCoolDown);
        onSkillEndCallback?.Invoke();
    }

    private IEnumerator Skill_PrisonOfFreedom()
    {
        Debug.Log("1페이즈: 자유의 감옥");
        AnnounceSkill("PrisonOfFreedom");
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
                CreateBullet(commonBulletPrefab, spawnPos, dir, 3f, 0f, BulletShape.Circle);
            }
            yield return new WaitForSeconds(2.0f);
        }
        yield return new WaitForSeconds(skillCoolDown);
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
}
