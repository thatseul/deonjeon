using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableMonster : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Vector2 originalPos;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPos = rectTransform.anchoredPosition;
        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / transform.root.GetComponent<Canvas>().scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = true;

        // ❗ 실패 시 원위치 복귀 (선택)
        // rectTransform.anchoredPosition = originalPos;
    }

    // ==========================================
    // ✅ 드롭 성공 시 QuickSlot에서 호출할 함수
    // ==========================================
    public void ConsumeAndDestroy()
    {
        StartCoroutine(_ConsumeAndDestroyNextFrame());
    }

    private IEnumerator _ConsumeAndDestroyNextFrame()
    {
        // 모든 드래그 이벤트가 끝나도록 1프레임 대기
        yield return null;

        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = true;

        // ✅ 자신(=인벤토리 아이콘)을 파괴
        Destroy(gameObject);
    }
}
