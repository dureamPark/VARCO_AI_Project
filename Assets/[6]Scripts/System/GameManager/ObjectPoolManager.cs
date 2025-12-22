using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance;

    // 키: 프리팹(원본), 값: 대기 중인 오브젝트들의 큐(창고)
    private Dictionary<GameObject, Queue<GameObject>> poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this.gameObject);
    }

    // 1. 오브젝트 빌리기 (Spawn)
    public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        // 1-1. 이 프리팹 전용 창고가 없으면 새로 만듦
        if (!poolDictionary.ContainsKey(prefab))
        {
            poolDictionary.Add(prefab, new Queue<GameObject>());
        }

        // 1-2. 창고에 남는게 없으면 새로 생성 (Instantiate)
        if (poolDictionary[prefab].Count == 0)
        {
            GameObject newObj = Instantiate(prefab);
            EnemyPojectile p = newObj.GetComponent<EnemyPojectile>();
            if (p != null) p.SetOriginPrefab(prefab);

            newObj.SetActive(false);
            poolDictionary[prefab].Enqueue(newObj);
        }

        // 1-3. 창고에서 하나 꺼내서 활성화
        GameObject obj = poolDictionary[prefab].Dequeue();
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.SetActive(true);

        return obj;
    }

    // 2. 오브젝트 반납하기 (Return)
    public void ReturnToPool(GameObject obj, GameObject prefab)
    {
        obj.SetActive(false); // 비활성화

        // 해당 프리팹 창고에 다시 넣음
        if (poolDictionary.ContainsKey(prefab))
        {
            poolDictionary[prefab].Enqueue(obj);
        }
        else
        {
            // 만약 창고가 없으면(예외 상황) 그냥 파괴
            Destroy(obj);
        }
    }
}
