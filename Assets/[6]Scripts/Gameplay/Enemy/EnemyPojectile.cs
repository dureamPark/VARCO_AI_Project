using UnityEngine;
using System.Collections;
public enum BulletShape
{
    Triangle, // 0
    Square,   // 1
    Circle,   // 2
    Pentagon  // 3
}
public class EnemyPojectile : MonoBehaviour
{
    [Header("Sprite Settings")]
    [SerializeField] private SpriteRenderer sr;
    // 인스펙터에서 0:삼각형, 1:사각형, 2:원, 3:오각형 순서로 이미지를 넣어주세요
    [SerializeField] private Sprite[] shapeSprites;

    [SerializeField] private float moveSpeed = 10f;
    private int damage = 1;

    // 오브젝트 풀링용 원본 프리팹 저장
    private GameObject originPrefab;

    // 풀 매니저용 설정 함수
    public void SetOriginPrefab(GameObject prefab)
    {
        originPrefab = prefab;
    }

    // [수정됨] startDelay 파라미터 추가 (기본값 0f)
    public void Initialize(Vector2 direction, int damageValue, float speed, float startDelay, BulletShape shape)
    {
        int shapeIndex = (int)shape;
        if (sr != null && shapeSprites != null && shapeIndex < shapeSprites.Length)
        {
            sr.sprite = shapeSprites[shapeIndex];
        }

        this.damage = damageValue;
        if (speed > 0) this.moveSpeed = speed;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        // 1. 방향 회전은 즉시 적용 (그래야 멈춰있을 때도 올바른 곳을 바라봄)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // 이전 코루틴 정리 (재사용 시 필수)
        StopAllCoroutines();

        // 2. 딜레이 여부에 따른 이동 처리
        if (startDelay > 0f)
        {
            rb.linearVelocity = Vector2.zero; // 일단 정지
            StartCoroutine(DelayMoveRoutine(direction, this.moveSpeed, startDelay));
        }
        else
        {
            rb.linearVelocity = direction * this.moveSpeed; // 즉시 출발
        }

        // 3. 수명 관리 (딜레이 시간만큼 수명도 길어져야 함)
        StartCoroutine(AutoDisableRoutine(5f + startDelay));
    }

    private void OnEnable()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Debug.Log($"플레이어 피격! 데미지: {damage}");
            ReturnToPool();
        }
        else if (collision.CompareTag("Wall"))
        {
            ReturnToPool();
        }
    }

    IEnumerator DelayMoveRoutine(Vector2 dir, float spd, float delay)
    {
        // 지정된 시간만큼 대기
        yield return new WaitForSeconds(delay);

        // 출발!
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = dir * spd;
        }
    }

    IEnumerator AutoDisableRoutine(float time)
    {
        yield return new WaitForSeconds(time);
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        if (ObjectPoolManager.Instance != null && originPrefab != null)
        {
            ObjectPoolManager.Instance.ReturnToPool(this.gameObject, originPrefab);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}