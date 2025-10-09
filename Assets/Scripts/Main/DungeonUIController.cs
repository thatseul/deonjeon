using UnityEngine;
using TMPro;

public class DungeonUIController : MonoBehaviour
{
    [Header("UI 요소")]
    [SerializeField] private TextMeshProUGUI goldText;

    [Header("던전 능력치 매니저")]
    [SerializeField] private DungeonStatsManager statsManager;

    [Header("업그레이드 비용")]
    [SerializeField] private int atkCost  = 1;
    [SerializeField] private int hpCost   = 1;
    [SerializeField] private int aspdCost = 1;

    private void Start()
    {
        UpdateGoldUI();
    }

    private void Update()
    {
        // 간단히 매 프레임 갱신(원하면 이벤트 방식으로 바꿔도 됨)
        UpdateGoldUI();
    }

    // 공격력 업그레이드 버튼
    public void OnClickIncreaseAttack()
    {
        if (GameState.I == null || statsManager == null) return;

        if (GameState.I.TrySpend(atkCost))
        {
            Debug.Log("💥 공격력 증가 버튼 클릭됨!");
            statsManager.UpgradeStat("ATK");
            UpdateGoldUI();
        }
        else
        {
            Debug.Log("❌ 골드 부족!");
        }
    }

    // 체력 업그레이드 버튼
    public void OnClickIncreaseHp()
    {
        if (GameState.I == null || statsManager == null) return;

        if (GameState.I.TrySpend(hpCost))
        {
            Debug.Log("❤️ 체력 증가 버튼 클릭됨!");
            statsManager.UpgradeStat("HP");
            UpdateGoldUI();
        }
        else
        {
            Debug.Log("❌ 골드 부족!");
        }
    }

    // 공속 업그레이드 버튼
    public void OnClickIncreaseSpeed()
    {
        if (GameState.I == null || statsManager == null) return;

        if (GameState.I.TrySpend(aspdCost))
        {
            Debug.Log("⚡ 공속 증가 버튼 클릭됨!");
            statsManager.UpgradeStat("ASPD");
            UpdateGoldUI();
        }
        else
        {
            Debug.Log("❌ 골드 부족!");
        }
    }

    private void UpdateGoldUI()
    {
        if (goldText && GameState.I != null)
            goldText.text = $"Gold: {Mathf.FloorToInt((float)GameState.I.Gold)}";
    }
}
