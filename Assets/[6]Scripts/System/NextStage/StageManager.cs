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
    
    private GameObject currentEnemy;
    private bool isStageClearConditionMet = false;

    private void Awake() { Instance = this; }

    private void Start()
    {
        currentStage = PlayerPrefs.GetInt("SavedStage", 0);
        StartCoroutine(ProcessStageFlow()); 
        AudioEvents.TriggerPlayBGM("ArenaCall");
    }

    IEnumerator ProcessStageFlow()
    {
        while (true)
        {
            Debug.Log($"🎬 스테이지 {currentStage + 1} 시작");
            isStageClearConditionMet = false;

            // 1. 적 소환
            if (currentStage != 1) 
            {
                if (spawner != null) currentEnemy = spawner.SpawnEnemy(currentStage);
                yield return new WaitForSeconds(2.0f);
            }

            // =============================================================
            // [수정 1] 시작 대화 ID를 엑셀 이름(Dialog_Start_X)과 똑같이 맞춤
            // =============================================================
            string startID = $"Dialog_Start_{currentStage + 1}"; 
            yield return StartCoroutine(PlayDialogueAndWait(startID));


            // 3. 전투 및 조건 감시
            yield return StartCoroutine(MonitorClearCondition());


            // =============================================================
            // [수정 2] 종료 대화 ID를 엑셀 이름(Dialog_End_X)과 똑같이 맞춤
            // =============================================================
            string endID = $"Dialog_End_{currentStage + 1}";
            yield return StartCoroutine(PlayDialogueAndWait(endID));


            // 5. 정리 및 저장
            if (currentStage == 2) yield return StartCoroutine(ExitPentaSequence());

            currentStage++;
            PlayerPrefs.SetInt("SavedStage", currentStage);
            PlayerPrefs.Save();

            yield return new WaitForSeconds(1.0f);
        }
    }

    IEnumerator MonitorClearCondition()
    {
        EnemyStats stats = null;
        if (currentEnemy != null) stats = currentEnemy.GetComponent<EnemyStats>();

        switch (currentStage)
        {
            case 0: // 실드 까기
                if (stats != null)
                {
                    stats.OnShieldBroken += OnConditionMet;
                    yield return new WaitUntil(() => isStageClearConditionMet);
                    stats.OnShieldBroken -= OnConditionMet;
                }
                else isStageClearConditionMet = true;
                break;

            case 1: // 헥사1 죽이기
                if (stats != null)
                {
                    stats.OnDead += OnConditionMet;
                    yield return new WaitUntil(() => isStageClearConditionMet);
                    stats.OnDead -= OnConditionMet;
                }
                break;

            case 2: // 버티기
                float timer = 20f;//잠깐 바꿈
                while (timer > 0)
                {
                    timer -= Time.deltaTime;
                    yield return null;
                }
                break;

            case 3: // 헥사2 죽이기
            case 4: // 펜타 죽이기
                if (stats != null)
                {
                    stats.OnDead += OnConditionMet;
                    yield return new WaitUntil(() => isStageClearConditionMet);
                    stats.OnDead -= OnConditionMet;
                }
                break;
        }
    }

    void OnConditionMet() => isStageClearConditionMet = true;

    //IEnumerator PlayDialogueAndWait(string dialogID)
    //{
    //    bool isFinished = false;
    //    if (StoryManager.Instance != null)
    //    {
    //        Time.timeScale = 0f;
    //        StoryManager.Instance.StartScenario(dialogID, () => { isFinished = true; });
    //        yield return new WaitUntil(() => isFinished);
    //    }
    //    else yield return new WaitForSeconds(0.5f);
    //}
    IEnumerator PlayDialogueAndWait(string dialogID)
    {
        // [수정 1] 게임 시간 정지 (캐릭터, 적, 물리 연산 등 멈춤)
        Time.timeScale = 0f;

        bool isFinished = false;
        if (StoryManager.Instance != null)
        {
            StoryManager.Instance.StartScenario(dialogID, () => { isFinished = true; });

            // WaitUntil은 timeScale이 0이어도 작동합니다. (매 프레임 조건 검사)
            yield return new WaitUntil(() => isFinished);
        }
        else
        {
            // [수정 2] timeScale이 0일 때는 WaitForSeconds는 무한 대기하므로
            // 실제 시간(Realtime)을 기준으로 기다리는 함수를 써야 합니다.
            yield return new WaitForSecondsRealtime(0.5f);
        }

        // [수정 3] 게임 시간 재개
        Time.timeScale = 1f;
    }

    IEnumerator ExitPentaSequence()
    {
        Debug.Log("🚀 펜타 퇴장 시퀀스 시작 (상세 연출)");

        // 1. 현재 적이 존재하는지 확인
        if (currentEnemy != null)
        {
            // [복구] 죽음 이벤트 구독 해제 (중요: 에러 방지)
            EnemyStats stats = currentEnemy.GetComponent<EnemyStats>();
            if (stats != null)
            {
                // OnEnemyDead 함수가 StageManager에 있다고 가정합니다.
                // 만약 에러가 난다면 이 줄은 주석 처리하거나 해당 함수가 있는지 확인하세요.
                // stats.OnDead -= OnEnemyDead; 
                
                stats.SetInvincible(true); // 퇴장 중 무적 설정 
            }

            // [복구] AI 끄기 (공격 멈춤)
            // ※ 프로젝트에 EnemyFSM 스크립트가 있어야 작동합니다.
            // 없으면 에러가 날 수 있으니, 없다면 주석 처리하세요.
             EnemyFSM fsm = currentEnemy.GetComponent<EnemyFSM>();
            if (fsm != null) fsm.enabled = false;
            

            // [복구] 이동 로직 끄기 (제자리 고정 풀기)
            // ※ EnemyMovement 스크립트가 있어야 작동합니다.
            
            EnemyMovement moveScript = currentEnemy.GetComponent<EnemyMovement>();
            if (moveScript != null)
            {
                moveScript.StopMove(); 
                moveScript.enabled = false; 
            }
            

            // [복구] 물리 충돌 끄기 (플레이어 통과 가능)
            Collider2D col = currentEnemy.GetComponent<Collider2D>();
            if (col != null) col.enabled = false;

            // [복구] 위쪽 화면 밖으로 이동 연출
            Vector3 startPos = currentEnemy.transform.position;
            Vector3 endPos = new Vector3(0, 6.5f, 0); // 화면 위쪽 목표 지점
            float duration = 2.0f; // 2초 동안 이동
            float elapsed = 0f;

            while (elapsed < duration)
            {
                if (currentEnemy == null) break;
                
                // 부드럽게 위로 이동 (Lerp)
                currentEnemy.transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            // [복구] 완전히 사라지게 파괴
            if (currentEnemy != null) Destroy(currentEnemy);
        }

        // 스토리 대화 등을 위한 대기
        Debug.Log("스토리 대화 진행 중... (Dialog)");
        yield return new WaitForSeconds(2.0f); 
    }
}