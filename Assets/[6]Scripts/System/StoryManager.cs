using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class StoryManager : MonoBehaviour
{
    public static StoryManager Instance;

    [Header("Components")]
    public StoryPanel storyPanel;  
    public DialogueParser parser;  
    public AudioSource audioSource; 

    private Queue<DialogueData> dialogQueue = new Queue<DialogueData>();
    private Action onDialogueFinished; 
    private bool isDialogueActive = false;

    private void Awake()
    {
        Instance = this;
        if (storyPanel != null) storyPanel.gameObject.SetActive(false);
    }

    public void StartScenario(string groupID, Action onFinished = null)
    {
        List<DialogueData> dataList = parser.GetDialogue(groupID);
        
        if (dataList == null || dataList.Count == 0)
        {
            // 데이터가 없으면 로그 띄우고 바로 넘김
            Debug.LogWarning($"[StoryManager] '{groupID}'에 해당하는 대화 데이터가 없습니다.");
            onFinished?.Invoke();
            return;
        }

        if (storyPanel != null) storyPanel.gameObject.SetActive(true);

        isDialogueActive = true;
        onDialogueFinished = onFinished;
        
        dialogQueue.Clear();
        foreach (var data in dataList) dialogQueue.Enqueue(data);

        ShowNextLine();
    }

    public void ShowNextLine()
    {
        if (dialogQueue.Count == 0)
        {
            EndScenario();
            return;
        }

        DialogueData currentData = dialogQueue.Dequeue();

        // 텍스트 출력
        if (storyPanel != null)
        {
            storyPanel.nameText.text = currentData.speakerName;
            StopAllCoroutines();
            StartCoroutine(TypeWriterEffect(currentData.content));
        }

        // ★ [수정됨] 오디오 재생 (audioName 사용)
        if (audioSource != null && !string.IsNullOrEmpty(currentData.audioName))
        {
            audioSource.Stop();
            
            // "Resources/Voice/Text_1_1" 파일을 찾음s
            AudioClip clip = Resources.Load<AudioClip>($"Voice/{currentData.audioName}");
            
            if (clip != null) 
            {
                audioSource.PlayOneShot(clip);
            }
            else
            {
                // 파일이 없을 경우 로그 (디버깅용)
                Debug.LogWarning($"오디오 파일 없음: Assets/[3]Resources/Voice/{currentData.audioName}");
            }
        }
    }

    IEnumerator TypeWriterEffect(string text)
    {
        if (storyPanel != null)
        {
            storyPanel.scriptText.text = "";
            foreach (char c in text)
            {
                storyPanel.scriptText.text += c;
                yield return new WaitForSecondsRealtime(0.05f);
            }
        }
    }

    void EndScenario()
    {
        isDialogueActive = false;
        if (storyPanel != null) storyPanel.gameObject.SetActive(false);
        onDialogueFinished?.Invoke();
    }

    private void Update()
    {
        // 1. 대화 중이 아니라면 입력 무시
        if (!isDialogueActive) return;

        // 2. 스페이스바(Space) 입력 체크
        // (키보드가 연결되어 있고, 스페이스바를 이번 프레임에 눌렀는지 확인)
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            ShowNextLine(); // 다음 대사로 넘기기
        }
        
        // 3. (옵션) 마우스 왼쪽 클릭으로도 넘기기 싶다면 추가
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            ShowNextLine();
        }
    }
}