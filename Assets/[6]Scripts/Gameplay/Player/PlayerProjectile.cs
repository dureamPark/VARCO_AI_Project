using UnityEngine;
using System.Collections;

public class PlayerProjectile : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    private int damage;

    private GameObject originPrefab;
    public void Initialize(Vector2 direction, int damageValue)
    {
        this.damage = damageValue;

        GetComponent<Rigidbody2D>().linearVelocity = direction * moveSpeed;

        StopAllCoroutines();
        StartCoroutine(AutoDisableRoutine(3f));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<EnemyStats>().TakeDamage(damage);
            Destroy(gameObject); // 총알 삭제
        }
    }

    public void SetOriginPrefab(GameObject prefab)
    {
        originPrefab = prefab;
    }

    IEnumerator AutoDisableRoutine(float time)
    {
        yield return new WaitForSeconds(time);
        ReturnToPool();
    }

    // 풀 매니저에게 돌아가기
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