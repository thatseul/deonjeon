using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class QuickSlot : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    [Header("몬스터 소환 위치")]
    [SerializeField] private Transform summonPosition;

    [Header("던전 능력치 매니저")]
    [SerializeField] private DungeonStatsManager statsManager;

    [Header("퀵슬롯 이미지")]
    [SerializeField] private Image slotImage;

    [Header("선택 UI")]
    [Tooltip("빨간 테두리용 UI 오브젝트")]
    [SerializeField] private GameObject slotHighlight;

    [Tooltip("퀵슬롯 매니저")]
    [SerializeField] private DungeonSlotManager dungeonSlotManager;

    private GameObject summonedMonster;

    private void Start()
    {
        if (slotHighlight != null)
            slotHighlight.SetActive(false); // 처음엔 숨김
    }

    public void OnDrop(PointerEventData eventData)
    {
        var draggedSlot = eventData.pointerDrag?.GetComponent<MonsterSlot>();
        if (draggedSlot == null) return;

        MonsterItem item = draggedSlot.GetAssignedMonster();
        if (item == null || item.monsterPrefab == null) return;

        summonedMonster = Instantiate(item.monsterPrefab, summonPosition.position, Quaternion.identity);
        Monster monster = summonedMonster.GetComponent<Monster>();

        if (monster != null)
        {
            float hp = statsManager.GetStatValue("HP");
            float atk = statsManager.GetStatValue("ATK");
            float aspd = statsManager.GetStatValue("ASPD");
            monster.Init(hp, atk, aspd, item.statScaleRatio);

            // ✅ 몬스터에 슬롯 연결
            monster.SetQuickSlotOwner(this);
        }

        if (slotImage != null)
        {
            slotImage.sprite = item.monsterIcon;
            slotImage.color = Color.white;
        }

        draggedSlot.ClearSlot();
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

        ClearSlot();
    }

    public void ClearSlot()
    {
        summonedMonster = null;

        if (slotImage != null)
        {
            slotImage.sprite = null;
            slotImage.color = new Color(1, 1, 1, 0); // 완전 투명
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
