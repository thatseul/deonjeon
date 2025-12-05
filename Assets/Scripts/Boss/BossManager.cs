using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Button upgradeButton;
    [SerializeField] private TextMeshProUGUI statsText;
    [SerializeField] private TextMeshProUGUI resultText;

    [Header("Boss Data (기본값 템플릿)")]
    [SerializeField] private BossData bossData;

    [Header("Balance")]
    public int upCost = 100;

    private void Start()
    {
        UpdateStatsUI();

        upgradeButton.onClick.RemoveAllListeners();
        upgradeButton.onClick.AddListener(TryUpgrade);
    }

    public void TryUpgrade()
    {
        if (GameState.I == null)
        {
            resultText.text = "GameState not found";
            return;
        }

        if (BossStateManager.Instance == null)
        {
            resultText.text = "BossStateManager missing";
            return;
        }

        if (!GameState.I.TrySpend(upCost))
        {
            resultText.text = "No money!";
            return;
        }

        // 랜덤 업그레이드
        UpgradeType type = (UpgradeType)Random.Range(0, 3);
        int value = Random.Range(-1, 2);

        // 업그레이드 적용 (SO에 적용 x)
        BossStateManager.Instance.ApplyUpgrade(type, value);

        string result = value switch
        {
            1 => "Success!",
            0 => "Zero",
            -1 => "Fail",
            _ => ""
        };

        resultText.text = $"{type} {result} ({value:+#;-#;0})";

        UpdateStatsUI();
    }

    private void UpdateStatsUI()
    {
        if (statsText == null)
        {
            return;
        }

        if (bossData == null)
        {
            return;
        }

        var s = BossStateManager.Instance;
        if (s == null)
        {
            return;
        }

        float finalAtk = bossData.attackPercent + s.bonusAtk;
        float finalCool = Mathf.Max(0.5f, bossData.cooldown + s.bonusCooldown);
        float finalRange = bossData.range + s.bonusRange;

        statsText.text =
            $"atk : {finalAtk * 100f:+0.#;-0.#;0}%\n" +
            $"skill time: {finalCool:F1}s\n" +
            $"range: {finalRange:+0.0;-0.0;0.0}m";
    }

}
