using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class QuickSlot : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    [Header("몬스터 스폰")]
    [SerializeField] private Transform summonPosition;
    [SerializeField] private float spawnOffsetX = 0.8f;

    [Header("던전 스탯")]
    [SerializeField] private DungeonStatsManager statsManager;

    [Header("UI")]
    [SerializeField] private Image slotImage;
    [SerializeField] private GameObject slotHighlight;

    [Header("매니저")]
    [SerializeField] private DungeonSlotManager dungeonSlotManager;
    [SerializeField] private MonsterInventoryManager inventoryManager;

    [Header("슬롯 인덱스")]
    [SerializeField] private int slotIndex = -1;

    private GameObject summonedMonster;
    private MonsterItem currentItem;


    /* ============================================================
       ⬇️  자동 참조 연결
    ============================================================ */

    private void Awake()
    {
        if (statsManager == null)
            statsManager = DungeonStatsManager.Instance;

        if (inventoryManager == null)
            inventoryManager = FindObjectOfType<MonsterInventoryManager>();

        if (dungeonSlotManager == null)
            dungeonSlotManager = FindObjectOfType<DungeonSlotManager>();
    }

    private void Start()
    {
        slotHighlight?.SetActive(false);
        RestoreFromGameState();
    }


    /* ============================================================
       ⬇️  저장된 슬롯 복원
    ============================================================ */

    private void RestoreFromGameState()
    {
        if (GameState.I == null) return;
        if (slotIndex < 0) return;

        MonsterItem saved = GameState.I.GetQuickSlotItem(slotIndex);
        if (saved == null) return;

        // 인벤 존재 여부와는 상관없이, 퀵슬롯은 독립적으로 복원
        currentItem = saved;

        SpawnFieldMonster();
        UpdateSlotVisual(saved);
    }


    /* ============================================================
       ⬇️  필드 몬스터 스폰
    ============================================================ */

    private void SpawnFieldMonster()
    {
        if (summonPosition == null) return;
        if (currentItem == null || currentItem.monsterPrefab == null) return;
        if (statsManager == null) return;

        // 기존 소환 몬스터 정리
        if (summonedMonster != null)
        {
            Destroy(summonedMonster);
            summonedMonster = null;
        }

        Vector3 pos = summonPosition.position + new Vector3(spawnOffsetX, 0f, 0f);
        summonedMonster = Instantiate(currentItem.monsterPrefab, pos, Quaternion.identity);

        Monster monster = summonedMonster.GetComponent<Monster>();
        if (monster != null)
        {
            float hp = statsManager.GetStatValue("HP");
            float atk = statsManager.GetStatValue("ATK");
            float aspd = statsManager.GetStatValue("ASPD");

            monster.Init(hp, atk, aspd, currentItem.statScaleRatio);
            monster.SetQuickSlotOwner(this);
        }
    }


    /* ============================================================
       ⬇️  슬롯 UI 갱신
    ============================================================ */

    private void UpdateSlotVisual(MonsterItem item)
    {
        if (slotImage == null) return;

        if (item != null && item.monsterIcon != null)
        {
            slotImage.sprite = item.monsterIcon;
            slotImage.color = Color.white;
        }
        else
        {
            slotImage.sprite = null;
            slotImage.color = new Color(1, 1, 1, 0);
        }
    }


    /* ============================================================
       ⬇️  드래그 드롭 (인벤 → 퀵슬롯)
    ============================================================ */

    public void OnDrop(PointerEventData eventData)
    {
        var dragged = eventData.pointerDrag;
        if (dragged == null) return;

        var slot = dragged.GetComponent<MonsterSlot>();
        if (slot == null) return;

        MonsterItem item = slot.GetAssignedMonster();
        if (item == null || item.monsterPrefab == null) return;

        // 1) 기존 퀵슬롯 몬스터 제거 (퀵슬롯/필드만 비움, 인벤X)
        DeleteMonster();

        // 2) 인벤 데이터 1개 감소 (UI는 아직 그대로)
        if (inventoryManager != null)
        {
            inventoryManager.RemoveOneIcon(item);
        }

        // 3) 드래그하던 아이콘 UI 제거 (실제 화면에서 사라지는 부분)
        var draggable = dragged.GetComponent<DraggableMonster>();
        if (draggable != null)
        {
            draggable.ConsumeAndDestroy();
        }

        // 4) 퀵슬롯에 세팅 + 필드 몬스터 소환
        currentItem = item;
        SpawnFieldMonster();
        UpdateSlotVisual(item);

        // 5) GameState 퀵슬롯 저장
        if (GameState.I != null)
            GameState.I.SetQuickSlotItem(slotIndex, item);
    }

    /* ============================================================
       ⬇️  슬롯 선택
    ============================================================ */

    public void OnPointerClick(PointerEventData eventData)
    {
        slotHighlight?.SetActive(true);
        dungeonSlotManager?.SelectSlot(this);
    }


    /* ============================================================
       ⬇️  슬롯 정리 (퀵슬롯만 비움)
    ============================================================ */

    public void DeleteMonster()
    {
        if (summonedMonster != null)
        {
            Destroy(summonedMonster);
            summonedMonster = null;
        }

        currentItem = null;

        if (GameState.I != null)
            GameState.I.SetQuickSlotItem(slotIndex, null);

        ClearSlot();
    }

    public void ClearSlot()
    {
        UpdateSlotVisual(null);
        Deselect();
    }

    public void Highlight() => slotHighlight?.SetActive(true);
    public void Deselect() => slotHighlight?.SetActive(false);


    /* ============================================================
       ⬇️  몬스터 사망 콜백 (Monster.cs → Die()에서 호출)
    ============================================================ */

    public void OnMonsterDied(Monster monster)
    {
        // 죽어도 인벤에는 영향 없음 (이미 인벤에서 빠진 상태)
        DeleteMonster();
    }
}
