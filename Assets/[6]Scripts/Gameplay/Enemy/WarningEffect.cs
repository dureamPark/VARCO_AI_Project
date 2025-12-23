using System.Collections;
using UnityEngine;

public class WarningEffect : MonoBehaviour
{
    private SpriteRenderer sr;
    private GameObject originPrefab; // 풀링용 원본

    public void SetOriginPrefab(GameObject prefab) => originPrefab = prefab;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    // 초기화 및 연출 시작
    public void Initialize(float duration, float targetScale)
    {
        transform.localScale = Vector3.zero; // 크기 0에서 시작

        // 투명도 초기화 (반투명 붉은색 추천)
        if (sr != null)
        {
            Color c = sr.color;
            c.a = 0.5f; // 반투명
            sr.color = c;
        }

        StopAllCoroutines();
        StartCoroutine(AnimateRoutine(duration, targetScale));
    }

    IEnumerator AnimateRoutine(float duration, float targetScale)
    {
        float timer = 0f;
        Vector3 startScale = Vector3.zero;
        Vector3 endScale = Vector3.one * targetScale;

        // duration 동안 크기가 점점 커짐 (차오르는 느낌)
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            transform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }

        transform.localScale = endScale;

        // 연출 끝나면 반납
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        if (ObjectPoolManager.Instance != null && originPrefab != null)
            ObjectPoolManager.Instance.ReturnToPool(this.gameObject, originPrefab);
        else
            Destroy(gameObject);
    }
}