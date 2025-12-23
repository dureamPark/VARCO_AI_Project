using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class PlayerStatsUI : MonoBehaviour
{
    [Header("Player")]
    public PlayerStats playerStats;

    [Header("UI")]
    public Image[] hearts;
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI manaText;

    void Start()
    {
        if (playerStats != null)
        {
            playerStats.OnHealthChanged += UpdateHealthUI;
            playerStats.OnStatsChanged += UpdateStatsUI;

            // 시작할 때 UI 초기화
            UpdateHealthUI();
            UpdateStatsUI();
        }
    }

    void OnDestroy()
    {
        if (playerStats != null)
        {
            playerStats.OnHealthChanged -= UpdateHealthUI;
            playerStats.OnStatsChanged -= UpdateStatsUI;
        }
    }

    void Update()
    {
        if (playerStats != null)
        {
            manaText.text = $"Mana: {playerStats.CurrentMana:F0} / {playerStats.MaxMana}";
        }
    }

    void UpdateHealthUI()
    {
        // 하트 개수만큼 반복문을 돕니다.
        for (int i = 0; i < hearts.Length; i++)
        {
            // i가 현재 체력보다 작으면 하트를 켜고, 크거나 같으면 끕니다.
            // 예: 체력 3 -> 인덱스 0,1,2는 true(켜짐), 3,4는 false(꺼짐)
            if (i < playerStats.CurrentHealth)
            {
                hearts[i].enabled = true;  // 하트 보이기

                // 만약 빈 하트 이미지가 따로 있다면 enabled 대신 Sprite를 교체하는 방식을 씁니다.
                // 지금은 이미지가 하나뿐이므로 끄는 방식을 사용합니다.
            }
            else
            {
                hearts[i].enabled = false; // 하트 숨기기
            }
        }
    }

    void UpdateStatsUI()
    {
        attackText.text = $"ATK: {playerStats.AttackPower}";
    }
}