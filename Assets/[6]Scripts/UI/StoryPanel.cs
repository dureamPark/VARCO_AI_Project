using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // TextMeshPro 필수
using System;

public class StoryPanel : MonoBehaviour
{
    [Header("UI 연결")]
    public TextMeshProUGUI speakerText; // 화자 이름
    public TextMeshProUGUI contentText; // 대사 내용
    public GameObject dialogPanel;      // 패널 (껏다 켰다 용)

    // [핵심] CSV 대신 사용할 데이터 구조
    [System.Serializable]
    public class DialogueLine
    {
        public string speaker; // 말하는 사람
        [TextArea(3, 10)]      // 인스펙터에서 입력창 크게 보기
        public string content; // 내용
    }

    [System.Serializable]
    public class DialogueGroup
    {
        public string groupID; // 예: "Start_1", "Intro" 등 내가 정하는 이름
        public List<DialogueLine> lines; // 대사 리스트
    }

    // 인스펙터에서 이 리스트에 대사를 직접 추가하면 됩니다!
    [Header("대사 데이터 입력")]
    public List<DialogueGroup> storyData = new List<DialogueGroup>();

    private Queue<DialogueLine> currentQueue = new Queue<DialogueLine>();
    private Action onEndCallback;

    private void Awake()
    {
        dialogPanel.SetActive(false); // 시작할 때 꺼둠
    }

    // StageManager가 호출하는 함수
    public void StartDialogue(string id, Action callback)
    {
        // 리스트에서 ID가 같은 그룹을 찾음
        DialogueGroup foundGroup = storyData.Find(x => x.groupID == id);

        if (foundGroup == null)
        {
            Debug.LogWarning($"[StoryPanel] ID '{id}'에 해당하는 대사가 없습니다! 오타 확인 바람.");
            callback?.Invoke(); // 없으면 바로 넘김
            return;
        }

        dialogPanel.SetActive(true);
        onEndCallback = callback;

        // 큐에 대사 담기
        currentQueue.Clear();
        foreach (var line in foundGroup.lines)
        {
            currentQueue.Enqueue(line);
        }

        ShowNext();
    }

    public void ShowNext()
    {
        if (currentQueue.Count > 0)
        {
            DialogueLine line = currentQueue.Dequeue();
            speakerText.text = line.speaker;
            contentText.text = line.content;
        }
        else
        {
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        dialogPanel.SetActive(false);
        onEndCallback?.Invoke(); // StageManager에게 "끝났다"고 보고
        onEndCallback = null;
    }

}