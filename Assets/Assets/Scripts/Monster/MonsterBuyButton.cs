using UnityEngine;
using UnityEngine.UI;

public class MonsterBuyButton : MonoBehaviour
{
    [Tooltip("구매할 몬스터 정보")]
    [SerializeField] private MonsterItem monsterToBuy;

    [Tooltip("인벤토리 매니저")]
    [SerializeField] private MonsterInventoryManager inventoryManager;

    [Tooltip("구매 버튼")]
    [SerializeField] private Button buyButton;

    private void Start()
    {
        if (buyButton != null)
        {
            buyButton.onClick.AddListener(BuyMonster);
        }
    }

    private void BuyMonster()
    {
        if (GoldManager.Instance.GetCurrentGold() >= monsterToBuy.cost)
        {
            bool success = inventoryManager.AddMonsterToInventory(monsterToBuy);

            if (success)
            {
                GoldManager.Instance.SpendGold(monsterToBuy.cost);
                Debug.Log($"🛒 {monsterToBuy.monsterName} 구매 성공!");
            }
            else
            {
                Debug.LogWarning("❌ 구매 실패: 인벤토리에 공간 없음");
            }
        }
        else
        {
            Debug.LogWarning("💸 골드 부족!");
        }
    }
}
