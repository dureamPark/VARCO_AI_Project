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

        GameObject obj = null;

        // [수정된 부분] 1-2. 창고에서 '살아있는' 녀석이 나올 때까지 꺼내봄
        while (poolDictionary[prefab].Count > 0)
        {
            obj = poolDictionary[prefab].Dequeue();

            // 꺼냈는데 유효한(파괴되지 않은) 오브젝트라면 루프 탈출
            if (obj != null)
            {
                break;
            }
            // obj가 null이면(이미 Destroy됨), 그냥 버리고 다음 거 뽑으러 루프 다시 돔
        }

        // 1-3. 창고가 비었거나, 꺼낸 것들이 다 죽어서 obj가 여전히 null이라면 -> 새로 생성
        if (obj == null)
        {
            GameObject newObj = Instantiate(prefab);

            // 컴포넌트 세팅 (Enemy, Warning, Player 각각 확인)
            EnemyPojectile p = newObj.GetComponent<EnemyPojectile>();
            if (p != null) p.SetOriginPrefab(prefab);

            WarningEffect w = newObj.GetComponent<WarningEffect>();
            if (w != null) w.SetOriginPrefab(prefab);

            PlayerProjectile pp = newObj.GetComponent<PlayerProjectile>();
            if (pp != null) pp.SetOriginPrefab(prefab);

            obj = newObj; // 새로 만든 걸 obj에 할당
        }

        // 1-4. 위치 잡고 활성화
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
