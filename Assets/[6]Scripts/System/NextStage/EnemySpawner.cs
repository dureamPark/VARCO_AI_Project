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

        // enemy 생성 (화면 밖 spawnPoint에서)
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
}