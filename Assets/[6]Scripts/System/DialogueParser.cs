using System.Collections.Generic;
using System.Text;
using UnityEngine;

// 대화 데이터 구조체
[System.Serializable]
public class DialogueData
{
    public string audioName;     // 오디오 파일 이름
    public string groupID;       // 대화 챕터 그룹
    public string speakerName;
    public string content;
    public int order;            // 순서
    
    // dialogID 저장
    public string dialogID;      
}

public class DialogueParser : MonoBehaviour
{
    public TextAsset csvFile; // 데이터테이블 파일 연결
    
    public Dictionary<string, List<DialogueData>> dialogueDictionary = new Dictionary<string, List<DialogueData>>();

    void Awake()
    {
        ParseCSV();
    }

    void ParseCSV()
    {
        if(csvFile == null) return;

        string fileContent = Encoding.UTF8.GetString(csvFile.bytes);
        
        // 엑셀이 가끔 맨 앞에 보이지 않는 문자를 넣을 때가 있어서 제거
        if (fileContent.StartsWith("\uFEFF"))
        {
            fileContent = fileContent.Substring(1);
        }

        // 공백 문자들이 존재한다면 제거해서 내용만 남기고 저장
        string[] lines = fileContent.Split(new char[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);

        // 0, 1, 2번 줄은 헤더이므로 3번 줄부터 시작
        for (int i = 3; i < lines.Length; i++) 
        {
            string line = lines[i];
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] fields = line.Split(',');
            // 접근해야 하는 데이터 길이 확인
            if (fields.Length < 11)
            {
                continue;
            }

            DialogueData data = new DialogueData();
            
            // 오디오 파일 이름은 10번째 인덱스
            data.audioName = fields[10].Trim(); 

            // 나머지 데이터 매핑
            data.dialogID = fields[6].Trim();
            data.groupID = fields[7].Trim();
            int.TryParse(fields[8], out data.order);
            data.speakerName = fields[9].Trim();
            
            // content는 14번째 인덱스
            if (fields.Length > 15)
            {
                data.content = fields[14].Trim();
            }
            else
            {
                data.content = ""; // 내용이 없으면 빈칸 저장
            }

            // 딕셔너리에 추가
            if (!dialogueDictionary.ContainsKey(data.groupID))
            {
                dialogueDictionary.Add(data.groupID, new List<DialogueData>());
            }
            dialogueDictionary[data.groupID].Add(data);
        }
    }

    // 대화 가져오기
    public List<DialogueData> GetDialogue(string groupID)
    {
        if (dialogueDictionary.ContainsKey(groupID))
        {
            var list = dialogueDictionary[groupID];
            list.Sort((a, b) => a.order.CompareTo(b.order));
            return list;
        }
        return null;
    }
}