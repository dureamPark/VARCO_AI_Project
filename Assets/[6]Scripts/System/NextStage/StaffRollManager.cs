using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StaffRollManager : MonoBehaviour
{
    [System.Serializable]
    public struct StaffWave
    {
        public GameObject enemyPrefab;
        public float nextSpawnDelay;
    }

    [Header("Settings")]
    [SerializeField] private float startDelay = 2.0f;
    [SerializeField] private Transform spawnPoint; // 화면 상단 (스폰 위치)
    [SerializeField] private List<StaffWave> staffWaves;
    [SerializeField] private GameObject finalBossPrefab;
    [SerializeField] private GameObject endPanel;

    // [추가] 타이틀 씬 이름 (틀리면 이동 안함)
    [SerializeField] private string titleSceneName = "Title";
    [SerializeField] private SceneFader sceneFader; // 아까 만든 페이더

    private int activeEnemyCount = 0;
    private List<StaffRollMovement> spawnedStaffList = new List<StaffRollMovement>();

    private void Start()
    {
        if (endPanel != null) endPanel.SetActive(false);
        AudioEvents.TriggerPlayBGM("VortexStance"); // 엔딩곡

        StartCoroutine(StaffRollRoutine());
    }

    IEnumerator StaffRollRoutine()
    {
        yield return new WaitForSeconds(startDelay);

        // 1. 스탭롤 순차 소환
        foreach (var wave in staffWaves)
        {
            SpawnStaff(wave.enemyPrefab);
            yield return new WaitForSeconds(wave.nextSpawnDelay);
        }

        Debug.Log("5명 소환 완료. 마지막 스탭이 자리잡을 때까지 잠시 대기...");

        // (옵션) 마지막 5번째 스탭이 내려와서 멈출 때까지 시간을 좀 줍니다.
        yield return new WaitForSeconds(2.0f);

        Debug.Log("모든 스탭롤 타이머 동시 시작!");

        // 2. [핵심] 기억해둔 모든 스탭에게 타이머 시작 명령 내리기
        foreach (var staff in spawnedStaffList)
        {
            // 플레이어가 기다리는 동안 죽였을 수도 있으니 null 체크 필수
            if (staff != null)
            {
                staff.BeginSelfDestructTimer();
            }
        }

        // 3. 전멸 대기 (기존과 동일)
        while (activeEnemyCount > 0)
        {
            yield return new WaitForSeconds(0.5f);
        }

        Debug.Log("최종 보스 소환");
        yield return new WaitForSeconds(1.0f);

        SpawnFinalBoss(); // (이전 답변의 함수 구현 참고)
    }

    // 통합 스폰 함수
    void SpawnStaff(GameObject prefab, bool isFinalBoss = false)
    {
        if (prefab == null) return;

        GameObject obj = Instantiate(prefab, spawnPoint.position, Quaternion.identity);

        activeEnemyCount++;

        StaffRollMovement movement = obj.GetComponent<StaffRollMovement>();
        if (movement != null)
        {
            spawnedStaffList.Add(movement);
            Debug.Log($"리스트 추가됨: {obj.name}. 현재 리스트 크기: {spawnedStaffList.Count}");
        }
        else
        {
            Debug.LogError($"[오류] {obj.name} 프리팹에 StaffRollMovement 스크립트가 없습니다!");
        }

        EnemyStats stats = obj.GetComponent<EnemyStats>();
        if (stats != null)
        {
            stats.OnDead += () => { activeEnemyCount--; };
        }
    }

    void OnBossDead()
    {
        StartCoroutine(EndGameRoutine());
    }

    IEnumerator EndGameRoutine()
    {
        // 펑 터지는 거 감상 시간
        yield return new WaitForSeconds(3.0f);

        // 패널 짠!
        if (endPanel != null) endPanel.SetActive(true);

        // 패널 보여주는 시간
        yield return new WaitForSeconds(4.0f);

        // 페이드 아웃 하며 타이틀로
        if (sceneFader != null)
            sceneFader.FadeOutAndLoadScene(titleSceneName);
        else
            SceneManager.LoadScene(titleSceneName);
    }

    void SpawnFinalBoss()
    {
        if (finalBossPrefab == null) return;

        // 1. 보스 생성 (화면 상단)
        GameObject boss = Instantiate(finalBossPrefab, spawnPoint.position, Quaternion.identity);

        StaffRollMovement movement = boss.GetComponent<StaffRollMovement>();
        if (movement != null)
        {
            movement.autoStartTimer = true;
        }
        else
        {
            Debug.LogWarning("보스 프리팹에 StaffRollMovement가 없습니다!");
        }

        EnemyStats stats = boss.GetComponent<EnemyStats>();
        if (stats != null)
        {
            // 자폭하든 맞아 죽든, 죽으면 OnBossDead 실행 -> 엔딩
            stats.OnDead += OnBossDead;
        }
    }
}