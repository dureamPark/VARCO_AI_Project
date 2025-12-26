using System.Collections;
using System.Diagnostics;
using UnityEngine.InputSystem;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance; // StageManager 싱글톤 근데 이게 필요한가?

    [Header("References")]
    public EnemySpawner spawner;

    [Header("Status")]
    public int currentStage = 0; // 현재 스테이지 (0부터 시작)

    private GameObject currentEnemy;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // 저장된 스테이지가 있다면 불러오기 (없으면 0 반환)
        currentStage = PlayerPrefs.GetInt("SavedStage", 0);

        UnityEngine.Debug.Log($"스테이지 {currentStage} 부터 시작");

        // 게임 시작 시 적 소환 (StartNextStage의 인자는 delay 시간임)
        StartCoroutine(StartNextStage(0.5f));
        AudioEvents.TriggerPlayBGM("ArenaCall");
    }

    // 적이 죽었을 때 호출될 함수
    public void OnEnemyDead()
    {
        UnityEngine.Debug.Log($"스테이지 {currentStage + 1} 클리어!");
        
        // 다음 스테이지 준비 5초 뒤 스테이지 시작.
        // 만약 대화창 같은게 있고 그 뒤에 다음 스테이지로 넘어간다면
        // 해당 대화가 끝나고선 StartCoroutine(StartNextStage(5.0f)); 호출하도록 설정해야 함.

        if (currentEnemy != null)
        {
            EnemyStats stats = currentEnemy.GetComponent<EnemyStats>();
            if (stats != null)
            {
                stats.OnDead -= OnEnemyDead; // 구독 해제
            }
        }

        currentStage++;
        StartCoroutine(StartNextStage(5.0f));
    }

    public void SurvivalStageEnd()
    {
        UnityEngine.Debug.Log("생존 시간 종료! 펜타 퇴장 시퀀스 시작.");
        StartCoroutine(ExitPentaAndNextStage());
    }

    private IEnumerator ExitPentaAndNextStage()
    {
        // 현재 적(펜타)이 존재하는지 확인
        if (currentEnemy != null)
        {
            // 죽음 이벤트 구독 해제
            EnemyStats stats = currentEnemy.GetComponent<EnemyStats>();
            if (stats != null)
            {
                stats.OnDead -= OnEnemyDead;
                stats.SetInvincible(true); // 퇴장 중 무적 설정 
            }

            // AI 끄기 
            EnemyFSM fsm = currentEnemy.GetComponent<EnemyFSM>();
            if (fsm != null)
            {
                fsm.enabled = false;
            }

            // 이동 로직(EnemyMovement) 끄기
            EnemyMovement moveScript = currentEnemy.GetComponent<EnemyMovement>();
            if (moveScript != null)
            {
                moveScript.StopMove(); // 속도 0으로 초기화
                moveScript.enabled = false; // 스크립트 비활성화
            }

            // 물리 충돌 끄기 
            Collider2D col = currentEnemy.GetComponent<Collider2D>();
            if (col != null)
            {
                col.enabled = false;
            }

            // 위쪽 화면 밖으로 이동 연출
            Vector3 startPos = currentEnemy.transform.position;
            Vector3 endPos = new Vector3(0, 6.5f, 0);
            float duration = 2.0f; // 2초 동안 이동
            float elapsed = 0f;

            while (elapsed < duration)
            {
                if (currentEnemy == null)
                {
                    break;
                }
                
                // 부드럽게 위로 이동
                currentEnemy.transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            // 완전히 사라지게 파괴
            if (currentEnemy != null)
            {
                Destroy(currentEnemy);
            }
        }

        // 스토리 대화 진행 
        UnityEngine.Debug.Log("스토리 대화 진행 중... (Dialog)");
        yield return new WaitForSeconds(2.0f); 

        currentStage++;
        
        // 육각형 소환
        StartCoroutine(StartNextStage(0.5f));
    }

    private void Update()
    {
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            UnityEngine.Debug.Log("[TEST] P키 입력: 서바이벌 모드 강제 종료 시도");

            if (StageManager.Instance != null)
            {
                SurvivalStageEnd();
            }
        }
    }

    IEnumerator StartNextStage(float delay)
    {
        // 잠시 대기
        yield return new WaitForSeconds(delay);

        if(spawner != null)
        {
            currentEnemy = spawner.SpawnEnemy(currentStage);

            if (currentEnemy != null)
            {
                EnemyStats stats = currentEnemy.GetComponent<EnemyStats>();
                if (stats != null)
                {
                    // 구독 연결
                    stats.OnDead += OnEnemyDead;
                }
            }
        }
    }
}