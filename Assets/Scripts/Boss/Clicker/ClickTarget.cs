using UnityEngine;

public class ClickTarget : MonoBehaviour
{
    private MiniGameController controller;
    private bool isGood;

    public void Init(MiniGameController ctrl)
    {
        controller = ctrl;
        isGood = gameObject.tag == "good";
    }

    private void OnMouseDown()
    {
        if (isGood)
        {
            controller.AddScore();
        }
        else
        {
            // 실패 시 효과 등
        }
        Destroy(gameObject);
    }
}
