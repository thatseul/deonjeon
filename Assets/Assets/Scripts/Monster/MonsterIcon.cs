using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MonsterIcon : MonoBehaviour
{
    public MonsterItem item;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;

    public void Setup(MonsterItem item)
    {
        this.item = item;
        
        if (iconImage != null)
            iconImage.sprite = item.monsterIcon;

        if (nameText != null)
            nameText.text = item.monsterName;
    }
}
