using UnityEngine;
using System.Collections;

[RequireComponent(typeof(EnemyStats))]
public class StaffRollMovement : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float moveSpeed = 3.0f;
    [SerializeField] private float stopY = -3.0f;
    [SerializeField] private float autoDestroyTime = 5.0f;
    [SerializeField] private float exitYThreshold = -10.0f;

    public bool autoStartTimer = false;

    private bool isStopped = false;
    private bool isExiting = false;
    private EnemyStats stats;

    private void Awake()
    {
        stats = GetComponent<EnemyStats>();
    }

    private void Update()
    {
        if (stats.CurrentHealth <= 0) return;

        if (isExiting)
        {
            transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);

            if (transform.position.y <= exitYThreshold)
            {
                // 이제 진짜 파괴
                KillSelf();
            }
            return;
        }

        // 일반 등장 로직 (기존과 동일)
        if (isStopped) return;

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

        if (autoStartTimer)
        {
            BeginSelfDestructTimer();
        }
    }

    public void BeginSelfDestructTimer()
    {
        if (stats == null || stats.CurrentHealth <= 0) return;
        StartCoroutine(WaitAndStartExitRoutine());
    }

    private IEnumerator WaitAndStartExitRoutine()
    {
        yield return new WaitForSeconds(autoDestroyTime);

        if (stats != null && stats.CurrentHealth > 0)
        {
            isExiting = true;
        }
    }

    private void KillSelf()
    {
        if (stats != null && stats.CurrentHealth > 0)
        {
            stats.TakeDamage(stats.MaxHealth);
        }
    }
}