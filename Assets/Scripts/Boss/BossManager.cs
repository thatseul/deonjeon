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
    public int upCost = 100;

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

        if (!GameState.I.TrySpend(upCost))
        {
            if (resultText) resultText.text = "no money!";
            return;
        }

        // 랜덤 업그레이드
        UpgradeType type = (UpgradeType)Random.Range(0, 3); 
        int value = Random.Range(-1, 2);                    

        bossData.ApplyUpgrade(type, value);

        string result = value switch
        {
            1  => "Success!",
            0  => "Zero",
            -1 => "Fail",
            _  => ""
        };

        if (resultText) resultText.text = $"{type} {result} ({value:+#;-#;0})";

        UpdateStatsUI();
    }

    private void UpdateStatsUI()
    {
        if (!statsText || bossData == null) return;

        float atkPct = bossData.attackPercent * 100f;

        statsText.text =
            $"atk : {atkPct:+0.#;-0.#;0}%\n" +
            $"skill time: {bossData.cooldown:F1}s\n" +
            $"range: {bossData.range:+0.0;-0.0;0.0}m";
    }
}
