using UnityEngine;
using UnityEngine.UI;

public class EnemyStatsUI : MonoBehaviour
{
    public static EnemyStatsUI Instance;

    [Header("UI Components")]
    public GameObject healthBarContainer;
    public Image healthBarImage;

    [Header("Color Settings")] // [추가] 색상 설정
    public Color healthColor = Color.red;   // 체력일 때 (빨강)
    public Color shieldColor = Color.cyan;  // 실드일 때 (하늘색)

    private EnemyStats currentBoss;
    private bool isShieldMode = false; 

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (healthBarContainer != null) healthBarContainer.SetActive(false);
    }

    public void SetBoss(EnemyStats boss)
    {
        // 기존 연결 해제
        if (currentBoss != null)
        {
            currentBoss.OnHealthChanged -= UpdateUI;
            currentBoss.OnDead -= HideUI;
        }

        currentBoss = boss;

        // 새 보스 연결
        if (currentBoss != null)
        {
            currentBoss.OnHealthChanged += UpdateUI;
            currentBoss.OnDead += HideUI;
            if (healthBarContainer != null) healthBarContainer.SetActive(true);

            
            if (currentBoss.MaxShield > 0)
            {
                SetDisplayMode(true); // 실드 모드
            }
            else
            {
                SetDisplayMode(false); // 체력 모드
            }
        }
    }


    public void SetDisplayMode(bool isShield)
    {
        isShieldMode = isShield;

        // 모드에 따라 색상 변경
        if (healthBarImage != null)
        {
            healthBarImage.color = isShieldMode ? shieldColor : healthColor;
        }

        UpdateUI(); // 즉시 갱신
    }

    void UpdateUI()
    {
        if (currentBoss == null) return;

        if (isShieldMode && currentBoss.CurrentShield <= 0)
        {
            SetDisplayMode(false); // 체력으로 변환
            return; 
        }

        float ratio = 0f;

        if (isShieldMode)
        {
            ratio = (float)currentBoss.CurrentShield / currentBoss.MaxShield;
        }
        else
        {
            ratio = (float)currentBoss.CurrentHealth / currentBoss.MaxHealth;
        }

        if (healthBarImage != null)
            healthBarImage.fillAmount = ratio;
    }

    void HideUI()
    {
        if (healthBarContainer != null) healthBarContainer.SetActive(false);
        // (안전하게 이벤트 해제는 유지)
        if (currentBoss != null)
        {
            currentBoss.OnHealthChanged -= UpdateUI;
            currentBoss.OnDead -= HideUI;
            currentBoss = null;
        }
    }
}