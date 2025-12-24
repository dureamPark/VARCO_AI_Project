using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private float moveSpeed = 12f; // 속도 테스트 후 수정 필요

    public float MoveSpeed => moveSpeed;

    public void Initialize(Rigidbody2D rigidbody)
    {
        rb = rigidbody;
    }

    public void Move(Vector2 direction)
    {
        rb.linearVelocity = direction.normalized * moveSpeed;
    }

    // 가속 스킬 속도 설정
    public void SetSpeed(float newSpeed)
    {
        moveSpeed = newSpeed;
    }
}