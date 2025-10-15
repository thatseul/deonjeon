using System;
using UnityEngine;

[Serializable]
public class DungeonStat
{
    [Header("스탯 이름 (예: HP, ATK, ASPD)")]
    public string name;

    [Tooltip("현재 업그레이드 레벨")]
    public int upgradeLevel;

    [Tooltip("기본 수치")]
    public float baseValue;

    [Header("업그레이드 비용 설정")]
    public int baseCost = 1;         // 스탯별 시작 가격
    public float costMultiplier = 1.5f; // 레벨업마다 가격 배수

    public float CurrentValue
    {
        get
        {
            if (name == "ASPD")
            {
                float aspd = baseValue;
                for (int i = 0; i < upgradeLevel; i++) aspd *= 1.02f;
                return Mathf.Min(aspd, 3f);
            }
            return baseValue * (1 + 0.2f * upgradeLevel);
        }
    }

    public DungeonStat(string name, float baseValue)
    {
        this.name = name;
        this.baseValue = baseValue;
        this.upgradeLevel = 0;
    }

    public void Upgrade()
    {
        upgradeLevel++;
    }

    /// <summary>현재 업그레이드 비용 = baseCost * (costMultiplier^upgradeLevel)</summary>
    public int GetUpgradeCost()
    {
        // upgradeLevel은 “현재 레벨”이므로, 다음 업그레이드 가격 = baseCost * 1.5^현재레벨
        return Mathf.RoundToInt(baseCost * Mathf.Pow(costMultiplier, upgradeLevel));
    }

    public float GetValueAtLevel(int level)
    {
        if (name == "ASPD")
        {
            float aspd = baseValue;
            for (int i = 0; i < level; i++) aspd *= 1.02f;
            return Mathf.Min(aspd, 3f);
        }
        return baseValue * (1 + 0.2f * level);
    }

    public float GetNextValue()
    {
        return GetValueAtLevel(upgradeLevel + 1);
    }

}
