using UnityEngine;

public enum SkillGrade { Normal, Rare, Epic, Legendary }

[CreateAssetMenu(fileName = "SkillData", menuName = "Boss/Skill Data")]
public class SkillData : ScriptableObject
{
    public string skillName;
    public float baseAttackPercent;
    public float baseCooldown;
    public float baseRange;
    public SkillGrade grade;

    public int GetMaxLevel(int dungeonLevel)
    {
        return grade switch
        {
            SkillGrade.Normal => Mathf.Min(3 + dungeonLevel, 5),
            SkillGrade.Rare => Mathf.Min(5 + dungeonLevel, 7),
            SkillGrade.Epic => Mathf.Min(7 + dungeonLevel, 10),
            SkillGrade.Legendary => Mathf.Min(10 + dungeonLevel, 15),
            _ => 3
        };
    }
}
