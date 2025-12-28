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
                float timer = 60f;
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

    IEnumerator PlayDialogueAndWait(string dialogID)
    {
        bool isFinished = false;
        if (StoryManager.Instance != null)
        {
            StoryManager.Instance.StartScenario(dialogID, () => { isFinished = true; });
            yield return new WaitUntil(() => isFinished);
        }
        else yield return new WaitForSeconds(0.5f);
    }

    IEnumerator ExitPentaSequence()
    {
        if (currentEnemy != null)
        {
            var stats = currentEnemy.GetComponent<EnemyStats>();
            if(stats) stats.SetInvincible(true);
            
            // AI 끄기 등 추가 가능
            Destroy(currentEnemy, 2.0f);
        }
        yield return new WaitForSeconds(2.0f);
    }
}