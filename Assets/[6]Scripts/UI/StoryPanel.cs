using UnityEngine;
using TMPro; // 네임스페이스 변경됨

public class StoryPanel : MonoBehaviour
{
    // [인스펙터] Hierarchy에 있는 TextMeshProUGUI 오브젝트를 여기에 연결하세요
    public TMP_Text nameText;   // 화자 이름 (기존 Text -> TMP_Text 로 변경)
    public TMP_Text scriptText; // 대사 내용 (기존 Text -> TMP_Text 로 변경)
    
    // public UnityEngine.UI.Image portraitImage; // 이미지는 그대로 UnityEngine.UI 사용
}