using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStatsUI : MonoBehaviour
{
    // ▼▼▼ [핵심 1] 어디서든 접근할 수 있게 싱글톤 인스턴스 선언
    public static PlayerStatsUI Instance;

    [Header("Target")]
    public PlayerStats playerStats; // 인스펙터에서 연결 안 해도 됨 (코드로 연결됨)

    [Header("Life UI (Hearts)")]
    public Image[] lifeIcons;

    [Header("Bomb UI (Bombs)")]
    public Image[] bombIcons;

    [Header("Text UI")]
    public TextMeshProUGUI attackText;

    private void Awake()
    {
        // ▼▼▼ [핵심 2] 게임 시작 시 전광판(Instance) 켜기
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // 중복 방지
        }
    }

    // ▼▼▼ [핵심 3] 플레이어가 태어나면 이 함수를 통해 자신을 등록함
    public void SetPlayer(PlayerStats player)
    {
        // 기존에 연결된 게 있다면 끊어주기 (안전장치)
        if (playerStats != null)
        {
            playerStats.OnStatsChanged -= UpdateUI;
        }

        playerStats = player;

        if (playerStats != null)
        {
            // 이벤트 구독 및 즉시 갱신
            playerStats.OnStatsChanged += UpdateUI;
            UpdateUI();
        }
    }

    void OnDestroy()
    {
        if (playerStats != null)
        {
            playerStats.OnStatsChanged -= UpdateUI;
        }
    }

    // 통합 업데이트 함수
    void UpdateUI()
    {
        if (playerStats == null) return;

        UpdateLives();
        UpdateBombs();
        UpdateTextInfo();
    }

    void UpdateLives()
    {
        if (lifeIcons == null) return;
        for (int i = 0; i < lifeIcons.Length; i++)
        {
            if (i < playerStats.CurrentLives)
                lifeIcons[i].enabled = true;
            else
                lifeIcons[i].enabled = false;
        }
    }

    void UpdateBombs()
    {
        if (bombIcons == null) return;
        for (int i = 0; i < bombIcons.Length; i++)
        {
            if (i < playerStats.CurrentBombs)
                bombIcons[i].enabled = true;
            else
                bombIcons[i].enabled = false;
        }
    }

    void UpdateTextInfo()
    {
        if (attackText != null)
        {
            attackText.text = $"POWER: {playerStats.AttackPower}";
        }
    }
}