using System.Collections.Generic;
using System.Text;
using UnityEngine;

// 대화 데이터 구조체
[System.Serializable]
public class DialogueData
{
    public string audioName;     // ★ [추가] 오디오 파일 이름 (Text_1_1 등)
    public string groupID;       // 챕터 그룹 (Dialog_Start_1 등)
    public string speakerName;   // 화자 이름
    public string content;       // 대사 내용
    public int order;            // 순서
    
    // (참고용) 원래 ID도 저장
    public string dialogID;      
}

public class DialogueParser : MonoBehaviour
{
    public TextAsset csvFile; // [인스펙터] CSV 파일 연결
    
    public Dictionary<string, List<DialogueData>> dialogueDictionary = new Dictionary<string, List<DialogueData>>();

    void Awake()
    {
        ParseCSV();
    }

    void ParseCSV()
    {
        if(csvFile == null) return;

        string fileContent = Encoding.UTF8.GetString(csvFile.bytes);
        
        // 엑셀이 가끔 맨 앞에 보이지 않는 문자(BOM)를 넣을 때가 있어서 제거
        if (fileContent.StartsWith("\uFEFF")) fileContent = fileContent.Substring(1);

        string[] lines = fileContent.Split(new char[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);

        // 0, 1번 줄은 헤더이므로 2번 줄부터 시작 (데이터 구조에 따라 조절)
        for (int i = 3; i < lines.Length; i++) 
        {
            string line = lines[i];
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] fields = line.Split(',');
            // 최소한의 데이터 길이 확인 (K열이 10번이므로 11개 이상이어야 함)
            if (fields.Length < 11) continue; 

            DialogueData data = new DialogueData();
            
            // ★ [핵심] 오디오 파일 이름은 10번째 칸 (K열)
            data.audioName = fields[10].Trim(); 

            // 나머지 데이터 매핑
            data.dialogID = fields[6].Trim();       // G열
            data.groupID = fields[7].Trim();        // H열
            int.TryParse(fields[8], out data.order); // I열
            data.speakerName = fields[9].Trim();    // J열
            
            // 내용(P열)은 15번째 칸 (데이터가 짧을 수도 있으니 체크)
            if (fields.Length > 15)
                data.content = fields[14].Trim();
            else
                data.content = ""; // 내용이 없으면 빈칸

            // 딕셔너리에 추가
            if (!dialogueDictionary.ContainsKey(data.groupID))
            {
                dialogueDictionary.Add(data.groupID, new List<DialogueData>());
            }
            dialogueDictionary[data.groupID].Add(data);
        }
    }

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