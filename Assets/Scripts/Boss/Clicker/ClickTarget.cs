using UnityEngine;
using UnityEngine.EventSystems;

public class ClickTarget : MonoBehaviour, IPointerClickHandler
{
    private MiniGameController controller;
    private bool isGood;

    public void Init(MiniGameController ctrl)
    {
        controller = ctrl;
        isGood = gameObject.tag == "Good"; // 태그 확인
    }

    // UI 클릭 이벤트
    public void OnPointerClick(PointerEventData eventData)
    {
        if (isGood)
        {
            controller.AddScore();
        }
        else
        {
            controller.EndGame(false);
        }

        Destroy(gameObject);
    }
}
