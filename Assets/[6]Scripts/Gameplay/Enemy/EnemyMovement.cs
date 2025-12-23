using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private float moveSpeed = 7f;
    private Vector2 moveDir;
    private bool isMoving = false;
    private bool isBoundsCalculated = false;

    //이동범위 제한용
    [SerializeField] private float paddingX = 4.3f;
    [SerializeField] private float paddingY = 0.8f;
    [SerializeField] private float paddingUpperY = 7f;

    private Vector2 minBounds;
    private Vector2 maxBounds;
    public void Initialize(Rigidbody2D rigidbody)
    {
        rb = rigidbody;
        CalculateScreenBounds();
    }

    public void StartRandomMove()
    {
        // 화면 안 랜덤한 방향 설정 (단순 예시)
        float x = Random.Range(-1f, 1f);
        float y = Random.Range(-1f, 1f);
        moveDir = new Vector2(x, y).normalized;
        isMoving = true;
    }

    public void StopMove()
    {
        isMoving = false;
        rb.linearVelocity = Vector2.zero; // 멈춤
    }

    void FixedUpdate()
    {
        if (isMoving)
        {
            rb.linearVelocity = moveDir * moveSpeed;
            // 화면 밖으로 나가지 않게 하는 로직 추가 필요
        }
    }
    void LateUpdate()
    {
        // 현재 위치 가져오기
        Vector3 viewPos = transform.position;

        // X, Y 좌표를 화면 경계 안으로 가두기 (Clamp)
        // min + padding ~ max - padding 사이로 제한
        viewPos.x = Mathf.Clamp(viewPos.x, minBounds.x + paddingX, maxBounds.x - paddingX);
        viewPos.y = Mathf.Clamp(viewPos.y, minBounds.y + paddingUpperY, maxBounds.y - paddingY);

        // 보정된 위치 적용
        transform.position = viewPos;
    }
    void CalculateScreenBounds()
    {
        // 카메라의 좌측 하단(0,0)과 우측 상단(1,1)을 월드 좌표로 변환
        minBounds = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
        maxBounds = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));
        isBoundsCalculated = true;
    }

    private void OnDrawGizmos()
    {
        if (isBoundsCalculated)
        {
            Gizmos.color = Color.green;
            // 사각형 그리기
            float width = (maxBounds.x - paddingX) - (minBounds.x + paddingX);
            float height = (maxBounds.y - paddingY) - (minBounds.y + paddingUpperY);
            float centerX = (minBounds.x + paddingX + maxBounds.x - paddingX) / 2;
            float centerY = (minBounds.y + paddingUpperY + maxBounds.y - paddingY) / 2;

            Gizmos.DrawWireCube(new Vector3(centerX, centerY, 0), new Vector3(width, height, 0));
        }
    }
}
