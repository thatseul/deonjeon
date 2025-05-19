using UnityEngine;

public enum UpgradeType
{
    AttackPercent,
    CooldownReduce,
    RangeIncrease
}

[CreateAssetMenu(fileName = "BossData", menuName = "Boss/Boss Data")]
public class BossData : ScriptableObject
{
    public float attackPercent = 1.0f;
    public float cooldown = 5.0f;
    public float range = 1.0f;

    public void ApplyUpgrade(UpgradeType type, int value)
    {
        switch (type)
        {
            case UpgradeType.AttackPercent:
                attackPercent += value * 0.1f;
                break;
            case UpgradeType.CooldownReduce:
                cooldown -= value * 0.2f;
                cooldown = Mathf.Max(0.5f, cooldown);
                break;
            case UpgradeType.RangeIncrease:
                range += value * 0.2f;
                break;
        }
    }
}
