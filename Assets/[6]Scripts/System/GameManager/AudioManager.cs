using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    [System.Serializable]
    public struct SkillSoundData
    {
        public string skillName; // 예: "거친 미래"
        public AudioClip clip;   // 재생할 소리 파일
    }

    [Header("Settings")]
    [SerializeField] private AudioSource audioSource;

    // 인스펙터에서 짝을 지어줄 리스트
    [SerializeField] private List<SkillSoundData> soundList;

    private Dictionary<string, AudioClip> soundDict;

    private void Awake()
    {
        // 리스트를 검색하기 편하게 딕셔너리로 변환
        soundDict = new Dictionary<string, AudioClip>();
        foreach (var data in soundList)
        {
            if (!soundDict.ContainsKey(data.skillName))
            {
                soundDict.Add(data.skillName, data.clip);
            }
        }
    }

    private void OnEnable()
    {
        // UI랑 똑같은 이벤트를 구독! (서로 간섭 X)
        EnemySkillBase.OnBossSkillCast += PlaySkillSound;
    }

    private void OnDisable()
    {
        EnemySkillBase.OnBossSkillCast -= PlaySkillSound;
    }

    private void PlaySkillSound(string skillName)
    {
        if (audioSource == null) return;

        // 딕셔너리에서 이름으로 소리 찾기
        if (soundDict.TryGetValue(skillName, out AudioClip clip))
        {
            // PlayOneShot: 소리가 겹쳐도 끊기지 않고 자연스럽게 섞임
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"[Audio] '{skillName}'에 해당하는 소리가 등록되지 않았습니다!");
        }
    }
}
