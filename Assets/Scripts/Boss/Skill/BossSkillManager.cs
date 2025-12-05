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
    public int drawCost = 100; // 스킬 뽑기 비용
    public int upgradeCost = 50; // 스킬 업그레이드 비용

    private void Start()
    {
        skillDatabase = FindObjectOfType<SkillDatabase>();
        if (skillDatabase == null)
            Debug.LogWarning("BossSkillManager: SkillDatabase를 찾을 수 없음.");

        if (drawSkillButton != null)
            drawSkillButton.onClick.AddListener(DrawSkill);
        else
            Debug.LogWarning("BossSkillManager: drawSkillButton이 할당되지 않음.");

        selectedSkillUI = null;

        RebuildSkillUI();

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

        if (skillDatabase == null || skillDatabase.allSkillData == null || skillDatabase.allSkillData.Count == 0)
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

        if (skillSlotPrefab == null || skillSlotParent == null)
        {
            Debug.LogError("skillSlotPrefab 또는 skillSlotParent가 할당되지 않았습니다.");
            return;
        }

        GameObject go = Instantiate(skillSlotPrefab, skillSlotParent);
        SkillUI skillUI = go.GetComponent<SkillUI>();
        if (skillUI == null)
        {
            Debug.LogError("skillSlotPrefab에 SkillUI 컴포넌트가 없습니다.");
            Destroy(go);
            return;
        }

        skillUI.SetBossSkill(newSkill, this);
        skillSlotList.Add(skillUI);

        // SkillTransferManager에 저장 (존재하면)
        if (SkillTransferManager.Instance != null)
        {
            SkillTransferManager.Instance.selectedSkills.Add(newSkill);
        }
        else
        {
            Debug.LogWarning("SkillTransferManager.Instance is null — 스킬 영속화가 불가능합니다.");
        }

        UpdateButtonStates();
    }

    public void RemoveSkill(SkillUI skillUI)
    {
        if (skillUI == null) return;

        // SkillTransferManager에서도 삭제
        if (SkillTransferManager.Instance != null)
        {
            var bs = skillUI.GetBossSkill();
            if (bs != null)
                SkillTransferManager.Instance.selectedSkills.Remove(bs);
        }

        skillSlotList.Remove(skillUI);
        Destroy(skillUI.gameObject);

        if (selectedSkillUI == skillUI)
        {
            selectedSkillUI = null;
            if (selectedSkillNameText != null) selectedSkillNameText.text = "empty";
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
            if (selectedSkillNameText != null)
                selectedSkillNameText.text = $"{skillUI.GetBossSkill().skillName}";
            skillUI.SetSelected(true);
        }
        else
        {
            if (selectedSkillNameText != null)
                selectedSkillNameText.text = "empty";
        }

        UpdateButtonStates();
    }

    public void StartMiniGameForUpgrade(BossSkill skill, SkillUI skillUI)
    {
        if (GameState.I == null)
        {
            Debug.Log("GameState/I is null");
            return;
        }

        if (!GameState.I.TrySpend(upgradeCost))
        {
            Debug.Log("골드 부족!");
            return;
        }

        MiniGameController miniGame = FindFirstObjectByType<MiniGameController>();

        if (miniGame == null)
        {
            Debug.LogError("MiniGameController를 찾을 수 없습니다. miniGame 시작 실패.");
            return;
        }

        miniGame.StartMiniGame(skill, skillUI);
    }

    private void UpdateButtonStates()
    {
        if (drawSkillButton != null)
            drawSkillButton.interactable = skillSlotList.Count < maxSkillCount;
    }
    private void RebuildSkillUI()
    {
        Debug.Log("RebuildSkillUI 실행");

        // 먼저 기존 UI 정리
        foreach (var ui in skillSlotList)
        {
            if (ui != null)
                Destroy(ui.gameObject);
        }
        skillSlotList.Clear();
        selectedSkillUI = null;
        if (selectedSkillNameText != null) selectedSkillNameText.text = "empty";

        // SkillTransferManager가 존재하는지 확인
        if (SkillTransferManager.Instance == null)
        {
            Debug.LogWarning("RebuildSkillUI: SkillTransferManager.Instance가 null입니다. 유지된 스킬이 없습니다.");
            return;
        }

        if (SkillTransferManager.Instance.selectedSkills == null || SkillTransferManager.Instance.selectedSkills.Count == 0)
        {
            Debug.Log("RebuildSkillUI: 유지된 스킬 없음");
            return;
        }

        if (skillSlotPrefab == null || skillSlotParent == null)
        {
            Debug.LogError("RebuildSkillUI: skillSlotPrefab 또는 skillSlotParent가 할당되지 않았습니다.");
            return;
        }

        // 저장된 스킬로 UI 재생성
        foreach (var bs in SkillTransferManager.Instance.selectedSkills)
        {
            if (bs == null) continue;
            GameObject go = Instantiate(skillSlotPrefab, skillSlotParent);
            SkillUI ui = go.GetComponent<SkillUI>();
            if (ui == null)
            {
                Debug.LogError("RebuildSkillUI: prefab에 SkillUI가 없습니다.");
                Destroy(go);
                continue;
            }

            ui.SetBossSkill(bs, this);
            skillSlotList.Add(ui);
        }

        Debug.Log($"RebuildSkillUI 완료: {skillSlotList.Count}개 생성");
    }
}
