using UnityEngine;
using System.Collections;

public class MonsterInventoryManager : MonoBehaviour
{
    [SerializeField] private Transform inventoryParent;
    [SerializeField] private GameObject monsterIconPrefab;
    [SerializeField] private float padding = 20f;

    private RectTransform invenRect;

    private void Awake()
    {
        invenRect = inventoryParent.GetComponent<RectTransform>();
    }

    private void Start()
    {
        var gs = GameState.I;
        if (gs == null) return;
        if (inventoryParent == null) return;

        // 기존 UI 전부 제거
        for (int i = inventoryParent.childCount - 1; i >= 0; i--)
            Destroy(inventoryParent.GetChild(i).gameObject);

        // GameState에 저장된 것들을 좌표 포함해서 복원
        foreach (var data in gs.ownedMonsters)
        {
            var iconObj = Instantiate(monsterIconPrefab, inventoryParent);

            var icon = iconObj.GetComponent<MonsterIcon>();
            icon.SetMonsterItem(data.item);

            var slot = iconObj.GetComponent<MonsterSlot>();
            slot.SetMonster(data.item);

            var rt = iconObj.GetComponent<RectTransform>();
            rt.anchoredPosition = data.anchoredPosition;
        }

        Debug.Log($"[Inven Restore] {gs.ownedMonsters.Count}마리 복원");
    }

    public int GetCurrentCount()
    {
        return GameState.I.ownedMonsters.Count;
    }

    public GameObject AddMonsterToInventory(MonsterItem item)
    {
        if (GameState.I == null) return null;
        if (monsterIconPrefab == null || inventoryParent == null) return null;

        var iconObj = Instantiate(monsterIconPrefab, inventoryParent);

        var icon = iconObj.GetComponent<MonsterIcon>();
        icon.SetMonsterItem(item);

        var slot = iconObj.GetComponent<MonsterSlot>();
        slot.SetMonster(item);

        // 랜덤 위치
        Vector2 size = invenRect.rect.size;
        float x = Random.Range(padding, size.x - padding);
        float y = Random.Range(padding, size.y - padding);
        Vector2 pos = new Vector2(x, y);

        iconObj.GetComponent<RectTransform>().anchoredPosition = pos;

        // GameState에 좌표 포함 저장
        GameState.I.AddOwnedMonster(item, pos);

        return iconObj;
    }

    /// <summary>
    /// ❗ 이제 이 함수는 "데이터만" 줄인다. UI는 건들지 않음.
    /// 실제 아이콘 파괴는 DraggableMonster.ConsumeAndDestroy()가 담당.
    /// </summary>
    public void RemoveOneIcon(MonsterItem item)
    {
        var gs = GameState.I;
        if (gs == null) return;

        gs.RemoveOwnedMonster(item);
    }
}
