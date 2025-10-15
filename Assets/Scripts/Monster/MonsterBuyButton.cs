using UnityEngine;
using UnityEngine.UI;

public class MonsterBuyButton : MonoBehaviour
{
    [Header("몬스터 데이터 (확률 기반)")]
    [SerializeField] private MonsterPoolManager poolManager;

    [Header("UI 프리팹 및 배치 대상")]
    [SerializeField] private GameObject monsterIconPrefab;       // (InventoryManager에서도 동일 프리팹 사용)
    [SerializeField] private RectTransform monsterInvenPanel;    // ★ inventoryManager.inventoryParent와 동일 객체여야 함
    [SerializeField] private float padding = 20f;

    [Header("버튼 연결")]
    [SerializeField] private Button buyButton;

    [Header("인벤토리 관리")]
    [SerializeField] private MonsterInventoryManager inventoryManager;

    [Header("밸런스")]
    [SerializeField] private int cost = 10; // ✅ 인스펙터에서 조절 가능

    private void Start()
    {
        if (buyButton != null)
        {
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(OnClickBuy);
        }
    }

    public void OnClickBuy()
    {
        // ✅ GameState 확인
        if (GameState.I == null)
        {
            Debug.LogWarning("GameState 없음: 구매 불가");
            return;
        }

        // ★ 수용량 검사 (MonsterInvenPanel 내 아이콘 수 기준)
        if (inventoryManager == null)
        {
            Debug.LogWarning("❌ inventoryManager 미할당");
            return;
        }

        int cap = GameState.I.monsterCapacityUnlocked;
        int cur = inventoryManager.GetCurrentCount();
        if (cur >= cap)
        {
            Debug.Log($"현재 소지 가능 몬스터 수 : {cap}");
            return; // 과금/생성 중단
        }

        // 과금
        if (!GameState.I.TrySpend(cost))
        {
            Debug.Log("❌ 골드 부족!");
            return;
        }

        // 뽑기
        var selectedItem = (poolManager != null) ? poolManager.GetRandomMonsterItem() : null;
        if (selectedItem == null)
        {
            Debug.LogWarning("❗ 몬스터 뽑기 실패");
            // 필요 시: GameState.I.AddGold(cost); // 환불
            return;
        }

        // ★ 아이콘 생성은 InventoryManager에서 '한 번만'
        GameObject iconObj = inventoryManager.AddMonsterToInventory(selectedItem);
        if (iconObj == null)
        {
            Debug.LogWarning("❌ 아이콘 생성 실패 (InventoryManager 확인 필요)");
            return;
        }

        // 패널 안 랜덤 배치 (부모가 MonsterInvenPanel이어야 함)
        if (monsterInvenPanel != null)
        {
            Vector2 size = monsterInvenPanel.rect.size;
            float x = Random.Range(padding, size.x - padding);
            float y = Random.Range(padding, size.y - padding);
            var rt = iconObj.GetComponent<RectTransform>();
            if (rt != null) rt.anchoredPosition = new Vector2(x, y);
        }

        Debug.Log($"🎁 '{selectedItem.monsterName}' 소환됨 (Cost: {cost})  [보유 {cur + 1}/{cap}]");
    }
}
