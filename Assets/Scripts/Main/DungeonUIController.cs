using UnityEngine;
using TMPro;

public class DungeonUIController : MonoBehaviour
{
    [Header("UI 요소")]
    [SerializeField] private TextMeshProUGUI goldText;

    [Header("업글 버튼 라벨")]
    [SerializeField] private TextMeshProUGUI hpBtnLabel;
    [SerializeField] private TextMeshProUGUI atkBtnLabel;
    [SerializeField] private TextMeshProUGUI aspdBtnLabel;

    [Header("던전 능력치 매니저")]
    [SerializeField] private DungeonStatsManager statsManager;

    private void Start()
    {
        RefreshAllUI();
    }

    private void Update()
    {
        // 간단 갱신
        UpdateGoldUI();
    }

    // ===== 버튼 클릭 =====
    public void OnClickIncreaseAttack()
    {
        if (GameState.I == null || statsManager == null) return;
        int cost = statsManager.GetUpgradeCost("ATK");
        if (GameState.I.TrySpend(cost))
        {
            Debug.Log($"💥 공격력 증가! -{cost}");
            statsManager.UpgradeStat("ATK");
            RefreshAllUI();
        }
        else Debug.Log("❌ 골드 부족!");
    }

    public void OnClickIncreaseHp()
    {
        if (GameState.I == null || statsManager == null) return;
        int cost = statsManager.GetUpgradeCost("HP");
        if (GameState.I.TrySpend(cost))
        {
            Debug.Log($"❤️ 체력 증가! -{cost}");
            statsManager.UpgradeStat("HP");
            RefreshAllUI();
        }
        else Debug.Log("❌ 골드 부족!");
    }

    public void OnClickIncreaseSpeed()
    {
        if (GameState.I == null || statsManager == null) return;
        // ASPD 상한(3f)은 기존 Manager에서 검사됨
        int cost = statsManager.GetUpgradeCost("ASPD");
        if (GameState.I.TrySpend(cost))
        {
            Debug.Log($"⚡ 공속 증가! -{cost}");
            statsManager.UpgradeStat("ASPD");
            RefreshAllUI();
        }
        else Debug.Log("❌ 골드 부족!");
    }

    // ===== UI 갱신 =====
    private void RefreshAllUI()
    {
        UpdateGoldUI();
        UpdateUpgradeLabels();
    }

    private void UpdateGoldUI()
    {
        if (goldText && GameState.I != null)
            goldText.text = $"Gold: {Mathf.FloorToInt((float)GameState.I.Gold)}";
    }

    private void UpdateUpgradeLabels()
    {
        if (statsManager == null) return;

        int hpLv = statsManager.GetStatLevel("HP");
        int atkLv = statsManager.GetStatLevel("ATK");
        int aspdLv = statsManager.GetStatLevel("ASPD");

        int hpCost = statsManager.GetUpgradeCost("HP");
        int atkCost = statsManager.GetUpgradeCost("ATK");
        int aspdCost = statsManager.GetUpgradeCost("ASPD");

        float hpCur = statsManager.GetStatCurrentValue("HP");
        float hpNext = statsManager.GetStatNextValue("HP");
        float atkCur = statsManager.GetStatCurrentValue("ATK");
        float atkNext = statsManager.GetStatNextValue("ATK");
        float spdCur = statsManager.GetStatCurrentValue("ASPD");
        float spdNext = statsManager.GetStatNextValue("ASPD");

        string F(float v, bool isSpeed = false) => isSpeed ? v.ToString("0.00") : Mathf.RoundToInt(v).ToString("N0");

        if (hpBtnLabel) hpBtnLabel.text = $"체력(HP) 증가        Lv.{hpLv}\n{F(hpCur)} → {F(hpNext)}      Cost {hpCost}";
        if (atkBtnLabel) atkBtnLabel.text = $"공격력 증가          Lv.{atkLv}\n{F(atkCur)} → {F(atkNext)}    Cost {atkCost}";
        if (aspdBtnLabel) aspdBtnLabel.text = $"공격속도 증가        Lv.{aspdLv}\n{F(spdCur, true)} → {F(spdNext, true)}  Cost {aspdCost}";
    }
}
