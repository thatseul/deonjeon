using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossSkillManager : MonoBehaviour
{
    [Header("Skill Setup")]
    public List<SkillData> allSkillData;
    public GameObject skillSlotPrefab;
    public Transform skillSlotParent;

    private List<SkillUI> skillSlotList = new();
    private const int maxSkillCount = 5;

    [Header("UI")]
    public Button drawSkillButton;
    public TMP_Text selectedSkillNameText;

    private SkillUI selectedSkillUI;
    private SkillDatabase skillDatabase;

    [Header("Balance")]
    public int drawCost = 50; // 스킬 뽑기 비용
    public int upgradeCost = 10; // 스킬 업그레이드 비용

    private void Start()
    {
        skillDatabase = FindObjectOfType<SkillDatabase>();
        drawSkillButton.onClick.AddListener(DrawSkill);

        selectedSkillUI = null;
        UpdateButtonStates();
    }

    public void DrawSkill()
    {

        if (GameState.I == null)
        {
            Debug.Log("골드 부족!");
            return;
        }

        if (!GameState.I.TrySpend(drawCost))
        {
            Debug.Log("골드 부족!");
            return;
        }
        
        if (skillDatabase.allSkillData == null || skillDatabase.allSkillData.Count == 0)
        {
            Debug.LogError("스킬 데이터가 존재하지 않습니다.");
            return;
        }

        if (skillSlotList.Count >= maxSkillCount)
        {
            Debug.Log("더 이상 스킬을 뽑을 수 없습니다.");
            return;
        }

        SkillData randomData = skillDatabase.allSkillData[Random.Range(0, skillDatabase.allSkillData.Count)];
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
            selectedSkillNameText.text = "empty";
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
            selectedSkillNameText.text = $"{skillUI.GetBossSkill().skillName}";
            skillUI.SetSelected(true);
        }
        else
        {
            selectedSkillNameText.text = "empty";
        }

        UpdateButtonStates();
    }

    public void StartMiniGameForUpgrade(BossSkill skill, SkillUI skillUI)
    {

        if (!GameState.I.TrySpend(upgradeCost))
        {
            Debug.Log("골드 부족!");
            return;
        }

        MiniGameController miniGame = FindFirstObjectByType<MiniGameController>();
        if (miniGame != null)
        {
            miniGame.StartMiniGame(skill, skillUI);
        }
    }

    private void UpdateButtonStates()
    {
        drawSkillButton.interactable = skillSlotList.Count < maxSkillCount;
    }
}
