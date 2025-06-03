using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MonsterIcon : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;

    private MonsterItem item;

    public void SetMonsterItem(MonsterItem newItem)
    {
        item = newItem;

        if (iconImage != null && item.monsterIcon != null)
            iconImage.sprite = item.monsterIcon;

        if (nameText != null)
            nameText.text = item.monsterName;
    }

    // 여기에 드래그 & 삭제 관련 기능 추가할 예정
}
