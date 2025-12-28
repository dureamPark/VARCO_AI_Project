using UnityEngine;
using System.Collections;

[RequireComponent(typeof(EnemyStats))]
public class StaffRollMovement : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float moveSpeed = 3.0f;
    [SerializeField] private float stopY = -3.0f;
    [SerializeField] private float autoDestroyTime = 5.0f;

    public bool autoStartTimer = false;

    private bool isStopped = false;
    private EnemyStats stats;

    private void Awake()
    {
        stats = GetComponent<EnemyStats>();
    }

    private void Update()
    {
        if (isStopped || stats.CurrentHealth <= 0) return;

        transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);

        if (transform.position.y <= stopY)
        {
            StopMovement();
        }
    }

    private void StopMovement()
    {
        isStopped = true;
        Vector3 pos = transform.position;
        pos.y = stopY;
        transform.position = pos;

        // [수정] 옵션이 켜져 있으면 도착 즉시 타이머 시작!
        if (autoStartTimer)
        {
            BeginSelfDestructTimer();
        }
    }

    public void BeginSelfDestructTimer()
    {
        if (stats == null || stats.CurrentHealth <= 0) return;
        StartCoroutine(SelfDestructRoutine());
    }

    private IEnumerator SelfDestructRoutine()
    {
        // 설정된 시간만큼 대기
        yield return new WaitForSeconds(autoDestroyTime);

        if (stats != null && stats.CurrentHealth > 0)
        {
            // 자폭 (이게 실행되면 OnDead 이벤트가 발생해서 엔딩으로 넘어감)
            stats.TakeDamage(stats.MaxHealth);
        }
    }
}