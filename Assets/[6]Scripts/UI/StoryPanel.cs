using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI 텍스트 사용 시
using TMPro; // TextMeshPro 사용 시 (추천)
using System;

public class StoryPanel : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI speakerText; // 화자 이름 표시용 UI
    public TextMeshProUGUI contentText; // 대사 내용 표시용 UI
    public GameObject dialogPanel; // 패널 전체 (껏다 켰다 할 대상)

    [Header("CSV Settings")]
    public string csvFileName = "스토리 테이블"; // Resources 폴더 안의 파일 이름 (확장자 제외)

    // 내부 데이터 구조 (대사 하나를 담는 클래스)
    [System.Serializable]
    public class DialogueData
    {
        public string speaker;
        public string content;
    }

    // 모든 대사를 저장할 딕셔너리 (Key: 그룹ID, Value: 대사 리스트)
    private Dictionary<string, List<DialogueData>> dialogueDatabase = new Dictionary<string, List<DialogueData>>();

    // 현재 진행 중인 대사 큐
    private Queue<DialogueData> currentDialogueQueue = new Queue<DialogueData>();

    // 대화가 끝났을 때 실행할 함수 저장소
    private Action onDialogueEnd;

    void Awake()
    {
        // 게임 시작 시 CSV 파일 로드
        LoadCSV();
        dialogPanel.SetActive(false); // 처음에 꺼두기
    }

    // 1. CSV 파일 읽기 (가장 단순화한 파싱 로직)
    void LoadCSV()
    {
        TextAsset csvData = Resources.Load<TextAsset>(csvFileName);
        if (csvData == null)
        {
            Debug.LogError($"Resources 폴더에 {csvFileName} 파일이 없습니다!");
            return;
        }

        string[] lines = csvData.text.Split(new char[] { '\n' });

        // 첫 줄(헤더)은 건너뛰고 1부터 시작
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] fields = line.Split(',');

            // CSV 열 위치에 맞춰 인덱스 조정 필요 (업로드된 파일 기준 추정)
            // 예: GroupID가 5번째(idx 4), 화자가 7번째(idx 6), 대사가 11번째(idx 10) 라고 가정
            // *실제 CSV를 열어서 몇 번째 칸인지 세어보고 아래 숫자를 수정하세요!*
            if (fields.Length < 10) continue;

            string groupID = fields[4].Trim(); // 그룹 ID 위치
            string speaker = fields[7].Trim(); // 화자 위치
            string content = fields[11].Trim(); // 대사 위치 (따옴표 제거 로직 등 필요할 수 있음)

            // 딕셔너리에 추가
            if (!dialogueDatabase.ContainsKey(groupID))
            {
                dialogueDatabase[groupID] = new List<DialogueData>();
            }

            dialogueDatabase[groupID].Add(new DialogueData { speaker = speaker, content = content });
        }
    }

    // 2. 외부(StageManager)에서 대화 시작 요청
    public void StartDialogue(string groupID, Action onEndCallback)
    {
        if (!dialogueDatabase.ContainsKey(groupID))
        {
            Debug.LogWarning($"CSV에 ID [{groupID}]가 없습니다. 바로 넘깁니다.");
            onEndCallback?.Invoke();
            return;
        }

        dialogPanel.SetActive(true);
        onDialogueEnd = onEndCallback;

        // 큐에 대사 채워넣기
        currentDialogueQueue.Clear();
        foreach (var data in dialogueDatabase[groupID])
        {
            currentDialogueQueue.Enqueue(data);
        }

        ShowNextLine();
    }

    // 3. 다음 대사 출력
    public void ShowNextLine()
    {
        if (currentDialogueQueue.Count > 0)
        {
            DialogueData data = currentDialogueQueue.Dequeue();
            speakerText.text = data.speaker;
            contentText.text = data.content.Replace("\"", ""); // CSV 따옴표 제거
        }
        else
        {
            EndDialogue();
        }
    }

    // 4. 대화 종료 처리
    void EndDialogue()
    {
        dialogPanel.SetActive(false);
        onDialogueEnd?.Invoke(); // StageManager에게 알림
        onDialogueEnd = null;
    }

    // 5. 클릭 입력 처리
    void Update()
    {
        // 패널이 켜져있고, 마우스 클릭이나 스페이스바를 누르면
        if (dialogPanel.activeSelf && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)))
        {
            ShowNextLine();
        }
    }
}