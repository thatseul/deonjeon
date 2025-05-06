// MonsterInventoryManager.cs
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 몬스터 인벤토리 전체를 관리하는 매니저.
/// 구매된 몬스터를 슬롯에 채워 넣고, 슬롯 정보를 관리한다.
/// </summary>
public class MonsterInventoryManager : MonoBehaviour
{
    [Tooltip("슬롯 개수 (임시로 3칸)")]
    [SerializeField] private int slotCount = 3;

    [Tooltip("몬스터 슬롯 프리팹")]
    [SerializeField] private GameObject monsterSlotPrefab;

    [Tooltip("슬롯들이 들어갈 부모 오브젝트")]
    [SerializeField] private Transform slotParent;

    private List<MonsterSlot> monsterSlots = new List<MonsterSlot>();

    private void Start()
    {
        InitSlots();
    }

    /// <summary>
    /// 슬롯을 초기화해서 화면에 배치함
    /// </summary>
    private void InitSlots()
    {
        for (int i = 0; i < slotCount; i++)
        {
            GameObject slotObj = Instantiate(monsterSlotPrefab, slotParent);
            MonsterSlot slot = slotObj.GetComponent<MonsterSlot>();
            if (slot != null)
            {
                monsterSlots.Add(slot);
            }
        }
    }

    /// <summary>
    /// 빈 슬롯을 찾아서 몬스터를 배치함
    /// </summary>
    public bool AddMonsterToInventory(MonsterItem monsterItem)
    {
        foreach (var slot in monsterSlots)
        {
            if (slot.IsEmpty)
            {
                slot.SetMonster(monsterItem);  // ✅ 에러 해결
                return true;
            }
        }

        Debug.LogWarning("⚠️ 몬스터 인벤토리에 빈 슬롯이 없습니다!");
        return false;
    }

}
