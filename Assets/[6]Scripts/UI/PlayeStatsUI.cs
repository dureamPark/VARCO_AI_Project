using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PlayerStatsUI : MonoBehaviour
{
    //어디서든 접근할 수 있게 싱글톤 인스턴스 선언
    public static PlayerStatsUI Instance;

    [HideInInspector]
    private PlayerStats playerStats; //다른데서 가져다 쓸일 없으니 private으로 수정완료(두사부 피드백)
    private PlayerSkill playerSkill;

    [Header("Life UI (Hearts)")]  // 체력
    public Image[] lifeIcons;

    [Header("Bomb UI (Bombs)")]  //빰
    public Image[] bombIcons;

    [Header("Text UI")]
    public TextMeshProUGUI attackText; //공격력

    [Header("Skill UI")]
    public Image barrierGaugeImage; // 베리어

    [Header("Weapon Mode UI")]
    public Image weaponModeIconImage;   
    public Sprite normalModeSprite;     //토글
    public Sprite homingModeSprite;     //유도탄

    private void Awake()
    {
        //게임 시작 시 전광판(Instance) 켜기
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // 중복 방지
        }
    }
    private void Update() //베리어 게이지 관리용
    {
        if (playerSkill != null && barrierGaugeImage != null)
        {
            float fillAmount = playerSkill.CurrentBarrierEnergy / playerSkill.MaxBarrierEnergy;
            barrierGaugeImage.fillAmount = fillAmount;
        }
    }
    void OnDestroy()
    {
        if (playerStats != null)
        {
            playerStats.OnStatsChanged -= UpdateUI;
        }
        if (playerSkill != null)
        {
            playerSkill.OnWeaponModeChanged -= UpdateWeaponIcon;
        }

    }

    // 플레이어가 태어나면 이 함수를 통해 자신을 등록함
    public void SetPlayer(PlayerStats player)
    {
        // 안전장치
        if (playerStats != null) //스탯
        {
            playerStats.OnStatsChanged -= UpdateUI;
        }
        if (playerSkill != null)
        {
            playerSkill.OnWeaponModeChanged -= UpdateWeaponIcon;
        }

        playerStats = player;

        if (playerStats != null)
        {
            playerStats.OnStatsChanged += UpdateUI;
            UpdateUI();
            playerSkill = player.GetComponent<PlayerSkill>();
            if (playerSkill != null)
            {
                playerSkill.OnWeaponModeChanged += UpdateWeaponIcon;
                UpdateWeaponIcon(playerSkill.IsHomingMode);
            }
        }
    }
    private void UpdateWeaponIcon(bool isHoming)
    {
        if (weaponModeIconImage == null) return;

        if (isHoming)
            weaponModeIconImage.sprite = homingModeSprite;
        else
            weaponModeIconImage.sprite = normalModeSprite;
    }
    void UpdateUI()// ui 통합 갱신용
    {
        if (playerStats == null) return;
        UpdateLives();
        UpdateBombs();
        UpdateTextInfo();
    }
    void UpdateLives() //체력 관리용
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
    void UpdateBombs() //빰 관리용
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
    void UpdateTextInfo() //공격력 관리용
    {
        if (attackText != null)
        {
            attackText.text = $"POWER: {playerStats.AttackPower}";
        }
    }
    
}