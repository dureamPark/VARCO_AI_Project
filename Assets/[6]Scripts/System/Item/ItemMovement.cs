using UnityEngine;

public class ItemMovement : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private float moveSpeed = 2.0f;   // 떨어지는 속도
    [SerializeField]
    private float destroyYPos = -5.8f; // 이 좌표보다 내려가면 삭제

    void Update()
    {
        // 아래로 이동 (매 프레임마다)
        // Vector3.down은 (0, -1, 0)을 의미합니다.
        transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);

        // 화면 밖으로 나갔는지 체크
        if (transform.position.y <= destroyYPos)
        {
            Destroy(gameObject); // 오브젝트 파괴
            Debug.Log("아이템이 화면 밖으로 사라짐");
        }
    }
}