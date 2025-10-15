using UnityEngine;

/// <summary>
/// 몬스터 인벤토리에 프리팹 아이콘을 직접 생성/삭제 관리
/// </summary>
public class MonsterInventoryManager : MonoBehaviour
{
    [Tooltip("MonsterIconPrefab을 붙일 부모 오브젝트 (MonsterInvenPanel)")]
    [SerializeField] private Transform inventoryParent;

    [Tooltip("몬스터 아이콘 프리팹")]
    [SerializeField] private GameObject monsterIconPrefab;

    public int GetCurrentCount()
{
    if (inventoryParent == null) return 0;
    return inventoryParent.childCount;
}

    /// <summary>
    /// 몬스터 아이콘을 인벤토리 패널에 추가함
    /// </summary>
    public GameObject AddMonsterToInventory(MonsterItem item)
    {
        if (monsterIconPrefab == null || inventoryParent == null)
        {
            Debug.LogWarning("❌ 프리팹이나 부모가 비어 있음!");
            return null;
        }

        GameObject iconObj = Instantiate(monsterIconPrefab, inventoryParent);

        // 아이콘 표시(스프라이트/텍스트 등)
        var icon = iconObj.GetComponent<MonsterIcon>();
        if (icon != null) icon.SetMonsterItem(item);

        // 드래그를 위한 슬롯 데이터 주입
        var slot = iconObj.GetComponent<MonsterSlot>();
        if (slot != null) slot.SetMonster(item);

        return iconObj;
    }

    /// <summary>
    /// 해당 MonsterItem에 해당하는 아이콘 1개를 찾아 삭제 (여러 개 있으면 첫 번째)
    /// </summary>
    public bool RemoveOneIcon(MonsterItem item)
    {
        if (inventoryParent == null || item == null) return false;

        for (int i = 0; i < inventoryParent.childCount; i++)
        {
            var t = inventoryParent.GetChild(i);
            var slot = t.GetComponent<MonsterSlot>();
            if (slot == null) continue;

            // 인벤토리 UI가 들고 있는 아이템이 같은지 비교
            var assigned = slot.GetAssignedMonster();
            if (assigned == item) // 동일 참조 비교 (동일한 ScriptableObject라면 참조 동일)
            {
                Destroy(t.gameObject);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 해당 MonsterItem에 해당하는 모든 아이콘 삭제(필요 시)
    /// </summary>
    public int RemoveAllIcons(MonsterItem item)
    {
        if (inventoryParent == null || item == null) return 0;

        int removed = 0;
        // 역순으로 지워야 안전
        for (int i = inventoryParent.childCount - 1; i >= 0; i--)
        {
            var t = inventoryParent.GetChild(i);
            var slot = t.GetComponent<MonsterSlot>();
            if (slot == null) continue;

            if (slot.GetAssignedMonster() == item)
            {
                Destroy(t.gameObject);
                removed++;
            }
        }
        return removed;
    }
}
