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
        upgradeButton.onClick.AddListener(() =>
        {
            skillManager.StartMiniGameForUpgrade(bossSkill, this);
        });

        upgradeButton.interactable = false;

        gameObject.SetActive(true);
    }


    public void RefreshUI()
    {
        if (bossSkill == null)
        {
            Clear();
            return;
        }

        skillNameText.text = bossSkill.skillName;
        levelText.text = $"Lv. {bossSkill.level}\n" +  $"{bossSkill.grade}";
        statText.text =
            $"atk {bossSkill.GetFinalAttack():F1}\n" +
            $"cool {bossSkill.GetFinalCooldown():F1}\n" +
            $"range {bossSkill.GetFinalRange():F1}";

        gameObject.SetActive(true);
    }

    public void Clear()
    {
        bossSkill = null;

        skillNameText.text = "";
        levelText.text = "";
        statText.text = "";

        deleteButton.onClick.RemoveAllListeners();
        upgradeButton.onClick.RemoveAllListeners();

        SetSelected(false);
    }

    public BossSkill GetBossSkill()
    {
        return bossSkill;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (skillManager != null)
        {
            skillManager.SetSelectedSkill(this);
        }
    }

    public void SetSelected(bool selected)
    {
        Image image = GetComponent<Image>();
        if (image != null)
        {
            image.color = selected ? new Color(1f, 0.92f, 0.6f) : Color.white;
        }

        if (upgradeButton != null)
        {
            upgradeButton.interactable = selected;
        }
    }
}
