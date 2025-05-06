// MonsterSlot.cs
using UnityEngine;
using UnityEngine.UI;

public class MonsterSlot : MonoBehaviour
{
    [Header("몬스터 이미지 표시용")]
    [SerializeField] private Image monsterIcon;

    private MonsterItem assignedMonster;

    public bool IsEmpty => assignedMonster == null;

    /// <summary>
    /// 슬롯에 몬스터 배치
    /// </summary>
    public void AssignMonster(MonsterItem monster)
    {
        assignedMonster = monster;
        if (monsterIcon != null && monster != null)
        {
            monsterIcon.sprite = monster.monsterIcon; // MonsterItem에서 Sprite 반환하는 메서드 필요
            monsterIcon.enabled = true;
        }
    }

    /// <summary>
    /// 슬롯 비우기
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
    /// 현재 배정된 몬스터 가져오기
    /// </summary>
    public MonsterItem GetAssignedMonster()
    {
        return assignedMonster;
    }

    public void SetMonster(MonsterItem monster)
{
    assignedMonster = monster;

    if (monsterIcon != null && monster.monsterIcon != null)
    {
        monsterIcon.sprite = monster.monsterIcon;
        monsterIcon.enabled = true;
    }
}
}
