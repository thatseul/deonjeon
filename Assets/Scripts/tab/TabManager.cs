using UnityEngine;
using UnityEngine.UI;

public class TabManager : MonoBehaviour
{
    [SerializeField] private GameObject statPanel;
    [SerializeField] private GameObject monsterPanel;

    [Header("탭 버튼")]
    [SerializeField] private Button statTabButton;
    [SerializeField] private Button monsterTabButton;

    private Color activeColor = Color.green;
    private Color inactiveColor = Color.white;

    private void Start()
    {
        ShowStatPanel();
    }

    public void ShowStatPanel()
    {
        statPanel.SetActive(true);
        monsterPanel.SetActive(false);

        SetTabColors(statTabButton, true);
        SetTabColors(monsterTabButton, false);
    }

    public void ShowMonsterPanel()
    {
        statPanel.SetActive(false);
        monsterPanel.SetActive(true);

        SetTabColors(statTabButton, false);
        SetTabColors(monsterTabButton, true);
    }

    private void SetTabColors(Button button, bool isActive)
    {
        if (button == null) return;

        var colors = button.colors;

        Color targetColor = isActive ? activeColor : inactiveColor;

        colors.normalColor = targetColor;
        colors.highlightedColor = targetColor;
        colors.pressedColor = targetColor;
        colors.selectedColor = targetColor;
        colors.disabledColor = Color.gray; // 비활성화된 상태는 회색

        button.colors = colors;
    }

}
