using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Settings")]
    public Transform spawnPoint;  // 스폰 위치
    public Transform startPoint; // 전투 시작 위치

    [Header("Enemy Prefabs")]
    public List<GameObject> enemyPrefabs; // 스테이지별 적 프리팹 리스트

    [Header("Panel")]
    public GameObject enemyPanel; // 적 정보 패널

    private Coroutine currentFlashRoutine;
    // StageManager에서 호출할 함수
    public GameObject SpawnEnemy(int stageIndex)
    {
        UnityEngine.Debug.Log("enemy 스폰 시작");
        // 리스트 범위를 넘지 않게 안전장치
        if (stageIndex >= enemyPrefabs.Count)
        {
            UnityEngine.Debug.Log("안전장치");
            stageIndex = enemyPrefabs.Count - 1;
        }

        GameObject enemyObj = enemyPrefabs[stageIndex];

        if (enemyObj == null) 
        {
            UnityEngine.Debug.Log("enemy null");
            return null;
        }

        if(stageIndex > 4)
        {
            UnityEngine.Debug.Log($"stageIndex 4초과 {stageIndex}");
            return null;
        }

        // enemy 생성 (화면 밖 spawnPoint에서)
        if (stageIndex == 4)
        {
            UnityEngine.Debug.Log("펜타킬 스폰 - 패널 플래시 코루틴 시작");
            StartCoroutine(CoFlashPanel(3.0f));
        }

        GameObject newEnemy = Instantiate(enemyObj, spawnPoint.position, Quaternion.identity);
        
        // enemy에게 startPoint로 이동 명령
        EnemyEntry entryScript = newEnemy.GetComponent<EnemyEntry>();
        
        if (entryScript != null)
        {
            entryScript.StartEntry(startPoint.position);
        }

        UnityEngine.Debug.Log("enemy 스폰 완료");

        return newEnemy;
    }

    public GameObject SpawnDirectly(GameObject prefab)
    {
        if (prefab == null) return null;
        return ProcessSpawn(prefab);
    }
    private GameObject ProcessSpawn(GameObject targetPrefab)
    {
        if (targetPrefab == null)
        {
            return null;
        }

        // 1. 화면 밖(spawnPoint)에서 생성
        GameObject newEnemy = Instantiate(targetPrefab, spawnPoint.position, Quaternion.identity);

        // 2. 등장 연출 스크립트(EnemyEntry) 찾기
        EnemyEntry entryScript = newEnemy.GetComponent<EnemyEntry>();

        if (entryScript != null)
        {
            entryScript.StartEntry(startPoint.position);
        }
        else
        {
            newEnemy.transform.position = startPoint.position;
        }

        return newEnemy;
    }

    IEnumerator CoFlashPanel(float duration)
    {
        UnityEngine.Debug.Log("플래시 코루틴 시작");
        // 시작 시 패널을 켭니다.
        enemyPanel.SetActive(true);

        float elapsed = 0f;
        float flashInterval = 0.2f; // 깜빡이는 속도 (낮을수록 빠름)

        // duration 동안 반복합니다.
        while (elapsed < duration)
        {
            yield return new WaitForSeconds(flashInterval);
            elapsed += flashInterval;

            // 패널이 삭제되었을 경우를 대비한 null 체크
            if (enemyPanel == null) yield break;

            // 현재 상태의 반대로 설정합니다 (켜져 있으면 끄고, 꺼져 있으면 킴)
            enemyPanel.SetActive(!enemyPanel.activeSelf);
        }

        // 시간이 끝나면 패널을 확실하게 끕니다.
        if (enemyPanel != null)
        {
            enemyPanel.SetActive(false);
        }

        UnityEngine.Debug.Log("플래시 코루틴 종료");
        // 코루틴 종료 표시
        currentFlashRoutine = null;
    }
}