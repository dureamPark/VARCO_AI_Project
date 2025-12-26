using System.Collections;
using UnityEngine;

public class EnemyEntry : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private float entrySpeed = 3f; // 등장 속도
    
    // 목표 지점 (스포너가 정해줄 예정)
    private Vector3 targetPosition;
    private bool isEntering = false;
    private EnemyFSM enemyFSM; // 적의 AI (공격/이동 로직)
    private EnemyStats enemyStats; // 적의 무적 설정용

    private void Awake()
    {
        enemyFSM = GetComponent<EnemyFSM>();
        enemyStats = GetComponent<EnemyStats>();
    }

    public void StartEntry(Vector3 destination)
    {
        targetPosition = destination;
        UnityEngine.Debug.Log($"적 등장 시작, 목표 지점: {targetPosition}");
        isEntering = true;

        // 등장 중에는 AI를 끔. (공격 안 하게)
        if (enemyFSM != null) 
        {
            enemyFSM.enabled = false;
        }

        EnemyMovement moveScript = GetComponent<EnemyMovement>();
        
        if (moveScript != null) 
        {
            moveScript.enabled = false; 
        }

        // 등장 중에는 무적 (선택 사항)
        if (enemyStats != null) 
        {
            enemyStats.SetInvincible(true);
        }
    }

    private void Update()
    {
        if (!isEntering)
        {
            return;
        }

        // 목표 지점으로 이동
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, entrySpeed * Time.deltaTime);

        // 도착했는지 체크 (거리가 거의 0이면)
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            FinishEntry();
        }
    }

    private void FinishEntry()
    {
        /*승우가 여기 코드에서 각 enemy hp ui 나오도록 수정하면 돼.*/

        isEntering = false;

        // 도착했으니 AI를 켜서 전투 시작!
        if (enemyFSM != null)
        {
            enemyFSM.enabled = true;
        }
        
        EnemyMovement moveScript = GetComponent<EnemyMovement>();

        if (moveScript != null)
        {
            moveScript.enabled = true; // 이제부터 Move 로직 가동
            moveScript.Initialize(GetComponent<Rigidbody2D>()); // 초기화도 이때 실행
            moveScript.StartRandomMove(); // 랜덤 이동 시작
        }

        if (enemyStats != null)
        {
            enemyStats.SetInvincible(false);
        }
        Debug.Log("적 등장 완료! 전투 개시!");
    }
}