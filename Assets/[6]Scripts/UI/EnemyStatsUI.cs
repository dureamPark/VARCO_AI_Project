using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyStatsUI : MonoBehaviour
{
    [Header("Enemy")]
    public EnemyStats enemyStats;

    [Header("UI")]
    public Image healthBarImage; 

    void Start()
    {
        if (enemyStats != null)
        {
            enemyStats.OnHealthChanged += UpdateEnemyHealthUI;
            UpdateEnemyHealthUI();
        }
    }

    void OnDestroy()
    {
        if (enemyStats != null)
        {
            enemyStats.OnHealthChanged -= UpdateEnemyHealthUI;
        }
    }
    void UpdateEnemyHealthUI()
    {
        if (enemyStats == null) return;
        float ratio = (float)enemyStats.CurrentHealth / enemyStats.MaxHealth;
        healthBarImage.fillAmount = ratio;

        
        if (enemyStats.CurrentHealth <= 0)
        {
            healthBarImage.gameObject.SetActive(false);
        }
        else
        {
            healthBarImage.gameObject.SetActive(true);
        }
    }
}