using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillUI : MonoBehaviour, IPointerClickHandler
{
    [Header("UI Elements")]
    public TMP_Text skillNameText;
    public TMP_Text levelText;
    public TMP_Text statText;
    public Button deleteButton;
    public Button upgradeButton;

    private BossSkill bossSkill;
    private BossSkillManager skillManager;

    /// <summary>
    /// BossSkill 데이터와 매니저 연결
    /// </summary>
    public void SetBossSkill(BossSkill skill, BossSkillManager manager)
    {
        bossSkill = skill;
        skillManager = manager;

        RefreshUI();

        deleteButton.onClick.RemoveAllListeners();
        deleteButton.onClick.AddListener(() =>
        {
            skillManager.RemoveSkill(this);
        });

        upgradeButton.onClick.RemoveAllListeners();
        upgradeButton.onClick.AddListener(() => skillManager.OnUpgradeButtonClicked(skill, this));

        gameObject.SetActive(true);
    }

    /// <summary>
    /// UI 요소 갱신
    /// </summary>
    public void RefreshUI()
    {
        if (bossSkill == null)
        {
            Clear();
            return;
        }

        skillNameText.text = bossSkill.skillName;
        levelText.text = $"Lv. {bossSkill.level}";
        statText.text = $"공격 {bossSkill.GetFinalAttack():F1}, 쿨 {bossSkill.GetFinalCooldown():F1}, 범위 {bossSkill.GetFinalRange():F1}";

        gameObject.SetActive(true); // 혹시 비활성화되어 있다면 다시 활성화
    }

    /// <summary>
    /// 슬롯 비우기
    /// </summary>
    public void Clear()
    {
        bossSkill = null;

        skillNameText.text = "";
        levelText.text = "";
        statText.text = "";

        deleteButton.onClick.RemoveAllListeners();
        upgradeButton.onClick.RemoveAllListeners();

        SetSelected(false); // 선택 해제
    }

    /// <summary>
    /// 현재 슬롯이 보유한 스킬 반환
    /// </summary>
    public BossSkill GetBossSkill()
    {
        return bossSkill;
    }

    /// <summary>
    /// 슬롯 클릭 시 선택 처리
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (skillManager != null)
        {
            skillManager.SetSelectedSkill(this);
        }
    }

    /// <summary>
    /// 선택 시 배경 색상 등 UI 강조 처리
    /// </summary>
    public void SetSelected(bool selected)
    {
        Image image = GetComponent<Image>();
        if (image != null)
        {
            image.color = selected ? new Color(1f, 0.92f, 0.6f) : Color.white; // 연노랑 강조
        }
    }
}
