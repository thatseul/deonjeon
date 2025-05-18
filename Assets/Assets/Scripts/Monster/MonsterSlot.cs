using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 인벤토리 슬롯에 들어가는 몬스터 정보 + 드래그 기능 포함
/// </summary>
public class MonsterSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("몬스터 아이콘 이미지")]
    [Tooltip("몬스터 아이콘이 표시될 UI 이미지")]
    [SerializeField] private Image monsterIcon;

    private MonsterItem assignedMonster;          // 현재 슬롯에 배정된 몬스터
    private GameObject dragIcon;                  // 드래그 시 따라다닐 UI 아이콘
    private Canvas parentCanvas;                  // 최상위 캔버스 (UI 좌표 정렬용)

    /// <summary> 슬롯이 비어있는지 여부 </summary>
    public bool IsEmpty => assignedMonster == null;

    private void Awake()
    {
        parentCanvas = GetComponentInParent<Canvas>();
    }

    /// <summary>
    /// 슬롯에 몬스터 배정 및 아이콘 표시
    /// </summary>
    public void SetMonster(MonsterItem monster)
    {
        assignedMonster = monster;
        if (monsterIcon != null && monster != null && monster.monsterIcon != null)
        {
            monsterIcon.sprite = monster.monsterIcon;
            monsterIcon.enabled = true;
        }
    }

    /// <summary>
    /// 슬롯을 비움 (아이콘 숨김)
    /// </summary>
    public void ClearSlot()
    {
        assignedMonster = null;
        if (monsterIcon != null)
        {
            monsterIcon.sprite = null;
            monsterIcon.enabled = false;
        }
    }

    /// <summary>
    /// 현재 슬롯의 몬스터 정보 반환
    /// </summary>
    public MonsterItem GetAssignedMonster()
    {
        return assignedMonster;
    }

    // ------------------------ 드래그 관련 ------------------------

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (assignedMonster == null) return;

        // 드래그 시 보여줄 아이콘 생성
        dragIcon = new GameObject("DragIcon");
        dragIcon.transform.SetParent(parentCanvas.transform, false);

        Image image = dragIcon.AddComponent<Image>();
        image.sprite = assignedMonster.monsterIcon;
        image.raycastTarget = false;

        dragIcon.transform.SetAsLastSibling(); // UI 가장 위에 위치
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragIcon != null)
            dragIcon.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragIcon != null)
            Destroy(dragIcon);
    }
}
