using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossManager : MonoBehaviour
{
    public Button upgradeButton;

    [Header("Boss Data")]
    public BossData bossData;

    [Header("UI")]
    public TextMeshProUGUI statsText;
    public TextMeshProUGUI resultText;
    public int upgradeCost = 100;

    private void Start()
    {
        UpdateStatsUI();

        if (upgradeButton != null)
        {
            upgradeButton.onClick.AddListener(TryUpgrade);
        }
    }

    public void TryUpgrade()
    {
        if (!GoldManager.Instance.SpendGold(upgradeCost))
        {
            resultText.text = "no money!";
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

    private void UpdateStatsUI()
    {
        statsText.text =
            $"atk up: +{bossData.attackPercent * 100f}%\n" +
            $"skill time: {bossData.cooldown:F1}s\n" +
            $"atk range: +{bossData.range:F1}m";
    }
}
