using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    [System.Serializable]
    public struct SkillSoundData
    {
        public string skillName;
        public AudioClip clip;
    }

    [Header("Settings")]
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private List<SkillSoundData> soundList;

    private Dictionary<string, AudioClip> soundDict;

    private void Awake()
    {
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
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"[Audio] '{skillName}'에 해당하는 소리가 등록되지 않았습니다!");
        }
    }
}
