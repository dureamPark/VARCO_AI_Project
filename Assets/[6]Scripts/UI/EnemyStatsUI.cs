using UnityEngine;
using UnityEngine.UI;

public class EnemyStatsUI : MonoBehaviour
{
    public static EnemyStatsUI Instance;

    [Header("UI Components")]
    public GameObject healthBarContainer;
    public Image healthBarImage;
    private EnemyStats currentBoss;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        if (healthBarContainer != null)
            healthBarContainer.SetActive(false);
    }
    public void SetBoss(EnemyStats boss)
    {
        if (currentBoss != null)
        {
            currentBoss.OnHealthChanged -= UpdateUI;
            currentBoss.OnDead -= HideUI;
        }

        currentBoss = boss;
        if (currentBoss != null)
        {
            currentBoss.OnHealthChanged += UpdateUI;
            currentBoss.OnDead += HideUI;
            if (healthBarContainer != null) healthBarContainer.SetActive(true);
            UpdateUI();
        }
    }
    void UpdateUI()
    {
        if (currentBoss == null) return;
        float ratio = (float)currentBoss.CurrentHealth / currentBoss.MaxHealth;

        if (healthBarImage != null)
            healthBarImage.fillAmount = ratio;
    }

    void HideUI()
    {
        if (healthBarContainer != null)
            healthBarContainer.SetActive(false);
        if (currentBoss != null)
        {
            currentBoss.OnHealthChanged -= UpdateUI;
            currentBoss.OnDead -= HideUI;
            currentBoss = null;
        }
    }
}