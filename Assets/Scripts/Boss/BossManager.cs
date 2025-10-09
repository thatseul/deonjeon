using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Button upgradeButton;
    [SerializeField] private TextMeshProUGUI statsText;
    [SerializeField] private TextMeshProUGUI resultText;

    [Header("Boss Data")]
    [SerializeField] private BossData bossData;

    [Header("Balance")]
    [SerializeField] private int upgradeCost = 100;

    private void Start()
    {
        UpdateStatsUI();

        if (upgradeButton != null)
        {
            upgradeButton.onClick.RemoveAllListeners();
            upgradeButton.onClick.AddListener(TryUpgrade);
        }
    }

    public void TryUpgrade()
    {
        if (GameState.I == null)
        {
            if (resultText) resultText.text = "GameState not found";
            return;
        }
        if (bossData == null)
        {
            if (resultText) resultText.text = "BossData missing";
            return;
        }

        // 골즈 매니저 수정
        if (!GameState.I.TrySpend(upgradeCost))
        {
            if (resultText) resultText.text = "no money!";
            return;
        }

        // 랜덤 업그레이드
        UpgradeType type = (UpgradeType)Random.Range(0, 3); // 0~2
        int value = Random.Range(-1, 2);                    // -1,0,1

        bossData.ApplyUpgrade(type, value);

        string result = value switch
        {
            1  => "success!",
            0  => "zero",
            -1 => "fail",
            _  => ""
        };

        if (resultText) resultText.text = $"{type} {result} ({value:+#;-#;0})";

        UpdateStatsUI();
    }

    private void UpdateStatsUI()
    {
        if (!statsText || bossData == null) return;

        // 예: bossData.attackPercent = 0.12f 면 “+12%”
        float atkPct = bossData.attackPercent * 100f;

        statsText.text =
            $"atk up: {atkPct:+0.#;-0.#;0}%\n" +
            $"skill time: {bossData.cooldown:F1}s\n" +
            $"atk range: {bossData.range:+0.0;-0.0;0.0}m";
    }
}
