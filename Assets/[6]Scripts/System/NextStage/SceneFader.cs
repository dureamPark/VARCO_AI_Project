using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private CanvasGroup canvasGroup; // 투명도 조절용 컴포넌트
    [SerializeField] private float fadeDuration = 1.0f; // 페이드 시간

    private void Start()
    {
        // 씬이 시작되면 자동으로 "검은색 -> 투명" (Fade In) 시작
        StartCoroutine(FadeInRoutine());
    }

    // 화면이 밝아지는 연출 (검은색 1 -> 투명 0)
    private IEnumerator FadeInRoutine()
    {
        if (canvasGroup == null) yield break;

        float timer = 0f;
        canvasGroup.alpha = 1f; // 시작은 완전 검정

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            // 1에서 0으로 줄어듦
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f; // 완전히 투명하게
        canvasGroup.blocksRaycasts = false; // UI 클릭 가능하게 차단 해제
    }

    // 화면이 어두워지는 연출 (투명 0 -> 검은색 1)
    // onComplete: 페이드가 다 끝나면 실행할 함수 (씬 이동 등)
    public void FadeOutAndLoadScene(string sceneName)
    {
        StartCoroutine(FadeOutRoutine(sceneName));
    }

    private IEnumerator FadeOutRoutine(string sceneName)
    {
        if (canvasGroup == null) yield break;

        canvasGroup.blocksRaycasts = true; // 페이드 중 클릭 방지
        float timer = 0f;
        canvasGroup.alpha = 0f; // 시작은 투명

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            // 0에서 1로 늘어남
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f; // 완전 검정

        // 페이드 끝났으니 씬 이동
        SceneManager.LoadScene(sceneName);
    }
}