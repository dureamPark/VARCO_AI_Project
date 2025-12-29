using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance;

    [Header("References")]
    public EnemySpawner spawner;

    // 0:실드까기, 1:헥사1킬, 2:버티기, 3:헥사2킬, 4:펜타킬
    [Header("Status")]
    public int currentStage = 0;

    [SerializeField]
    private float timer = 20.0f;

    [SerializeField]
    private float shakeTime = 1.0f;
    
    [SerializeField]
    private float shakeMagnitude = 0.3f;
    
    private GameObject currentEnemy;

    // 스테이지 클리어 조건 충족 플래그
    private bool isStageClearConditionMet = false;

    private void Awake() { Instance = this; }

    private void Start()
    {
        currentStage = 0; //PlayerPrefs.GetInt("SavedStage", 0);
        StartCoroutine(ProcessStageFlow()); 
        AudioEvents.TriggerPlayBGM("ArenaCall");
    }

    IEnumerator ProcessStageFlow()
    {
        while (true)
        {
            // 아직 스테이지를 클리어하지 못한 상황에서 false로 초기화
            isStageClearConditionMet = false;

            // enemy 소환
            // 헥사 hp 깎는 스테이지가 아니라면 if문 안으로
            if (currentStage != 1)
            {
                if (spawner != null)
                {
                    currentEnemy = spawner.SpawnEnemy(currentStage);
                }

                if (currentStage == 0) 
                {
                    // 1초 동안, 강도 0.3 정도로 흔들기
                    if (CameraShake.Instance != null)
                    {
                        StartCoroutine(CameraShake.Instance.Shake(shakeTime, shakeMagnitude));
                    }
                }
                yield return new WaitForSeconds(2.0f);
            }

            // 시작 대화 ID를 엑셀 이름(Dialog_Start_X)과 똑같이 맞춤
            string startID = $"Dialog_Start_{currentStage + 1}"; 
            yield return StartCoroutine(PlayDialogueAndWait(startID));

            // 전투 및 조건 감시
            yield return StartCoroutine(MonitorClearCondition());

            // 종료 대화 ID를 엑셀 이름(Dialog_End_X)과 똑같이 맞춤
            string endID = $"Dialog_End_{currentStage + 1}";
            yield return StartCoroutine(PlayDialogueAndWait(endID));

            // 펜타의 공격을 버텨내면 펜타는 퇴장
            if (currentStage == 2)
            {
                yield return StartCoroutine(ExitPentaSequence());
            }

            currentStage++;

            yield return new WaitForSeconds(1.0f);
        }
    }

    IEnumerator MonitorClearCondition()
    {
        EnemyStats stats = null;
        if (currentEnemy != null) stats = currentEnemy.GetComponent<EnemyStats>();

        switch (currentStage)
        {
            case 0: // 헥사 실드 까기
                if (stats != null)
                {
                    stats.OnShieldBroken += OnConditionMet;
                    // isStageClearConditionMet 변수 값이 true가 될 때까지 대기
                    yield return new WaitUntil(() => isStageClearConditionMet);
                    stats.OnShieldBroken -= OnConditionMet;
                }
                else 
                {
                    // 스테이지 클리어 조건 충족 완료
                    isStageClearConditionMet = true;
                } 
                break;

            case 1: // 헥사1 죽이기
                if (stats != null)
                {
                    stats.OnDead += OnConditionMet;
                    yield return new WaitUntil(() => isStageClearConditionMet);
                    stats.OnDead -= OnConditionMet;
                }
                break;

            case 2: // 펜타 공격 버티기
                if(stats != null)
                {
                    // 생성된 펜타는 무적으로 설정
                    stats.SetInvincible(true);
                }
                while (timer > 0)
                {
                    timer -= Time.deltaTime;
                    yield return null;
                }
                break;

            case 3: // 헥사2 죽이기 case 2에서 펜타 퇴장 이후에
                    // 헥사2가 생성되고 실행되기 때문에 해당 부분은 구현할 필요 없음.
            case 4: // 마지막 스테이지인 펜타 죽이기
                if (stats != null)
                {
                    stats.OnDead += OnConditionMet;
                    yield return new WaitUntil(() => isStageClearConditionMet);
                    stats.OnDead -= OnConditionMet;
                }
                break;
        }
    }

    // 람다함수로 처리
    void OnConditionMet() => isStageClearConditionMet = true;

    // 대화 재생
    IEnumerator PlayDialogueAndWait(string dialogID)
    {
        // 게임 물리 연산 정지
        Time.timeScale = 0f;

        bool isFinished = false;

        if (StoryManager.Instance != null)
        {
            // StartScenario 메소드 실행 완료 시
            // 자동으로 isFinished = true; 해당 코드 실행
            StoryManager.Instance.StartScenario(dialogID, () => { isFinished = true; });

            // WaitUntil은 timeScale이 0이어도 작동하므로 문제 없음
            // isFinished가 true가 될 때까지 대기
            yield return new WaitUntil(() => isFinished);
        }
        else
        {
            // timeScale이 0일 때는 WaitForSeconds는 무한 대기하므로
            // 실제 시간을 기준으로 기다리는 함수를 사용
            yield return new WaitForSecondsRealtime(0.5f);
        }

        // 게임 물리 연산 재개
        Time.timeScale = 1f;
    }

    IEnumerator ExitPentaSequence()
    {
        // 현재 enemy가 존재하는지 확인
        if (currentEnemy != null)
        {
            // 죽음 이벤트 비활성화
            EnemyStats stats = currentEnemy.GetComponent<EnemyStats>();
            if (stats != null)
            {
                // 퇴장 시 무적 활성화
                stats.SetInvincible(true);
            }

            // AI 끄기
             EnemyFSM fsm = currentEnemy.GetComponent<EnemyFSM>();
            if (fsm != null)
            {
                fsm.enabled = false;
            }

            // enemy 이동 로직 끄기            
            EnemyMovement moveScript = currentEnemy.GetComponent<EnemyMovement>();
            if (moveScript != null)
            {
                moveScript.StopMove(); 
                moveScript.enabled = false; 
            }
            
            // 물리 충돌 끄기
            Collider2D col = currentEnemy.GetComponent<Collider2D>();
            if (col != null)
            {
                col.enabled = false;
            } 
            
            // 위쪽 화면 밖으로 이동 연출
            Vector3 startPos = currentEnemy.transform.position;
            Vector3 endPos = new Vector3(0, 6.5f, 0); // 화면 위쪽 목표 지점
            float duration = 2.0f; // 2초 동안 이동
            float elapsed = 0f;

            while (elapsed < duration)
            {
                if (currentEnemy == null)
                {
                    break;
                }
                
                // 위로 이동
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

        // 스토리 대화 등을 위한 대기
        yield return new WaitForSeconds(2.0f); 
    }
}