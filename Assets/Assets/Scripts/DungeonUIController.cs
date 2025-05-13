using UnityEngine;
using TMPro;

/// <summary>
/// UI 버튼을 통해 던전의 능력치를 강화하는 기능을 담당.
/// </summary>
public class DungeonUIController : MonoBehaviour
{
    [Header("UI 요소")]
    [Tooltip("현재 골드를 표시하는 TextMeshProUGUI")]
    [SerializeField] private TextMeshProUGUI goldText;

    [Tooltip("던전 능력치 매니저")]
    [SerializeField] private DungeonStatsManager statsManager;

    private void Start() 
    {
        UpdateGoldUI();
    }

    /// 공격력 업그레이드 버튼에 연결될 함수
    public void OnClickIncreaseAttack()
    {
        if (GoldManager.Instance.GetCurrentGold() > 0)
        {
            Debug.Log("💥 공격력 증가 버튼 클릭됨!");
            statsManager.UpgradeStat("ATK");
            GoldManager.Instance.SpendGold(1);
            UpdateGoldUI();
        }
    }

    /// 체력 업그레이드 버튼에 연결될 함수
    public void OnClickIncreaseHp()
    {
        if (GoldManager.Instance.GetCurrentGold() > 0)
        {
            Debug.Log("❤️ 체력 증가 버튼 클릭됨!");
            statsManager.UpgradeStat("HP");
            GoldManager.Instance.SpendGold(1);
            UpdateGoldUI();
        }
    }

    //공속
    public void OnClickIncreaseSpeed()
{
    if (GoldManager.Instance.GetCurrentGold() > 0)
    {
        Debug.Log("⚡공속 증가 버튼 클릭됨!");
        statsManager.UpgradeStat("ASPD");
        GoldManager.Instance.SpendGold(1);
        UpdateGoldUI();
    }
}

    /// <summary>
    /// 현재 골드 값을 UI에 반영
    /// </summary>
    private void UpdateGoldUI()
    {
        if (goldText != null)
        {
            goldText.text = $"Gold: {GoldManager.Instance.GetCurrentGold()}";
        }
    }
}
