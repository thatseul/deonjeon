using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 몬스터 인벤토리 슬롯에 들어가는 정보 및 드래그 기능
/// </summary>
public class MonsterSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("몬스터 아이콘 이미지")]
    [Tooltip("몬스터 아이콘이 표시될 UI 이미지")]
    [SerializeField] private Image monsterIcon;

    private MonsterItem assignedMonster;           // 현재 슬롯에 배정된 몬스터
    private GameObject dragIcon;                   // 드래그 시 따라다닐 임시 아이콘
    private Canvas parentCanvas;                   // 상위 캔버스
    private CanvasGroup canvasGroup;               // 드래그 중 Raycast 무시를 위한 그룹

    private void Awake()
    {
        parentCanvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    /// <summary> 슬롯에 몬스터 세팅 </summary>
    public void SetMonster(MonsterItem monster)
    {
        assignedMonster = monster;
        if (monsterIcon != null && monster != null && monster.monsterIcon != null)
        {
            monsterIcon.sprite = monster.monsterIcon;
            monsterIcon.enabled = true;
        }
    }

    /// <summary> 슬롯 비우기 </summary>
    public void ClearSlot()
    {
        assignedMonster = null;
        if (monsterIcon != null)
        {
            monsterIcon.sprite = null;
            monsterIcon.enabled = false;
        }
    }

    /// <summary> 현재 슬롯의 몬스터 정보 반환 </summary>
    public MonsterItem GetAssignedMonster()
    {
        return assignedMonster;
    }

    // ------------------------ 드래그 처리 ------------------------

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (assignedMonster == null) return;

        // 원본 아이콘은 일단 안보이게
        if (monsterIcon != null)
            monsterIcon.enabled = false;

        canvasGroup.blocksRaycasts = false;

        dragIcon = new GameObject("DragIcon");
        dragIcon.transform.SetParent(parentCanvas.transform, false);

        Image image = dragIcon.AddComponent<Image>();
        image.sprite = assignedMonster.monsterIcon;
        image.raycastTarget = false;

        dragIcon.transform.SetAsLastSibling();
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

        canvasGroup.blocksRaycasts = true;

        // 다시 보이게
        if (assignedMonster != null && monsterIcon != null)
            monsterIcon.enabled = true;
    }
}
