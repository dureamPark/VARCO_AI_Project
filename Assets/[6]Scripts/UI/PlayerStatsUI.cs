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

    void Start()
    {
        if (playerStats != null)
        {
            playerStats.OnHealthChanged += UpdateHealthUI;
            playerStats.OnStatsChanged += UpdateStatsUI;

            
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

    void UpdateHealthUI()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            
            if (i < playerStats.CurrentHealth)
            {
                hearts[i].enabled = true;
            }
            else
            {
                hearts[i].enabled = false;
            }
        }
    }

    void UpdateStatsUI()
    {
        attackText.text = $"ATK: {playerStats.AttackPower}";
    }
}