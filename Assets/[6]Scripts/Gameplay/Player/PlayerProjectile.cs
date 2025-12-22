using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    private int damage;

    public void Initialize(Vector2 direction, int damageValue)
    {
        this.damage = damageValue;

        GetComponent<Rigidbody2D>().linearVelocity = direction * moveSpeed;

        // 3초 뒤 자동 삭제 (화면 밖 처리 대용, 테스트 해보고 프레임 떨어지면 오브젝트 풀링으로 수정)
        Destroy(gameObject, 3f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<EnemyStats>().TakeDamage(damage);
            Destroy(gameObject); // 총알 삭제
        }
    }
}