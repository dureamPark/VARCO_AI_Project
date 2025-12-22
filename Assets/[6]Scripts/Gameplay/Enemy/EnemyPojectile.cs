using UnityEngine;
using System.Collections;

public class EnemyPojectile : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    private int damage = 1;

    private GameObject originPrefab;

    public void SetOriginPrefab(GameObject prefab)
    {
        originPrefab = prefab;
    }

    public void Initialize(Vector2 direction, int damageValue, float speed = -1f)
    {
        this.damage = damageValue;
        if (speed > 0) this.moveSpeed = speed;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        // 이동 및 회전
        rb.linearVelocity = direction * moveSpeed;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // 재사용을 위해 기존 코루틴 정리 후 다시 시작
        StopAllCoroutines();
        StartCoroutine(AutoDisableRoutine(5f));
    }

    // 풀에서 꺼내질 때마다 실행 (초기화)
    private void OnEnable()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log($"플레이어 피격! 데미지: {damage}");
            ReturnToPool(); // 반납
        }
    }

    IEnumerator AutoDisableRoutine(float time)
    {
        yield return new WaitForSeconds(time);
        ReturnToPool();
    }

    // 풀 매니저에게 반납하는 로직
    private void ReturnToPool()
    {
        if (ObjectPoolManager.Instance != null && originPrefab != null)
        {
            ObjectPoolManager.Instance.ReturnToPool(this.gameObject, originPrefab);
        }
        else
        {
            // 예외 상황: 그냥 파괴
            Destroy(gameObject);
        }
    }
}