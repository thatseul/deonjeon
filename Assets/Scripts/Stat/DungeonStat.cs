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

    public float CurrentValue
{
    get
    {
        if (name == "ASPD")
        {
            float aspd = baseValue;

            for (int i = 0; i < upgradeLevel; i++)
            {
                aspd *= 1.02f; // 1.02배씩 곱해서 점점 빨라짐
            }

            return Mathf.Min(aspd, 3f); // 최대 3까지 제한
        }

        // HP, ATK 등 나머지는 기존 계산식 유지
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

    /// <summary>
    /// 현재 업그레이드 비용 계산 함수
    /// </summary>
    public int GetUpgradeCost()
    {
        // 예: (현재 레벨 + 1) * 5 (스탯마다 커스터마이징 가능하게 확장 가능)
        return (upgradeLevel + 1) * 5;
    }
}
