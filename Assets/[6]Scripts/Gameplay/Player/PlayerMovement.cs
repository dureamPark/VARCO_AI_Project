using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private float moveSpeed = 7.5f; // 속도 테스트 후 수정 필요

    public float MoveSpeed => moveSpeed;

    public void Initialize(Rigidbody2D rigidbody)
    {
        rb = rigidbody;
    }

    public void Move(Vector2 direction)
    {
        rb.linearVelocity = direction.normalized * moveSpeed;
    }

    private void LateUpdate()
    {
        Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);


        viewPos.x = Mathf.Clamp(viewPos.x, 0.22f, 0.78f);
        viewPos.y = Mathf.Clamp(viewPos.y, 0.065f, 0.93f);

        transform.position = Camera.main.ViewportToWorldPoint(viewPos);
    }

    // 가속 스킬 속도 설정
    public void SetSpeed(float newSpeed)
    {
        moveSpeed = newSpeed;
    }
}