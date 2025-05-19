using UnityEngine;
using TMPro;

public class BossManager : MonoBehaviour
{
    [Header("Boss Data")]
    public BossData bossData; // 인스펙터에서 ScriptableObject 할당

    [Header("UI")]
    public TextMeshProUGUI statsText;
    public TextMeshProUGUI resultText;
    public int upgradeCost = 100;

    public void TryUpgrade()
    {
        if (!GoldManager.Instance.SpendGold(upgradeCost))
        {
            resultText.text = "❌ 골드 부족!";
            return;
        }

        UpgradeType type = (UpgradeType)Random.Range(0, 3);
        int value = Random.Range(-1, 2); // -1, 0, 1

        bossData.ApplyUpgrade(type, value);

        string result = value switch
        {
            1 => "success!",
            0 => "zero",
            -1 => "fail",
            _ => ""
        };

        resultText.text = $"{type} {result} ({value})";

        UpdateStatsUI();
    }

    private void Start()
    {
        UpdateStatsUI();
    }

    private void UpdateStatsUI()
    {
        statsText.text =
            $"공격력 보정: +{bossData.attackPercent * 100f}%\n" +
            $"스킬 쿨타임: {bossData.cooldown:F1}s\n" +
            $"공격 범위: +{bossData.range:F1}m";
    }
}
