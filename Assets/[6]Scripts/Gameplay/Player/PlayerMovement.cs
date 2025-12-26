using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private float moveSpeed = 12f; // 속도 테스트 후 수정 필요

    [Header("Movement Boundaries")]
    public float minX = -6.3f;
    public float maxX = 6.3f;
    public float minY = -5.6f;
    public float maxY = 5.3f;

    public float MoveSpeed => moveSpeed;

    public void Initialize(Rigidbody2D rigidbody)
    {
        rb = rigidbody;
    }

    public void Move(Vector2 direction)
    {
        rb.linearVelocity = direction.normalized * moveSpeed;
    }

    // 맵 안에 가두는 로직 (좌표 밖으로 이동하면 강제로 고정)
    private void LateUpdate()
    {
        Vector3 currentPos = transform.position;

        currentPos.x = Mathf.Clamp(currentPos.x, minX, maxX);
        currentPos.y = Mathf.Clamp(currentPos.y, minY, maxY);

        transform.position = currentPos;
    }

    // 가속 스킬 속도 설정
    public void SetSpeed(float newSpeed)
    {
        moveSpeed = newSpeed;
    }
}