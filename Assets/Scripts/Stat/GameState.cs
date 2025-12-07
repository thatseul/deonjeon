using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameState : MonoBehaviour
{
    public static GameState I { get; private set; }

    [Header("골드")]
    public double Gold;

    [Header("보스 씬 자동골드")]
    public double PassiveIncomePerSecond;

    DateTime _lastTickUtc;
    bool _isInMain;


    // ================================================================
    // 몬스터 인벤 저장 구조 (위치 포함)
    // ================================================================
    [Serializable]
    public class OwnedMonsterData
    {
        public MonsterItem item;
        public Vector2 anchoredPosition; // 랜덤 위치 저장
    }

    [Header("몬스터 인벤 데이터")]
    public List<OwnedMonsterData> ownedMonsters = new();

    [Header("인벤 수용량")]
    public int monsterCapacityUnlocked = 1;
    public const int monsterCapacityMax = 8;

    // ================================================================
    // 퀵슬롯
    // ================================================================
    public MonsterItem[] quickSlotItems = new MonsterItem[3];


    // ================================================================
    // 스탯 저장
    // ================================================================
    [SerializeField] public List<DungeonStatSaveData> dungeonStats = new();
    [Serializable]
    public class DungeonStatSaveData
    {
        public string name;
        public int level;
    }


    // ================================================================
    // Unity lifecycle
    // ================================================================
    void Awake()
    {
        if (I != null)
        {
            Destroy(gameObject);
            return;
        }

        I = this;
        DontDestroyOnLoad(gameObject);

        if (quickSlotItems == null || quickSlotItems.Length == 0)
            quickSlotItems = new MonsterItem[3];

        _lastTickUtc = DateTime.UtcNow;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (I == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene sc, LoadSceneMode mode)
    {
        _isInMain = sc.name == "deonjeon2020";
        _lastTickUtc = DateTime.UtcNow;
    }

    private void Update()
    {
        if (!_isInMain && PassiveIncomePerSecond > 0)
        {
            var now = DateTime.UtcNow;
            var dt = (now - _lastTickUtc).TotalSeconds;

            if (dt > 0)
            {
                Gold += PassiveIncomePerSecond * dt;
                _lastTickUtc = now;
            }
        }
        else
        {
            _lastTickUtc = DateTime.UtcNow;
        }
    }


    // ================================================================
    // 골드
    // ================================================================
    public void AddGold(double amount) => Gold += amount;

    public bool TrySpend(double amount)
    {
        if (Gold < amount) return false;
        Gold -= amount;
        return true;
    }


    // ================================================================
    // 스탯 저장/로드
    // ================================================================
    public void SaveDungeonStats(List<DungeonStat> stats)
    {
        dungeonStats.Clear();

        foreach (var s in stats)
        {
            dungeonStats.Add(new DungeonStatSaveData
            {
                name = s.name,
                level = s.upgradeLevel
            });
        }
    }

    public void LoadDungeonStatsInto(DungeonStatsManager mgr)
    {
        foreach (var stat in mgr.stats)
        {
            var saved = dungeonStats.Find(d => d.name == stat.name);
            if (saved != null)
                stat.upgradeLevel = saved.level;
        }
    }


    // ================================================================
    // 인벤: 추가 / 삭제
    // ================================================================
    public void AddOwnedMonster(MonsterItem item, Vector2 pos)
    {
        ownedMonsters.Add(new OwnedMonsterData
        {
            item = item,
            anchoredPosition = pos
        });
    }

    public void RemoveOwnedMonster(MonsterItem item)
    {
        for (int i = 0; i < ownedMonsters.Count; i++)
        {
            if (ownedMonsters[i].item == item)
            {
                ownedMonsters.RemoveAt(i);
                return;
            }
        }
    }


    // ================================================================
    // 퀵슬롯 저장/로드
    // ================================================================
    public MonsterItem GetQuickSlotItem(int index)
    {
        if (index < 0 || index >= quickSlotItems.Length)
            return null;
        return quickSlotItems[index];
    }

    public void SetQuickSlotItem(int index, MonsterItem item)
    {
        if (index < 0 || index >= quickSlotItems.Length)
            return;

        quickSlotItems[index] = item;
    }
}
