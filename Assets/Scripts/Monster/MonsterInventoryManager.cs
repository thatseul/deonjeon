using UnityEngine;

/// <summary>
/// 몬스터 인벤토리에 프리팹 아이콘을 직접 생성하는 방식
/// </summary>
public class MonsterInventoryManager : MonoBehaviour
{
    [Tooltip("MonsterIconPrefab을 붙일 부모 오브젝트 (MonsterInvenPanel)")]
    [SerializeField] private Transform inventoryParent;

    [Tooltip("몬스터 아이콘 프리팹")]
    [SerializeField] private GameObject monsterIconPrefab;

    /// <summary>
    /// 몬스터 아이콘을 인벤토리 패널에 추가함
    /// </summary>
    public bool AddMonsterToInventory(MonsterItem item)
    {
        if (monsterIconPrefab == null || inventoryParent == null)
        {
            Debug.LogWarning("❌ 프리팹이나 부모가 비어 있음!");
            return false;
        }

        GameObject iconObj = Instantiate(monsterIconPrefab, inventoryParent);
        MonsterIcon icon = iconObj.GetComponent<MonsterIcon>();

        if (icon != null)
        {
            icon.SetMonsterItem(item);
            return true;
        }

        Debug.LogWarning("❌ MonsterIcon 스크립트를 프리팹에 붙이지 않았거나 문제 있음");
        return false;
    }
}
