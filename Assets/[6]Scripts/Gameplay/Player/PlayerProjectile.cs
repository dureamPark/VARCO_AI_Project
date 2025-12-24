using UnityEngine;
using System.Collections;

public class PlayerProjectile : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 15f;
    [SerializeField] private float rotateSpeed = 15000f; 

    private int damage;
    private bool isHoming;
    private Transform target;
    private Rigidbody2D rb;
    private GameObject originPrefab; 

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // ObjectPoolManager가 생성 시 자동 호출함
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
            FindClosestTarget(); 
        }

        StopAllCoroutines();
        StartCoroutine(AutoDisableRoutine(7f));
    }

    private void FixedUpdate()
    {
        // 유도탄 로직
        if (isHoming && target != null && target.gameObject.activeInHierarchy)
        {
            Vector2 direction = (Vector2)target.position - rb.position;
            float distance = direction.magnitude; 
            direction.Normalize();

            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

            float angleDiff = Mathf.DeltaAngle(rb.rotation, targetAngle);

            float nextAngle = Mathf.MoveTowardsAngle(rb.rotation, targetAngle, rotateSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(nextAngle);

            float speedMultiplier = Mathf.Clamp01(1f - Mathf.Abs(angleDiff) / 90f);

            speedMultiplier = Mathf.Max(0.5f, speedMultiplier);

            rb.linearVelocity = transform.up * (moveSpeed * speedMultiplier);
        }
        else
        {
            rb.linearVelocity = transform.up * moveSpeed;
        }
    }

    private void FindClosestTarget()
    {
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
            // 적 데미지 처리 
            var enemy = collision.GetComponent<EnemyStats>(); 
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

        // 매니저에게 반납
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