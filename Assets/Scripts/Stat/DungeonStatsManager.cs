using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DungeonStatsManager : MonoBehaviour
{
    public int dungeonLevel = 1;

    [Tooltip("HP, ATK, SPD 등 추가 가능한 능력치 목록입니다.")]
    public List<DungeonStat> stats;

    public int upgradeThreshold = 3;
    public static DungeonStatsManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); return; }

        if (stats == null || stats.Count == 0)
        {
            stats = new List<DungeonStat>
            {
                new DungeonStat("HP",   100){ baseCost = 4, costMultiplier = 1.5f },
                new DungeonStat("ATK",   10){ baseCost = 3, costMultiplier = 1.5f },
                new DungeonStat("ASPD",   1){ baseCost = 2, costMultiplier = 1.5f },
            };
        }
        else
        {
            // 이미 인스펙터에서 세팅된 경우, 누락된 기본가만 안전 보정
            var hp = stats.Find(s => s.name == "HP"); if (hp != null && hp.baseCost == 0) hp.baseCost = 4;
            var atk = stats.Find(s => s.name == "ATK"); if (atk != null && atk.baseCost == 0) atk.baseCost = 3;
            var aspd = stats.Find(s => s.name == "ASPD"); if (aspd != null && aspd.baseCost == 0) aspd.baseCost = 2;
        }
    }

    public void UpgradeStat(string statName)
    {
        var stat = stats.Find(s => s.name == statName);
        if (stat != null)
        {
            stat.Upgrade();
            Debug.Log($"🔧 {stat.name} 업그레이드 → Lv.{stat.upgradeLevel}");
            CheckLevelUp();
        }
    }

    private void CheckLevelUp()
    {
        // 모든 스탯 중 최솟값
        int minLv = stats.Min(s => s.upgradeLevel);

        // 이번 레벨업에 필요한 하한(현재 던전레벨 * 임계치)
        int required = dungeonLevel * upgradeThreshold;

        // 예: dungeonLevel=1, upgradeThreshold=3 → minLv가 3 이상일 때 2로 상승
        if (minLv >= required)
        {
            dungeonLevel++;
            Debug.Log($"🏰 던전 레벨 UP! 현재 Lv.{dungeonLevel}");
        }
    }

    public float GetStatValue(string statName)
    {
        var stat = stats.Find(s => s.name == statName);
        return stat != null ? stat.CurrentValue : 0;
    }

    public int GetUpgradeCost(string statName)
    {
        var stat = stats.Find(s => s.name == statName);
        return stat != null ? stat.GetUpgradeCost() : int.MaxValue;
    }

    public int GetStatLevel(string statName)
    {
        var stat = stats.Find(s => s.name == statName);
        return stat != null ? stat.upgradeLevel : 0;
    }
    // ⬇ 클래스 하단에 보조 메서드 2개 추가
    public float GetStatCurrentValue(string statName)
    {
        var stat = stats.Find(s => s.name == statName);
        return stat != null ? stat.GetValueAtLevel(stat.upgradeLevel) : 0f;
    }

    public float GetStatNextValue(string statName)
    {
        var stat = stats.Find(s => s.name == statName);
        return stat != null ? stat.GetNextValue() : 0f;
    }

}
