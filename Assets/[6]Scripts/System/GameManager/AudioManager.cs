using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    [System.Serializable]
    public struct AudioData
    {
        public string name;     // 소리 이름 (Key)
        public AudioClip clip;  // 소리 파일
    }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSource; // 효과음용 (OneShot)
    [SerializeField] private AudioSource bgmSource; // 배경음용 (Loop)

    [Header("Data Lists")]
    [SerializeField] private List<AudioData> bgmList; // BGM 목록
    [SerializeField] private List<AudioData> sfxList; // 효과음 목록 (스킬 소리 포함)

    // 빠른 검색을 위한 딕셔너리 2개
    private Dictionary<string, AudioClip> bgmDict = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> sfxDict = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        // BGM 딕셔너리 초기화
        foreach (var data in bgmList)
        {
            if (!bgmDict.ContainsKey(data.name))
                bgmDict.Add(data.name, data.clip);
        }

        // SFX 딕셔너리 초기화
        foreach (var data in sfxList)
        {
            if (!sfxDict.ContainsKey(data.name))
                sfxDict.Add(data.name, data.clip);
        }
    }

    private void OnEnable()
    {
        // 보스 스킬 이벤트 구독
        EnemySkillBase.OnBossSkillCast += PlaySkillSound;
        AudioEvents.OnPlaySFX += PlaySFX;
        AudioEvents.OnPlayBGM += PlayBGM;
        AudioEvents.OnStopBGM += StopBGM;
    }

    private void OnDisable()
    {
        EnemySkillBase.OnBossSkillCast -= PlaySkillSound;
        AudioEvents.OnPlaySFX -= PlaySFX;
        AudioEvents.OnPlayBGM -= PlayBGM;
        AudioEvents.OnStopBGM -= StopBGM;
    }


    // 외부에서 부를 때: AudioManager.Instance.PlaySFX("Jump");
    public void PlaySFX(string sfxName)
    {
        if (sfxSource == null) return;

        if (sfxDict.TryGetValue(sfxName, out AudioClip clip))
        {
            sfxSource.PlayOneShot(clip); // 겹쳐서 재생 가능
        }
        else
        {
            Debug.LogWarning($"[Audio] SFX '{sfxName}'를 찾을 수 없습니다.");
        }
    }

    private void PlaySkillSound(string skillName)
    {
        PlaySFX(skillName);
    }


    // 외부에서 부를 때: AudioManager.Instance.PlayBGM("Stage1");
    public void PlayBGM(string bgmName)
    {
        if (bgmSource == null) return;

        if (bgmDict.TryGetValue(bgmName, out AudioClip clip))
        {
            if (bgmSource.clip == clip && bgmSource.isPlaying) return;

            bgmSource.Stop();       // 이전 곡 정지
            bgmSource.clip = clip;  // 곡 교체
            bgmSource.loop = true;  // 반복 재생 설정
            bgmSource.Play();       // 재생
        }
        else
        {
            Debug.LogWarning($"[Audio] BGM '{bgmName}'를 찾을 수 없습니다.");
        }
    }

    // BGM 끄기
    public void StopBGM()
    {
        if (bgmSource != null) bgmSource.Stop();
    }
}
