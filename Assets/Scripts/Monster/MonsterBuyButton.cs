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

    private void Start()
    {
        if (buyButton != null)
        {
            buyButton.onClick.RemoveAllListeners(); // ✅ 기존 리스너 제거
            buyButton.onClick.AddListener(OnClickBuy);
        }
    }
    public void OnClickBuy()
    {
        const int cost = 10; // ✅ 고정 비용 사용

        if (!GoldManager.Instance.SpendGold(cost))
        {
            Debug.Log("❌ 골드 부족!");
            return;
        }

        MonsterItem selectedItem = poolManager.GetRandomMonsterItem();

        if (selectedItem == null)
        {
            Debug.LogWarning("❗ 몬스터 뽑기 실패");
            return;
        }

        // 아이템을 인벤토리에 추가하고 UI 생성
        inventoryManager.AddMonsterToInventory(selectedItem);

        GameObject icon = Instantiate(monsterIconPrefab, monsterInvenPanel);
        icon.GetComponent<MonsterIcon>()?.SetMonsterItem(selectedItem);

        MonsterSlot slot = icon.GetComponent<MonsterSlot>();
        if (slot != null)
        {
            slot.SetMonster(selectedItem);
        }

        Vector2 size = monsterInvenPanel.rect.size;
        float x = Random.Range(padding, size.x - padding);
        float y = Random.Range(padding, size.y - padding);
        icon.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);

        Debug.Log($"🎁 '{selectedItem.monsterName}' 소환됨 (Cost: {cost})");
    }

}
