using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public class MonsterBuyButton : MonoBehaviour
{
    [Tooltip("소환 가능한 몬스터 리스트")]
    [SerializeField] private List<MonsterItem> availableMonsters;

    [SerializeField] private MonsterInventoryManager inventoryManager;
    [SerializeField] private Button buyButton;

    private void Start()
    {
        if (buyButton != null)
            buyButton.onClick.AddListener(BuyRandomMonster);
    } 

    private void BuyRandomMonster()
    {
        if (availableMonsters == null || availableMonsters.Count == 0)
        {
            Debug.LogWarning("❌ 몬스터 리스트가 비어 있습니다.");
            return;
        }

        MonsterItem selected = availableMonsters[Random.Range(0, availableMonsters.Count)];

        if (GoldManager.Instance.GetCurrentGold() >= selected.cost)
        {
            bool success = inventoryManager.AddMonsterToInventory(selected);
            if (success)
            {
                GoldManager.Instance.SpendGold(selected.cost);
                Debug.Log($"🛒 랜덤 구매 성공: {selected.monsterName}");
            }
            else
            {
                Debug.LogWarning("❌ 인벤토리에 빈 슬롯이 없습니다!");
            }
        }
        else
        {
            Debug.LogWarning("💸 골드 부족!");
        }
    }
}
