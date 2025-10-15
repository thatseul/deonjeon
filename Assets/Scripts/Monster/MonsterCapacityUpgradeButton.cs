using UnityEngine;
using UnityEngine.UI;

public class MonsterCapacityUpgradeButton : MonoBehaviour
{
    [SerializeField] private Button upgradeButton;

    // 2~8마리로 확장할 때 필요한 비용(인덱스: 현재수용량-1)
    [SerializeField] private int[] capacityCosts = { 50, 250, 1250, 6250, 31250, 156250, 781250 };

    private void Start()
    {
        if (upgradeButton != null)
        {
            upgradeButton.onClick.RemoveAllListeners();
            upgradeButton.onClick.AddListener(OnClickUpgrade);
        }
    }

    private void OnClickUpgrade()
    {
        var gs = GameState.I;
        if (gs == null) { Debug.LogWarning("GameState 없음"); return; }

        int cap = gs.monsterCapacityUnlocked;
        if (cap >= GameState.monsterCapacityMax)
        {
            Debug.Log("🔒 최대 수용량(8)입니다.");
            return;
        }

        int idx = cap - 1; // cap=1일 때 idx=0 → 2마리 해금 비용
        if (idx < 0 || idx >= capacityCosts.Length)
        {
            Debug.LogWarning("비용 테이블 범위 초과");
            return;
        }

        int price = capacityCosts[idx];
        if (!gs.TrySpend(price))
        {
            Debug.Log($"❌ 골드 부족! 필요: {price}");
            return;
        }

        gs.monsterCapacityUnlocked = Mathf.Min(cap + 1, GameState.monsterCapacityMax);
        Debug.Log($"✅ 인벤 확장: {cap} → {gs.monsterCapacityUnlocked} (소모 {price})");
    }
}
