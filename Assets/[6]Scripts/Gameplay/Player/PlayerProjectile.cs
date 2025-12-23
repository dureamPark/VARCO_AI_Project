using UnityEngine;
using System.Collections;

public class PlayerProjectile : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 15f;
    [SerializeField] private float rotateSpeed = 300f; // 회전 속도

    private int damage;
    private bool isHoming;
    private Transform target;
    private Rigidbody2D rb;
    private GameObject originPrefab; // [매니저가 넣어줄 명찰]

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // [매니저 연동] ObjectPoolManager가 생성 시 자동 호출함
    public void SetOriginPrefab(GameObject prefab)
    {
        originPrefab = prefab;
    }

    public void Initialize(Vector2 direction, int damageValue, bool isHomingMode)
    {
        this.damage = damageValue;
        this.isHoming = isHomingMode;
        this.target = null;

        rb.linearVelocity = direction * moveSpeed;

        if (isHoming)
        {
            FindClosestTarget(); // 타겟 찾기 시작
        }

        StopAllCoroutines();
        StartCoroutine(AutoDisableRoutine(3f));
    }

    private void FixedUpdate()
    {
        // 유도 로직: 타겟이 있으면 방향 틂
        if (isHoming && target != null && target.gameObject.activeInHierarchy)
        {
            Vector2 direction = (Vector2)target.position - rb.position;
            direction.Normalize();

            float rotateAmount = Vector3.Cross(direction, transform.up).z;
            rb.angularVelocity = -rotateAmount * rotateSpeed;
            rb.linearVelocity = transform.up * moveSpeed;
        }
    }

    private void FindClosestTarget()
    {
        // [주의] 씬에 "Enemy" 태그가 없으면 아무것도 못 찾음 -> 직진함
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float closestDistance = Mathf.Infinity;
        GameObject closestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float dist = Vector2.Distance(transform.position, enemy.transform.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closestEnemy = enemy;
            }
        }

        if (closestEnemy != null) target = closestEnemy.transform;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            // 적 데미지 처리 (EnemyStats가 있다고 가정)
            var enemy = collision.GetComponent<EnemyStats>(); // 없으면 무시
            if (enemy != null) enemy.TakeDamage(damage);

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
        // 물리 초기화
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        transform.rotation = Quaternion.identity;
        isHoming = false;

        // [핵심] 매니저에게 반납
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