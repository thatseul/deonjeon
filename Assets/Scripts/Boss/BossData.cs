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
}
