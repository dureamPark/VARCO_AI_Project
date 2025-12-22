using UnityEngine;

public class EnemyPojectile : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    private int damage = 1;

    public void Initialize(Vector2 direction, int damageValue, float speed = -1f)
    {
        this.damage = damageValue;
        if (speed > 0) this.moveSpeed = speed;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        rb.linearVelocity = direction * moveSpeed;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        Destroy(gameObject, 5f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerStats playerStats = collision.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.TakeDamage(damage);
            }

            Debug.Log($"플레이어 피격! 데미지: {damage}");

            Destroy(gameObject); // 총알 삭제
        }
    }
}
