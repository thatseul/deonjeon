using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DungeonStatsManager : MonoBehaviour
{
    [Header("던전 전체 레벨")]
    [Tooltip("세 능력치의 업그레이드가 임계치에 도달하면 던전 레벨이 올라갑니다.")]
    public int dungeonLevel = 1;

    [Header("업그레이드 가능한 능력치들")]
    [Tooltip("HP, ATK, SPD 등 추가 가능한 능력치 목록입니다.")]
    public List<DungeonStat> stats;

    [Header("레벨업 조건")]
    [Tooltip("모든 능력치가 이 업그레이드 레벨 이상일 경우 던전 레벨이 상승합니다.")]
    public int upgradeThreshold = 3;

    public static DungeonStatsManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (stats == null || stats.Count == 0)
        {
            stats = new List<DungeonStat>
        {
            new DungeonStat("HP", 100),
            new DungeonStat("ATK", 10),
            new DungeonStat("ASPD", 1)
        };
        }
    }



    /// <summary>
    /// 특정 능력치를 업그레이드합니다.
    /// </summary>
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

    /// <summary>
    /// 모든 능력치가 업그레이드 기준에 도달했는지 확인합니다.
    /// </summary>
    private void CheckLevelUp()
    {
        bool allAboveThreshold = stats.All(s => s.upgradeLevel >= upgradeThreshold);
        if (allAboveThreshold)
        {
            dungeonLevel++;
            Debug.Log($"🏰 던전 레벨 UP! 현재 Lv.{dungeonLevel}");

            // TODO: 던전 레벨업 시 UI 애니메이션 추가 예정
            foreach (var s in stats)
                s.upgradeLevel = 0; // 초기화해서 다시 도전 가능하게 할 수도 있음
        }
    }

    /// <summary>
    /// 특정 능력치의 현재 수치를 가져옵니다.
    /// </summary>
    public float GetStatValue(string statName)
    {
        var stat = stats.Find(s => s.name == statName);
        return stat != null ? stat.CurrentValue : 0;
    }

    /// 비용함수 호출
    public int GetUpgradeCost(string statName)
    {
        var stat = stats.Find(s => s.name == statName);
        if (stat == null) return int.MaxValue;

        return stat.GetUpgradeCost();
    }
    public bool CanUpgrade(string statName)
    {
        var stat = stats.Find(s => s.name == statName);
        if (stat == null) return false;

        if (stat.name == "ASPD" && stat.CurrentValue >= 3f)
        {
            Debug.LogWarning("⚠️ ASPD는 더 이상 업그레이드할 수 없습니다!");
            return false;
        }

        return true;
    }

}
