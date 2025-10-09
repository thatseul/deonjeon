using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 퀵슬롯: 몬스터 드롭/소환/삭제 및 UI 관리
/// </summary>
public class QuickSlot : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    [Header("몬스터 소환 위치")]
    [SerializeField] private Transform summonPosition;

    [Header("던전 능력치 매니저")]
    [SerializeField] private DungeonStatsManager statsManager;

    [Header("퀵슬롯 이미지")]
    [SerializeField] private Image slotImage;

    [Header("선택 UI")]
    [SerializeField] private GameObject slotHighlight;

    [SerializeField] private DungeonSlotManager dungeonSlotManager;
    [SerializeField] private MonsterInventoryManager inventoryManager;

    private GameObject summonedMonster;    // 현재 슬롯에서 소환된 몬스터
    private MonsterItem currentItem;       // 현재 슬롯에 배치된 아이템

    private void Start()
    {
        if (slotHighlight != null)
            slotHighlight.SetActive(false);
    }

    public void OnDrop(PointerEventData eventData)
    {
        var dragged = eventData.pointerDrag;
        if (dragged == null) return;

        var itemSlot = dragged.GetComponent<MonsterSlot>();
        if (itemSlot == null) return;

        MonsterItem item = itemSlot.GetAssignedMonster();
        if (item == null || item.monsterPrefab == null) return;

        // 기존 몬스터 제거
        DeleteMonster();

        // 새 몬스터 생성
        summonedMonster = Instantiate(item.monsterPrefab, summonPosition.position, Quaternion.identity);
        var monster = summonedMonster.GetComponent<Monster>();
        if (monster != null)
        {
            float hp   = statsManager.GetStatValue("HP");
            float atk  = statsManager.GetStatValue("ATK");
            float aspd = statsManager.GetStatValue("ASPD");
            monster.Init(hp, atk, aspd, item.statScaleRatio);
            monster.SetQuickSlotOwner(this);
        }

        // 퀵슬롯 UI 표시
        if (slotImage != null)
        {
            slotImage.sprite = item.monsterIcon;
            slotImage.color  = Color.white;
        }

        currentItem = item;

        // ✅ 인벤토리 아이콘 삭제 (드래그한 아이콘)
        var draggable = dragged.GetComponent<DraggableMonster>();
        if (draggable != null)
            draggable.ConsumeAndDestroy();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (slotHighlight != null)
            slotHighlight.SetActive(true);

        dungeonSlotManager.SelectSlot(this);
    }

    public void DeleteMonster()
    {
        if (summonedMonster != null)
        {
            Destroy(summonedMonster);
            summonedMonster = null;
        }

        // 인벤토리 아이콘이 남았을 경우 (혹시 삭제 안 된 경우)
        if (currentItem != null && inventoryManager != null)
        {
            inventoryManager.RemoveOneIcon(currentItem);
            currentItem = null;
        }

        ClearSlot();
    }

    public void ClearSlot()
    {
        if (slotImage != null)
        {
            slotImage.sprite = null;
            slotImage.color  = new Color(1, 1, 1, 0);
        }

        Deselect();
        Debug.Log("🧼 슬롯 클리어됨");
    }

    public void Highlight()
    {
        if (slotHighlight != null)
            slotHighlight.SetActive(true);
    }

    public void Deselect()
    {
        if (slotHighlight != null)
            slotHighlight.SetActive(false);
    }
}
