using UnityEngine;

[System.Serializable]
public class BossSkill
{
    public string skillName;
    public float baseAttack;
    public float baseCooldown;
    public float baseRange;
    public SkillGrade grade;
    public int level;

    public BossSkill(string skillName, float baseAttack, float baseCooldown, float baseRange, SkillGrade grade)
    {
        this.skillName = skillName;
        this.baseAttack = baseAttack;
        this.baseCooldown = baseCooldown;
        this.baseRange = baseRange;
        this.grade = grade;
        this.level = 0;
    }

    public BossSkill(SkillData data)
    {
        skillName = data.skillName;
        baseAttack = data.baseAttackPercent;
        baseCooldown = data.baseCooldown;
        baseRange = data.baseRange;
        grade = data.grade;
        level = 0;
    }


    public float GetFinalAttack()
    {
        return baseAttack + level * 1.0f;
    }

    public float GetFinalCooldown()
    {
        return Mathf.Max(0.1f, baseCooldown - level * 0.1f);
    }

    public float GetFinalRange()
    {
        return baseRange + level * 0.2f;
    }

    public void LevelUp(int dungeonLevel)
    {
        int maxLevel = GetMaxLevel(dungeonLevel);
        if (level < maxLevel)
            level++;
    }

    private int GetMaxLevel(int dungeonLevel)
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
