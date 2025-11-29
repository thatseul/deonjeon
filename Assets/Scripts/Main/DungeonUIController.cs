using UnityEngine;
using TMPro;

public class DungeonUIController : MonoBehaviour
{
    [Header("UI 요소")]
    public TMP_Text GoldText;

    [Header("업글 버튼 라벨")]
    public TMP_Text HpBtnLabel;
    public TMP_Text AtkBtnLabel;
    public TMP_Text AspdBtnLabel;

    [Header("던전 능력치 매니저")]
    public DungeonStatsManager StatsManager;

    private void Awake()
    {
        if (DungeonStatsManager.Instance == null)
        {
        new GameObject("DungeonStatsManager").AddComponent<DungeonStatsManager>();
        }

        StatsManager = DungeonStatsManager.Instance;
    }

    private void Start()
    {
        Invoke(nameof(RefreshUI), 0.05f);
    }

    private void Update()
    {
        // 골드는 실시간으로 변화하므로 Update에서 UI 업데이트
        if (GoldText != null && GameState.I != null)
            GoldText.text = $"Gold: {Mathf.FloorToInt((float)GameState.I.Gold)}";
    }

    // UI 갱신 함수 (스탯)
    public void RefreshUI()
    {
        if (StatsManager == null)
        {
            Debug.LogWarning("DungeonUIController: StatsManager가 연결되지 않음");
            return;
        }

        // HP
        if (HpBtnLabel != null)
        {
            float cur = StatsManager.GetStatCurrentValue("HP");
            float next = StatsManager.GetStatNextValue("HP");
            int cost = StatsManager.GetUpgradeCost("HP");

            HpBtnLabel.text =
                $"체력(HP) 증가   Lv.{StatsManager.GetStatLevel("HP")}\n" +
                $"{cur} → {next}   Cost {cost}";
        }

        // ATK
        if (AtkBtnLabel != null)
        {
            float cur = StatsManager.GetStatCurrentValue("ATK");
            float next = StatsManager.GetStatNextValue("ATK");
            int cost = StatsManager.GetUpgradeCost("ATK");

            AtkBtnLabel.text =
                $"공격력 증가   Lv.{StatsManager.GetStatLevel("ATK")}\n" +
                $"{cur} → {next}   Cost {cost}";
        }

        // ASPD
        if (AspdBtnLabel != null)
        {
            float cur = StatsManager.GetStatCurrentValue("ASPD");
            float next = StatsManager.GetStatNextValue("ASPD");
            int cost = StatsManager.GetUpgradeCost("ASPD");

            AspdBtnLabel.text =
                $"공격속도 증가   Lv.{StatsManager.GetStatLevel("ASPD")}\n" +
                $"{cur:F2} → {next:F2}   Cost {cost}";
        }
    }

    // 버튼이 눌렸을 때 호출되는 함수 (UI → DungeonStatsManager)
    public void OnClickIncreaseHP()
    {
        if (StatsManager == null) return;
        int cost = StatsManager.GetUpgradeCost("HP");
        if (GameState.I.TrySpend(cost))
        {
            StatsManager.UpgradeStat("HP");
            RefreshUI();
        }
    }

    public void OnClickIncreaseATK()
    {
        if (StatsManager == null) return;
        int cost = StatsManager.GetUpgradeCost("ATK");
        if (GameState.I.TrySpend(cost))
        {
            StatsManager.UpgradeStat("ATK");
            RefreshUI();
        }
    }

    public void OnClickIncreaseASPD()
    {
        if (StatsManager == null) return;
        int cost = StatsManager.GetUpgradeCost("ASPD");
        if (GameState.I.TrySpend(cost))
        {
            StatsManager.UpgradeStat("ASPD");
            RefreshUI();
        }
    }
}
