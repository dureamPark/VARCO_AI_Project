using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StaffRollManager : MonoBehaviour
{
    [System.Serializable]
    public struct StaffWave
    {
        public GameObject enemyPrefab; // 스탭롤 적 (이름표 등)
        public float nextSpawnDelay;   // 다음 녀석 나올 때까지 대기 시간
    }

    [Header("References")]
    public EnemySpawner spawner; // [필수] EnemySpawner 연결!

    [Header("Settings")]
    [SerializeField] private List<StaffWave> staffWaves;
    [SerializeField] private GameObject finalBossPrefab;
    [SerializeField] private GameObject endPanel;

    private int activeEnemyCount = 0;

    private void Start()
    {
        if (endPanel != null) endPanel.SetActive(false);
        AudioEvents.TriggerPlayBGM("VortexStance");

        StartCoroutine(StaffRollRoutine());
    }

    IEnumerator StaffRollRoutine()
    {
        // 1. 스탭롤 순차 소환 (EnemySpawner 이용)
        foreach (var wave in staffWaves)
        {
            SpawnStaff(wave.enemyPrefab); // 함수 호출
            yield return new WaitForSeconds(wave.nextSpawnDelay);
        }

        Debug.Log("모든 스탭롤 소환 완료. 전멸 대기 중...");

        // 2. 남은 적이 0이 될 때까지 대기
        while (activeEnemyCount > 0)
        {
            yield return new WaitForSeconds(0.5f);
        }

        Debug.Log("최종 보스 소환");
        yield return new WaitForSeconds(1.0f);

        // 3. 최종 보스 소환
        SpawnFinalBoss();
    }

    // 일반 스탭 적 소환
    void SpawnStaff(GameObject prefab)
    {
        if (spawner == null || prefab == null) return;

        // [핵심] 스포너에게 프리팹을 주고 생성을 위임함
        // (그러면 스포너가 알아서 SpawnPoint에 만들고 StartPoint로 이동시켜 줌)
        GameObject enemy = spawner.SpawnDirectly(prefab);

        if (enemy != null)
        {
            activeEnemyCount++;
            EnemyStats stats = enemy.GetComponent<EnemyStats>();
            if (stats != null)
            {
                // 적이 죽으면 카운트 감소
                stats.OnDead += () => { activeEnemyCount--; };
            }
        }
    }

    // 최종 보스 소환
    void SpawnFinalBoss()
    {
        if (spawner == null || finalBossPrefab == null) return;

        // 보스도 스포너를 통해 등장 (연출 필요하면)
        GameObject boss = spawner.SpawnDirectly(finalBossPrefab);

        if (boss != null)
        {
            EnemyStats stats = boss.GetComponent<EnemyStats>();
            if (stats != null)
            {
                stats.OnDead += OnBossDead;
            }
        }
    }

    void OnBossDead()
    {
        StartCoroutine(EndGameRoutine());
    }

    IEnumerator EndGameRoutine()
    {
        yield return new WaitForSeconds(2.0f);
        if (endPanel != null) endPanel.SetActive(true);
        yield return new WaitForSeconds(3.0f);
        SceneManager.LoadScene("Title");
    }
}