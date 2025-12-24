using System.Collections;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance; // StageManager 싱글톤

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
        // 게임 시작 시 첫 번째 적 소환
        StartCoroutine(StartNextStage(0));
        AudioEvents.TriggerPlayBGM("ArenaCall");
    }

    // 적이 죽었을 때 호출될 함수
    public void OnEnemyDead()
    {
        Debug.Log($"스테이지 {currentStage + 1} 클리어!");
        
        // 다음 스테이지 준비 5초 뒤 스테이지 시작.
        // 만약 대화창 같은게 있고 그 뒤에 다음 스테이지로 넘어간다면
        // 해당 대화가 끝나고선 StartCoroutine(StartNextStage(5.0f)); 호출하도록 설정해야 함.

        if (currentEnemy != null)
        {
            EnemyStats stats = currentEnemy.GetComponent<EnemyStats>();
            if (stats != null)
            {
                stats.OnDead -= OnEnemyDead; // 구독 해제 (깔끔한 마무리)
            }
        }

        currentStage++;
        StartCoroutine(StartNextStage(5.0f));
    }

    IEnumerator StartNextStage(float delay)
    {
        // 잠시 대기
        yield return new WaitForSeconds(delay);

        Debug.Log($"스테이지 {currentStage + 1} 시작!");

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