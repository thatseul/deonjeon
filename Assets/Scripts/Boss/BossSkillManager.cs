using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossSkillManager : MonoBehaviour
{
    [Header("Skill Setup")]
    public List<SkillData> allSkillData;
    public GameObject skillSlotPrefab;          // 슬롯 프리팹
    public Transform skillSlotParent;           // HorizontalLayoutGroup 부모

    private List<SkillUI> skillSlotList = new(); // 생성된 슬롯 목록
    private const int maxSkillCount = 5;

    [Header("UI")]
    public Button drawSkillButton;
    public Button upgradeSkillButton;
    public TMP_Text selectedSkillNameText;

    private SkillUI selectedSkillUI;

    private void Start()
    {
        drawSkillButton.onClick.AddListener(DrawSkill);
        upgradeSkillButton.onClick.AddListener(OnUpgradeButtonClicked);

        selectedSkillUI = null; // 강제로 초기화
        UpdateButtonStates();
    }

    public void DrawSkill()
    {
        if (allSkillData == null || allSkillData.Count == 0)
        {
            Debug.LogError("스킬 데이터가 존재하지 않습니다.");
            return;
        }

        if (skillSlotList.Count >= maxSkillCount)
        {
            Debug.Log("더 이상 스킬을 뽑을 수 없습니다.");
            return;
        }

        SkillData randomData = allSkillData[Random.Range(0, allSkillData.Count)];
        BossSkill newSkill = new BossSkill(randomData);

        GameObject go = Instantiate(skillSlotPrefab, skillSlotParent);
        SkillUI skillUI = go.GetComponent<SkillUI>();
        skillUI.SetBossSkill(newSkill, this);

        skillSlotList.Add(skillUI);
        UpdateButtonStates();
    }


    public void RemoveSkill(SkillUI skillUI)
    {
        if (skillUI == null) return;

        skillSlotList.Remove(skillUI);
        Destroy(skillUI.gameObject);

        if (selectedSkillUI == skillUI)
        {
            selectedSkillUI = null;
            selectedSkillNameText.text = "선택된 스킬 없음";
        }

        UpdateButtonStates();
    }

    public void SetSelectedSkill(SkillUI skillUI)
    {
        if (selectedSkillUI != null)
            selectedSkillUI.SetSelected(false);

        selectedSkillUI = skillUI;

        if (skillUI != null && skillUI.GetBossSkill() != null)
        {
            selectedSkillNameText.text = $"선택된 스킬: {skillUI.GetBossSkill().skillName}";
            skillUI.SetSelected(true);
        }
        else
        {
            selectedSkillNameText.text = "선택된 스킬 없음";
        }

        UpdateButtonStates();
    }

    public void OnUpgradeButtonClicked()
    {
        if (selectedSkillUI == null)
        {
            Debug.Log("강화할 스킬을 먼저 선택하세요.");
            return;
        }
        OnUpgradeButtonClicked(selectedSkillUI.GetBossSkill(), selectedSkillUI);

    }
    public void OnUpgradeButtonClicked(BossSkill skill, SkillUI skillUI)
    {
        if (selectedSkillUI == null)
        {
            Debug.Log("강화할 스킬을 먼저 선택하세요.");
            return;
        }

        MiniGameController miniGame = FindFirstObjectByType<MiniGameController>();
        if (miniGame != null)
        {
            miniGame.StartMiniGame(selectedSkillUI.GetBossSkill(), selectedSkillUI);
        }
    }

    private void UpdateButtonStates()
    {
        drawSkillButton.interactable = skillSlotList.Count < maxSkillCount;
        upgradeSkillButton.interactable = selectedSkillUI != null;
    }
}
