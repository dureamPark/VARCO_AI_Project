using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class PlayerStatsUI : MonoBehaviour
{
    [Header("player")]
    public PlayerStats playerStats; 

    [Header("UI")]
    public Image healthBarImage;       
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI manaText;   

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
    void Update()
    {
        if (playerStats != null)
        {
            manaText.text = $"Mana: {playerStats.CurrentMana:F0} / {playerStats.MaxMana}";
        }
    }

    void UpdateHealthUI()
    {
        float ratio = (float)playerStats.CurrentHealth / playerStats.MaxHealth;
        healthBarImage.fillAmount = ratio;
        healthBarImage.gameObject.SetActive(playerStats.CurrentHealth > 0);
    }

    void UpdateStatsUI()
    {
        attackText.text = $"ATK: {playerStats.AttackPower}";
    }
}