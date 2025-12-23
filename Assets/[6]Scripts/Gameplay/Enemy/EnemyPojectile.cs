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

    // [수정 1] Initialize 로직 변경: 속도가 0이면 멈춰있도록 설정
    public void Initialize(Vector2 direction, int damageValue, float speed, float startDelay, BulletShape shape)
    {
        // 1. 데미지 및 속도 설정
        this.damage = damageValue;
        this.moveSpeed = speed; // 0이 들어오면 0으로 저장됨 (정지 상태)

        // 2. 모양(Sprite) 교체
        int shapeIndex = (int)shape;
        if (sr != null && shapeSprites != null && shapeIndex < shapeSprites.Length)
        {
            sr.sprite = shapeSprites[shapeIndex];
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        // 3. 방향 회전 (멈춰있어도 머리는 진행 방향을 보게 함)
        // direction이 (0,0)이 아닐 때만 회전
        if (direction != Vector2.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        // 재사용을 위해 이전 코루틴 정리
        StopAllCoroutines();

        // 4. 움직임 처리
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero; // 일단 리셋

            if (startDelay > 0f)
            {
                // 딜레이가 있으면 대기 후 출발
                StartCoroutine(DelayMoveRoutine(direction, this.moveSpeed, startDelay));
            }
            else if (this.moveSpeed > 0f)
            {
                // 딜레이 없고 속도가 있으면 즉시 출발
                rb.linearVelocity = direction * this.moveSpeed;
            }
            // 만약 moveSpeed가 0이면? -> 그냥 가만히 멈춰있음 (Launch 대기 상태)
        }

        // 5. 수명 관리 (10초 뒤 자동 삭제, 정지 상태가 길어질 수 있으므로 넉넉하게)
        StartCoroutine(AutoDisableRoutine(10f + startDelay));
    }

    // [수정 2] 외부에서 "지금 출발해!" 라고 명령하는 함수 추가
    public void Launch(Vector2 direction, float speed)
    {
        this.moveSpeed = speed;

        // 방향 다시 설정 (발사 때 방향이 바뀔 수도 있으므로)
        if (direction != Vector2.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * speed;
        }
    }

    // 딜레이 후 출발 코루틴
    IEnumerator DelayMoveRoutine(Vector2 dir, float spd, float delay)
    {
        yield return new WaitForSeconds(delay);
        Launch(dir, spd); // Launch 함수를 재활용
    }

    private void OnEnable()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Wall"))
        {
            ReturnToPool();
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