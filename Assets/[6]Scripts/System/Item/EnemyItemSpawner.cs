using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyStats))]
public class EnemyItemSpawner : MonoBehaviour
{
    [System.Serializable]
    public struct DropEntry
    {
        public string itemName;
        public BaseItem itemPrefab;
        
        [Range(0, 100)]
        public float dropPercent;
    }

    private EnemyItemSpawner enemyItemInstance;

    [Header("Drop Settings")]
    [SerializeField]
    private List<DropEntry> dropTable = new List<DropEntry>();

    private EnemyStats enemyStats;

    private void Awake()
    {
        enemyStats = GetComponent<EnemyStats>();
    }

    private void OnEnable()
    {
        if (enemyStats != null)
        {
            enemyStats.OnTakeDamage += TrySpawnItem;
        }
    }

    private void OnDisable()
    {
        if (enemyStats != null)
        {
            enemyStats.OnTakeDamage -= TrySpawnItem;
        }
    }

    private void TrySpawnItem(int damage)
    {
        if (dropTable.Count == 0)
        {
            return;
        }

        float randomValue = Random.Range(0f, 100f);
        float currentSum = 0f;

        foreach (DropEntry entry in dropTable)
        {
            currentSum += entry.dropPercent;

            if (randomValue <= currentSum)
            {
                if (entry.itemPrefab != null)
                {
                    SpawnItem(entry.itemPrefab);
                    Debug.Log($"스폰 : {entry.itemName}");
                }
                return;
            }
        }
    }

    private void SpawnItem(BaseItem itemToSpawn)
    {
        Instantiate(itemToSpawn.gameObject, transform.position, Quaternion.identity);
    }
}