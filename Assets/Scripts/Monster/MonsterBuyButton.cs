using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MonsterBuyButton : MonoBehaviour
{
    [Header("몬스터 데이터 (확률 기반)")]
    [SerializeField] private MonsterPoolManager poolManager;

    [Header("UI 프리팹 및 배치 대상")]
    [SerializeField] private GameObject monsterIconPrefab;
    [SerializeField] private RectTransform monsterInvenPanel;
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
        // ✅ GameState 사용으로 통일
        if (GameState.I == null)
        {
            Debug.LogWarning("GameState 없음: 구매 불가");
            return;
        }

        if (!GameState.I.TrySpend(cost))
        {
            Debug.Log("❌ 골드 부족!");
            return;
        }

        var selectedItem = poolManager ? poolManager.GetRandomMonsterItem() : null;
        if (selectedItem == null)
        {
            Debug.LogWarning("❗ 몬스터 뽑기 실패");
            // 실패 시 골드 환불할 거면 아래 주석 해제
            // GameState.I.AddGold(cost);
            return;
        }

        // 인벤토리 등록 + 아이콘 생성
        if (inventoryManager) inventoryManager.AddMonsterToInventory(selectedItem);

        var icon = Instantiate(monsterIconPrefab, monsterInvenPanel);
        icon.GetComponent<MonsterIcon>()?.SetMonsterItem(selectedItem);

        var slot = icon.GetComponent<MonsterSlot>();
        if (slot != null) slot.SetMonster(selectedItem);

        // 패널 안 랜덤 배치
        Vector2 size = monsterInvenPanel.rect.size;
        float x = Random.Range(padding, size.x - padding);
        float y = Random.Range(padding, size.y - padding);
        icon.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);

        Debug.Log($"🎁 '{selectedItem.monsterName}' 소환됨 (Cost: {cost})");
    }
}
