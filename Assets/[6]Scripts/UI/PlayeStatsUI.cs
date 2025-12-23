using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStatsUI : MonoBehaviour
{
    [Header("Target")]
    public PlayerStats playerStats;

    [Header("Life UI (Hearts)")]
    public Image[] lifeIcons;

    [Header("Bomb UI (Bombs)")]
    public Image[] bombIcons;

    [Header("Text UI")]
    public TextMeshProUGUI attackText; 

    void Start()
    {
        if (playerStats != null)
        {
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
    void UpdateUI()
    {
        UpdateLives();
        UpdateBombs();
        UpdateTextInfo();
    }
    void UpdateLives()
    {
        if (lifeIcons == null) return;

        for (int i = 0; i < lifeIcons.Length; i++)
        {
            lifeIcons[i].enabled = (i < playerStats.CurrentLives);
        }
    }

    void UpdateBombs()
    {
        if (bombIcons == null) return;

        for (int i = 0; i < bombIcons.Length; i++)
        {
            bombIcons[i].enabled = (i < playerStats.CurrentBombs);
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